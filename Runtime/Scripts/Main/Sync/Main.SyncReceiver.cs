
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        public bool IsInCollider { get; set; } = true;
        private bool _SYR_hasEverBeenSynced = false;




        private void SYR_Reset()
        {
            _SYR_hasEverBeenSynced = false;
        }


        public override void OnDeserialization()
        {
            _SYR_OnSynced();
        }


        private void _SYR_OnSynced()
        {
            CurrGameState = (GameState)(int)SYD_gameState;

            if (CurrGameState == GameState.Title)
            {
                _SYR_ResolveSyncedData();
                GLP_Reset();
            }
            else if (CurrGameState == GameState.Playing)
            {
                if (!_SYR_hasEverBeenSynced)
                {
                    GCT_Start();
                    GRR_Start();
                    PVR_Start();
                    UIM_Start();
                    STM_DisableStation();
                    _SYR_hasEverBeenSynced = true;
                }

                if (RCL_IsInCollider())
                {
                    _SYR_ResolveSyncedData();
                    _SYR_ReflectBlockRendering();

                    UIM_Update();
                }
            }
            else if (CurrGameState == GameState.GameOver)
            {
                if (!_SYR_hasEverBeenSynced)
                {
                    GCT_Start();
                    GRR_Start();
                    PVR_Start();
                    UIM_Start();
                    STM_DisableStation();
                    _SYR_hasEverBeenSynced = true;
                }

                _SYR_ResolveSyncedData();
                _SYR_ReflectBlockRendering(false);

                UIM_OnGameOver();
            }
            else if (CurrGameState == GameState.Resettable)
            {
                if (!_SYR_hasEverBeenSynced)
                {
                    GCT_Start();
                    GRR_Start();
                    PVR_Start();
                    UIM_Start();
                    STM_DisableStation();
                    _SYR_hasEverBeenSynced = true;
                }

                _SYR_ResolveSyncedData();
                _SYR_ReflectBlockRendering(false);

                UIM_OnResettable();
            }
        }


        private void _SYR_ResolveSyncedData()
        {
            _SYR_DecompressData();
            _SYR_CopyFromSyncedData();
        }


        private void _SYR_ReflectBlockRendering(bool shouldReflectMino = true)
        {
            _SYR_ReflectGrid();
            PVR_ShowHoldMino(HLR_HoldMinoType);
            PVR_ShowQueue(SPN_minoQueue);
            if (shouldReflectMino && CurrMinoType != MinoType.None)
            {
                GRR_ShowMino(currMinoPos, CurrMinoType);
            }
        }


        private void _SYR_DecompressData()
        {
            _SYR_DecompressMinos(SYD_compressedMinos, currMinoPos, out MinoType _minoType, out MinoType _holdMinoType);
            CurrMinoType = _minoType;
            HLR_HoldMinoType = _holdMinoType;

            _SYR_DecompressGrid(SYD_compressedGrid, grid);
            _SYR_DecompressMinoQueue(SYD_compressedMinoQueue, SPN_minoQueue);
        }


        private void _SYR_CopyFromSyncedData()
        {
            PlayId = SYD_playId;

            STT_Level      = SYD_level;
            STT_Score      = SYD_score;
            DRS_ScoreDelta = SYD_scoreDelta;

            STT_ApplySyncedData(SYD_recordGameStats);
            DRS_ApplySyncedData(SYD_recordDropResults);
        }


        private void _SYR_ReflectGrid()
        {
            for (byte i = 0; i < grid.Length; i++)
            {
                if (grid[i] == MinoType.None)
                {
                    GRR_HideBlock(i);
                }
                else
                {
                    GRR_ShowBlock(i, grid[i]);
                }
            }
        }








        // -------- 展開 --------


        private void _SYR_DecompressMinos(uint compressed, Vector2Int[] minoPos, out MinoType minoType, out MinoType holdMinoType)
        {
            int firstIndex = (int)(compressed & 0b11111111);
            minoPos[0] = new Vector2Int(firstIndex % 10, firstIndex / 10);

            for (int i = 1; i < minoPos.Length; i++)
            {
                int index = (int)((compressed >> (8 + 5 * (i - 1))) & 0b11111);
                int deltaX = (index % 5) - 2;
                int deltaY = (index / 5) - 2;
                minoPos[i] = new Vector2Int(minoPos[0].x + deltaX, minoPos[0].y + deltaY);
            }

            minoType = (MinoType)((compressed >> (8 + 15)) & 0b111);

            holdMinoType = (MinoType)((compressed >> (8 + 15 + 3)) & 0b111);
        }


        private void _SYR_DecompressGrid(ulong[] compressed, MinoType[] grid)
        {
            for (int i = 0; i < compressed.Length; i++)
            {
                ulong row = compressed[i];
                for (int j = 0; j < 20; j++)
                {
                    byte threeBits = (byte)((row >> (j * 3)) & 0b111);
                    grid[i * 20 + j] = (MinoType)threeBits;
                }
            }

            // 200個目以降のグリッドを展開（あえて初期化はしない）
            byte color = (byte)((compressed[compressed.Length - 1] >> 60) & 0b111);
            for (int i = 0; i < 4; i++)
            {
                byte fourBits1 = (byte)((compressed[i * 2] >> 60) & 0b1111);
                byte fourBits2 = (byte)((compressed[i * 2 + 1] >> 60) & 0b1111);
                byte index = (byte)(fourBits1 | (fourBits2 << 4));
                if (index >= 200)
                {
                    grid[index] = (MinoType)color;
                }
            }
        }


        private void _SYR_DecompressMinoQueue(ushort compressed, MinoType[] minoQueue)
        {
            for (int i = 0; i < minoQueue.Length; i++)
            {
                byte threeBits = (byte)((compressed >> (i * 3)) & 0b111);
                minoQueue[i] = (MinoType)threeBits;
            }
        }
    }
}
