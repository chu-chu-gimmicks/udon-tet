
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        // 同期のインターバル
        private const float _SYS_SYNC_INTERVAL = 0.5f;
        private const float _SYS_RETRY_INTERVAL = 0.5f; // ネットワークが詰まっていた際の再同期用
        private const float _SYS_PERIODIC_SYNC_INTERVAL = 1.0f;
        // SendCustomEventDelayedSecondsの予約状況
        private bool _SYS_isPendingSync = false;
        private bool _SYS_isPendingPeriodicSync = false;
        // 最後に同期した時間
        private double _SYS_lastSyncTime = double.MinValue;




        private void SYS_Reset()
        {
        }


        // 任意のタイミングで呼ぶ
        private void SYS_RequestSync()
        {
            // 重複予約回避
            if (_SYS_isPendingSync) { return; }
            if (!Networking.IsOwner(this.gameObject)) { return; }

            double now = Time.timeAsDouble;

            if (now >= _SYS_lastSyncTime + _SYS_SYNC_INTERVAL)
            {
                // 前回の同期から SyncInterval 秒経っていれば即同期実行
                _SYS_ExecuteSync();
            }
            else
            {
                // 次に実行可能なタイミングまでの差分だけ待つ
                float delay = (float)(_SYS_lastSyncTime + _SYS_SYNC_INTERVAL - now) + 0.01f;

                if (delay > 0.0f)
                {
                    SendCustomEventDelayedSeconds(nameof(_SYS_ExecuteSync), delay);
                    _SYS_isPendingSync = true;
                }
                else
                {
                    // 万が一待機時間がなかったら即同期実行
                    _SYS_ExecuteSync();
                }
            }
        }


        public void _SYS_ExecuteSync()
        {
            _SYS_isPendingSync = false;

            if (!Networking.IsOwner(this.gameObject)) { return; }

            if (Networking.IsClogged)
            {
                // ネットワークが詰まっていたら RetryInterval 秒後に同期予約
                SendCustomEventDelayedSeconds(nameof(_SYS_ExecuteSync), _SYS_RETRY_INTERVAL);
                _SYS_isPendingSync = true;
            }
            else
            {
                // 正常なら同期実行
                Sync_compressedMinos = _SYS_CompressMinos(currentMinoPos, CurrentGameState, CurrentMinoType, HR_HoldMinoType);
                Sync_compressedGrid = _SYS_CompressGrid(G_grid, Sync_compressedGrid);
                Sync_compressedMinoQueue = _SYS_CompressMinoQueue(S_minoQueue);
                _SYS_CopyToSyncedData();

                RequestSerialization();
                _SYS_lastSyncTime = Time.timeAsDouble;
            }
        }


        // 定期的な補完同期
        private void SYS_RequestPeriodicSync()
        {
            // 重複予約回避
            if (_SYS_isPendingPeriodicSync) { return; }
            if (!Networking.IsOwner(this.gameObject)) { return; }

            SendCustomEventDelayedSeconds(nameof(_SYS_ExecutePeriodicSync), _SYS_PERIODIC_SYNC_INTERVAL);
            _SYS_isPendingPeriodicSync = true;
        }


        public void _SYS_ExecutePeriodicSync()
        {
            _SYS_isPendingPeriodicSync = false;

            if (!Networking.IsOwner(this.gameObject)) { return; }

            SYS_RequestSync();

            if (CurrentGameState == GameState.Playing)
            {
                SYS_RequestPeriodicSync();
            }
        }


        private void _SYS_CopyToSyncedData()
        {
            Sync_playId = PlayId;

            Sync_level = ST_Level;
            Sync_score = ST_Score;
            Sync_scoreDelta = DR_ScoreDelta;

            Sync_line  = DR_Line;
            Sync_combo = DR_Combo;
            Sync_tSpin = (byte)(int)DR_TSpin;
            Sync_bTB   = DR_BTB;
            Sync_block = DR_Block;

            Sync_lineStat    = ST_Line;
            Sync_comboStat   = ST_Combo;
            Sync_tSpinStat   = ST_TSpin;
            Sync_bTBStat     = ST_BTB;
            Sync_perfectStat = ST_Perfect;
        }








        // -------- 圧縮 --------


        // 32 + 4 + 1 + 1byte �� 4byte �Ɉ��k
        private uint _SYS_CompressMinos(Vector2Int[] mino, GameState state, MinoType minoType, MinoType holdMinoType)
        {
            uint compressed = 0;

            compressed |= (uint)((int)state & 0b11);

            compressed |= (uint)(((int)minoType & 0b111) << 2);

            compressed |= (uint)(((int)holdMinoType & 0b111) << (2 + 3));

            compressed |= (uint)(((mino[0].x + mino[0].y * 10) & 0b11111111) << (2 + 3 + 3));

            for (int i = 1; i < mino.Length; i++)
            {
                int deltaX = mino[i].x - mino[0].x + 2;
                int deltaY = mino[i].y - mino[0].y + 2;
                int index = deltaX + deltaY * 5;
                compressed |= (uint)((index & 0b11111) << (((i - 1) * 5) + 2 + 3 + 3 + 8));
            }

            return compressed;
        }


        // byte[250] �� ulong[10] �Ɉ��k
        private ulong[] _SYS_CompressGrid(MinoType[] grid, ulong[] compressed)
        {
            if (grid.Length != 250 || compressed.Length != 10) { return compressed; }

            ulong row = 0;
            for (int i = 0; i < 200; i++)
            {
                int index = i % 20;
                // �r�b�g���]����̂́A�l������Ȃ��r�b�g�����ׂ�1�ɂ��邽��
                ulong threeBits = (ulong)(~(int)grid[i] & 0b111) << (index * 3);
                row |= threeBits;

                if (index == 19)
                {
                    compressed[i / 20] = row;
                    row = 0;
                }
            }

            // 200�ڈȍ~�� grid �̈��k
            int count = 0;
            for (int i = 200; i < grid.Length; i++)
            {
                if ((int)grid[i] == byte.MaxValue) { continue; }

                if (count == 0)
                {
                    ulong threeBits = (ulong)(~(int)grid[i] & 0b111);
                    compressed[0] |= threeBits << 60;
                }

                // �r�b�g���]����̂́A�l������Ȃ��r�b�g�����ׂ�1�ɂ��邽��
                ulong fourBits1 = (ulong)(~i & 0b1111);
                ulong fourBits2 = (ulong)((~i >> 4) & 0b1111);
                compressed[count * 2 + 1] |= fourBits1 << 60;
                compressed[count * 2 + 2] |= fourBits2 << 60;
                count++;

                if (count == 4) { break; }
            }

            // �l������Ȃ��r�b�g�����ׂ�1�ɂ���
            for (int i = 0; i < compressed.Length; i++)
            {
                compressed[i] = ~compressed[i];
            }

            return compressed;
        }


        // 5byte �� 2byte �Ɉ��k
        private ushort _SYS_CompressMinoQueue(MinoType[] minoQueue)
        {
            if (minoQueue.Length != 5) { return 0; }

            ushort compressed = 0;
            for (int i = 0; i < minoQueue.Length; i++)
            {
                ushort threeBits = (ushort)((int)minoQueue[i] & 0b111);
                compressed |= (ushort)(threeBits << (i * 3));
            }
            return compressed;
        }
    }
}
