
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
                GLP_Reset();

                _SYR_isAlreadySynced = false;
            }
            else if (CurrentGameState == GameState.Playing)
            {
                if (_SYR_isAlreadySynced)
                {
                    if (ReflectOnlyInCollider && !IsInCollider) { return; }

                    _SYR_SynchronizeGrid();
                    GRR_ShowMino(currentMinoPos, CurrentMinoType);
                    PVR_ShowHoldMino(HLR_HoldMinoType);
                    PVR_ShowQueue(SPN_minoQueue);
                    UIM_Update();
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
                UIM_OnGameOver();

                rangeCollider.enabled = false;
                _SYR_isAlreadySynced = false;
            }
        }


        private void _SYR_DecompressData()
        {
            _SYR_DecompressMinos(SYD_compressedMinos, currentMinoPos, out GameState _gameState, out MinoType _minoType, out MinoType _holdMinoType);
            CurrentGameState = _gameState;
            CurrentMinoType = _minoType;
            HLR_HoldMinoType = _holdMinoType;

            _SYR_DecompressGrid(SYD_compressedGrid, grid);
            _SYR_DecompressMinoQueue(SYD_compressedMinoQueue, SPN_minoQueue);
        }


        private void _SYR_CopyFromSyncData()
        {
            PlayId = SYD_playId;

            STT_Level      = SYD_level;
            STT_Score      = SYD_score;
            DRS_ScoreDelta = SYD_scoreDelta;

            DRS_Line  = SYD_line;
            DRS_Combo = SYD_combo;
            DRS_TSpin = (TSpinState)SYD_tSpin;
            DRS_BTB   = SYD_bTB;
            DRS_Block = SYD_block;

            STT_Line    = SYD_lineStat;
            STT_Combo   = SYD_comboStat;
            STT_TSpin   = SYD_tSpinStat;
            STT_BTB     = SYD_bTBStat;
            STT_Perfect = SYD_perfectStat;
        }


        private void _SYR_StartSyncDataReflection()
        {
            if (Networking.IsOwner(this.gameObject)) { return; }

            _SYR_SynchronizeGrid();
            if (CurrentGameState == GameState.Playing)
            {
                GRR_ShowMino(currentMinoPos, CurrentMinoType);
            }
            PVR_ShowHoldMino(HLR_HoldMinoType);
            PVR_ShowQueue(SPN_minoQueue);

            if (STT_Line == 0)
            {
                // ゲーム開始直後の UI に Perfect と表示されてしまうため、それを防ぐ
                DRS_Block = 1;
            }
            UIM_Start();
            UIM_Update();
        }


        private void _SYR_SynchronizeGrid()
        {
            for (byte i = 0; i < grid.Length; i++)
            {
                GRR_ShowBlock(i, grid[i]);
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
