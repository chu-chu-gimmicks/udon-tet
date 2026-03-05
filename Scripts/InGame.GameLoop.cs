
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        public bool ReflectOnlyInCollider { get; set; } = true;
        public bool IsInCollider { get; set; } = true;

        private Vector2Int[] _GLP_minoBuffer = new Vector2Int[4];




        private void GLP_UpdateGameState()
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                // 他のプレイヤーがプレイ中でないなら、オーナーを変更
                if (CurrentGameState != GameState.Playing)
                {
                    Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
                }
                // 他のプレイヤーがプレイ中なら反応しない
                else if (CurrentGameState == GameState.Playing)
                {
                    return;
                }
            }

            if (CurrentGameState == GameState.GameOver)
            {
                CurrentGameState = GameState.Title;
                GLP_Reset();
            }
            else if (CurrentGameState == GameState.Title)
            {
                CurrentGameState = GameState.Playing;
                _GLP_Start();
            }

            SYS_RequestSync();
        }


        private void GLP_Reset()
        {
            DRP_Reset();
            LDC_Reset();
            GRM_Reset();
            GRA_Reset();
            GRD_Reset();
            GRR_Reset();

            SPN_Reset();
            PVR_Reset();

            ACM_Reset();
            INR_Reset();
            MVR_Reset();
            SPR_Reset();
            SDR_Reset();
            HDR_Reset();
            HLR_Reset();
            ADR_Reset();

            UIR_Reset();

            STM_Reset();

            SYD_Reset();
            SYS_Reset();
            SYR_Reset();

            updateHandler.enabled = false;
            rangeCollider.enabled = false;
        }


        private void _GLP_Start()
        {
            // ランキング反映までの猶予を稼ぐため、ここで初期化する
            // 演出で数秒稼ぐ、ボタンを数秒後に表示等もあり
            GST_Reset();
            DRS_Reset();
            STT_Reset();

            _GLP_SpawnMino();
            UIM_Start();

            updateHandler.enabled = true;
            rangeCollider.enabled = false;

            SYS_RequestPeriodicSync();
        }


        public void GLP_Update()
        {
            INR_Update();
        }


        public void GLP_LateUpdate()
        {
            if (!Networking.IsOwner(this.gameObject)) { return; }
            if (CurrentGameState != GameState.Playing) { return; }
            if (CurrentClearAnimState != ClearAnimationState.Idle)
            {
                if (CurrentClearAnimState == ClearAnimationState.Completed)
                {
                    _GLP_SpawnAfterAnimation();
                }
                return;
            }

            CopyMino(currentMinoPos, _GLP_minoBuffer);

            int actions = ACM_ResolvedActions();

            if ((actions & (int)PlayerAction.FirstHold) != 0)
            {
                _GLP_SpawnMino();
            }
            else if ((actions & (int)PlayerAction.Hold) != 0)
            {
                _GLP_SpawnBySubsequentHold();
            }
            else if ((actions & (int)PlayerAction.HardDrop) != 0)
            {
                _GLP_LockDown();
            }
            else
            {
                _GLP_Drop();
                if (LDC_NeedsLockDown(currentMinoPos))
                {
                    _GLP_LockDown();
                }
            }
        }


        private void _GLP_Drop()
        {
            DRP_Drop(currentMinoPos, out bool hasDropped);

            if (hasDropped)
            {
                DRS_TSpin = TSpinState.None;

                LDC_UpdateByDrop(currentMinoPos);

                GRR_HideMino(_GLP_minoBuffer);
                GRR_ShowMino(currentMinoPos, CurrentMinoType);
                CopyMino(currentMinoPos, _GLP_minoBuffer);
            }
        }


        private void _GLP_LockDown()
        {
            DRS_AddScoreDelta();

            GRM_SaveMino(currentMinoPos, CurrentMinoType);
            int completeLines = GRM_CompleteLines(currentMinoPos);
            if (completeLines > 0)
            {
                GRM_ClearLines();
                GRM_ShiftLinesDown();

                if (STT_NeedsLevelUp())
                {
                    STT_LevelUp();
                    DRP_UpdateInterval();
                }

                DRS_CalculateScoreDelta();

                GRA_AnimateClear();
            }
            else
            {
                if (GRD_IsGameOver(currentMinoPos))
                {
                    GLP_OnGameOver();
                    return;
                }

                _GLP_SpawnMino();
                UIM_Update();
            }
        }


        private void GLP_OnGameOver()
        {
            CurrentGameState = GameState.GameOver;

            UIM_OnGameOver();
            STM_OnGameOver();

            PlayId++;
            UpdateUCS(STT_Score);

            updateHandler.enabled = false;
            rangeCollider.enabled = false;

            SYS_RequestSync();
        }


        private void _GLP_SpawnMino()
        {
            CurrentMinoType = SPN_SpawnMino(currentMinoPos);
            _GLP_InitializeWhenSpawn();
        }


        private  void _GLP_SpawnBySubsequentHold()
        {
            SPN_SetMino(currentMinoPos, CurrentMinoType);
            _GLP_InitializeWhenSpawn(false);
        }


        private void _GLP_InitializeWhenSpawn(bool canHold = true)
        {
            GRR_ShowMino(currentMinoPos, CurrentMinoType);
            Angle = 0;
            CanHold = canHold;
            DRS_TSpin = TSpinState.None;
            LDC_Reset();
        }


        private void _GLP_SpawnAfterAnimation()
        {
            CurrentClearAnimState = ClearAnimationState.Idle;

            if (GRD_IsGameOver(currentMinoPos))
            {
                GLP_OnGameOver();
                return;
            }

            _GLP_SpawnMino();
            UIM_Update();
        }




        // デバッグ用
        private void GLP_DebugLevel()
        {
            if (CurrentGameState != GameState.Playing) { return; }
            STT_Level = 19;
            STT_Line = 199;
        }
    }
}
