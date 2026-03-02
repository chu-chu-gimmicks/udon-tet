
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private bool _SYR_isAlreadySynced = false;




        private void SYR_Reset()
        {
            _SYR_isAlreadySynced = false;
        }


        public override void OnDeserialization()
        {
            _SYR_OnSynced();
        }


        private void _SYR_OnSynced()
        {
            _SYR_DecompressData();
            _SYR_CopyFromSyncData();

            if (CurrentGameState == GameState.Title)
            {
                GL_Reset();

                _SYR_isAlreadySynced = false;
            }
            else if (CurrentGameState == GameState.Playing)
            {
                if (_SYR_isAlreadySynced)
                {
                    if (ReflectOnlyInCollider && !IsInCollider) { return; }

                    _SYR_SynchronizeGrid();
                    GR_ShowMino(currentMinoPos, CurrentMinoType);
                    PR_ShowHoldMino(HR_HoldMinoType);
                    PR_ShowQueue(S_minoQueue);
                    UI_Update();
                }
                else
                {
                    _SYR_StartSyncDataReflection();

                    rangeCollider.enabled = true;
                    _SYR_isAlreadySynced = true;
                }
            }
            else if (CurrentGameState == GameState.GameOver)
            {
                _SYR_StartSyncDataReflection();
                UI_OnGameOver();

                rangeCollider.enabled = false;
                _SYR_isAlreadySynced = false;
            }
        }


        private void _SYR_DecompressData()
        {
            _SYR_DecompressMinos(Sync_compressedMinos, currentMinoPos, out GameState _gameState, out MinoType _minoType, out MinoType _holdMinoType);
            CurrentGameState = _gameState;
            CurrentMinoType = _minoType;
            HR_HoldMinoType = _holdMinoType;

            _SYR_DecompressGrid(Sync_compressedGrid, G_grid);
            _SYR_DecompressMinoQueue(Sync_compressedMinoQueue, S_minoQueue);
        }


        private void _SYR_CopyFromSyncData()
        {
            PlayId = Sync_playId;

            ST_Level      = Sync_level;
            ST_Score      = Sync_score;
            DR_ScoreDelta = Sync_scoreDelta;

            DR_Line  = Sync_line;
            DR_Combo = Sync_combo;
            DR_TSpin = (TSpinState)Sync_tSpin;
            DR_BTB   = Sync_bTB;
            DR_Block = Sync_block;

            ST_Line    = Sync_lineStat;
            ST_Combo   = Sync_comboStat;
            ST_TSpin   = Sync_tSpinStat;
            ST_BTB     = Sync_bTBStat;
            ST_Perfect = Sync_perfectStat;
        }


        private void _SYR_StartSyncDataReflection()
        {
            if (Networking.IsOwner(this.gameObject)) { return; }

            _SYR_SynchronizeGrid();
            if (CurrentGameState == GameState.Playing)
            {
                GR_ShowMino(currentMinoPos, CurrentMinoType);
            }
            PR_ShowHoldMino(HR_HoldMinoType);
            PR_ShowQueue(S_minoQueue);

            if (ST_Line == 0)
            {
                // ゲーム開始直後の UI に Perfect と表示されてしまうため、それを防ぐ
                DR_Block = 1;
            }
            UI_Start();
            UI_Update();
        }


        private void _SYR_SynchronizeGrid()
        {
            for (byte i = 0; i < G_grid.Length; i++)
            {
                GR_ShowBlock(i, G_grid[i]);
            }
        }








        // -------- 展開 --------


        private void _SYR_DecompressMinos(uint compressed, Vector2Int[] minoPos, out GameState state, out MinoType minoType, out MinoType holdMinoType)
        {
            state = (GameState)(compressed & 0b11);

            minoType = (MinoType)((compressed >> 2) & 0b111);

            holdMinoType = (MinoType)((compressed >> (2 + 3)) & 0b111);

            int firstIndex = (int)((compressed >> (2 + 3 + 3)) & 0b11111111);
            minoPos[0] = new Vector2Int(firstIndex % 10, firstIndex / 10);

            for (int i = 1; i < minoPos.Length; i++)
            {
                int index = (int)((compressed >> ((i - 1) * 5 + 2 + 3 + 3 + 8)) & 0b11111);
                int deltaX = (index % 5) - 2;
                int deltaY = (index / 5) - 2;
                minoPos[i] = new Vector2Int(minoPos[0].x + deltaX, minoPos[0].y + deltaY);
            }
        }


        private void _SYR_DecompressGrid(ulong[] compressed, MinoType[] grid)
        {
            if (grid.Length != 250 || compressed.Length != 10) { return; }

            for (int i = 0; i < compressed.Length; i++)
            {
                ulong row = compressed[i];
                for (int j = 0; j < 20; j++)
                {
                    byte threeBits = (byte)((row >> (j * 3)) & 0b111);
                    if (threeBits == 0b111) { threeBits = byte.MaxValue; }
                    grid[i * 20 + j] = (MinoType)threeBits;
                }
            }

            // 200個目以降のグリッドを初期化
            for (int i = 200; i < grid.Length; i++)
            {
                grid[i] = (MinoType)byte.MaxValue;
            }

            // 200個目以降のグリッドを展開
            byte color = (byte)((compressed[0] >> 60) & 0b111);
            for (int i = 0; i < 4; i++)
            {
                byte fourBits1 = (byte)((compressed[i * 2 + 1] >> 60) & 0b1111);
                byte fourBits2 = (byte)((compressed[i * 2 + 2] >> 60) & 0b1111);
                byte index = (byte)(fourBits1 | (fourBits2 << 4));

                if (index == 0b11111111) { break; }
                grid[index] = (MinoType)color;
            }
        }


        private void _SYR_DecompressMinoQueue(ushort compressed, MinoType[] minoQueue)
        {
            if (minoQueue.Length != 5) { return; }

            for (int i = 0; i < minoQueue.Length; i++)
            {
                byte threeBits = (byte)((compressed >> (i * 3)) & 0b111);
                if (threeBits == 0b111) { threeBits = byte.MaxValue; }
                minoQueue[i] = (MinoType)threeBits;
            }
        }
    }
}
