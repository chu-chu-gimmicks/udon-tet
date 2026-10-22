
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        private Vector2Int[] _GLP_minoBuffer = new Vector2Int[4];




        private void GLP_UpdateGameState()
        {
            if (!Networking.IsOwner(this.gameObject))
            {
                // 他のプレイヤーがプレイ中でないなら、オーナーを変更
                if (CurrGameState != GameState.Playing)
                {
                    Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
                }
                // 他のプレイヤーがプレイ中なら反応しない
                else if (CurrGameState == GameState.Playing)
                {
                    return;
                }
            }

            if (CurrGameState == GameState.Resettable)
            {
                CurrGameState = GameState.Title;
                GLP_Reset();
            }
            else if (CurrGameState == GameState.Title)
            {
                CurrGameState = GameState.Playing;
                _GLP_Start();
            }

            SYS_RequestSync();
        }


        private void GLP_Reset()
        {
            // Data
            MINO_Reset();
            GCT_Reset();
            DRS_Reset();
            STT_Reset();

            // Grid
            DRP_Reset();
            LDC_Reset();
            GRM_Reset();
            GRA_Reset();
            GRD_Reset();
            GRR_Reset();

            // Spawn
            SPN_Reset();
            PVR_Reset();

            // Action
            ACM_Reset();

            // UI
            UIM_Reset();

            // Station
            STM_Reset();

            // Sync
            SYD_Reset();
            SYS_Reset();
            SYR_Reset();
            RCL_Reset();

            _updateHandler.enabled = false;
        }


        private void _GLP_Start()
        {
            GCT_Start(); 

            // Grid
            GRR_Start();

            // Spawn
            PVR_Start();
            _GLP_SpawnMino();

            // UI
            UIM_Start();

            // Sync
            SYS_RequestPeriodicSync();

            _updateHandler.enabled = true;
        }


        public void GLP_Update()
        {
            if (CurrGameState != GameState.Playing) { return; }

            ProcessInput();

            if (GRA_IsAnimating())
            {
                _GLP_ProcessInAnimation();
                return;
            }

            int actions = _GLP_ProcessActions();
            _GLP_ProcessGameLogic(actions);
        }


        private void ProcessInput()
        {
            INR_Update();
        }


        private int _GLP_ProcessActions()
        {
            CopyMino(currMinoPos, _GLP_minoBuffer);
            int actions = ACM_ResolvedActions(_GLP_minoBuffer);
            if (ACM_HasAnyAction(actions) && GRR_HasMinoPosChanged(_GLP_minoBuffer, currMinoPos))
            {
                GRR_HideMino(currMinoPos);
                GRR_ShowMino(_GLP_minoBuffer, CurrMinoType);
                CopyMino(_GLP_minoBuffer, currMinoPos);
            }
            return actions;
        }


        private void _GLP_ProcessGameLogic(int actions)
        {
            if (ACM_HasAction(actions, PlayerAction.FirstHold))
            {
                _GLP_ExecuteHold(true);
            }
            else if (ACM_HasAction(actions, PlayerAction.Hold))
            {
                _GLP_ExecuteHold(false);
            }
            else if (ACM_HasAction(actions, PlayerAction.HardDrop))
            {
                _GLP_LockDown();
            }
            else
            {
                _GLP_Drop();
                if (LDC_NeedsLockDown(currMinoPos))
                {
                    _GLP_LockDown();
                }
            }
        }


        private void _GLP_ExecuteHold(bool isFirstHold)
        {
            GRR_HideMino(currMinoPos);
            PVR_ShowHoldMino(HLR_HoldMinoType);
            if (isFirstHold)
            {
                CurrMinoType = SPN_SpawnMino(currMinoPos);
            }
            else
            {
                SPN_SetMino(currMinoPos, CurrMinoType);
            }
            GRR_ShowMino(currMinoPos, CurrMinoType);
            CurrMinoAngle = 0;
            HLR_CanHold = false;
            DRS_Tspn = TSpinState.None;
            LDC_Reset();
        }


        private void _GLP_Drop()
        {
            CopyMino(currMinoPos, _GLP_minoBuffer);

            bool hasDropped = DRP_Drop(_GLP_minoBuffer);
            if (hasDropped)
            {
                DRS_Tspn = TSpinState.None;

                LDC_UpdateByDrop(_GLP_minoBuffer);

                GRR_HideMino(currMinoPos);
                GRR_ShowMino(_GLP_minoBuffer, CurrMinoType);
                CopyMino(_GLP_minoBuffer, currMinoPos);
            }
        }


        private void _GLP_LockDown()
        {
            DRS_AddScoreDelta();

            GRM_SaveMino(currMinoPos, CurrMinoType);
            CurrMinoType = MinoType.None;
            int completeLines = GRM_CompleteLines(currMinoPos);
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
                if (GRD_IsGameOver())
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
            CurrGameState = GameState.GameOver;

            UIM_OnGameOver();
            STM_OnGameOver();

            PlayId++;
            _GLP_UpdateChips(STT_Score);

            _updateHandler.enabled = false;

            if (Utilities.IsValid(_manager))
            {
                Log($"GameLoop", $"{nameof(_manager.OnGameFinished)}");
                _manager.OnGameFinished(STT_Score, STT_Level, 1);
            }
            else
            {
                CurrGameState = GameState.Resettable;
                UIM_OnResettable();
            }

            SYS_RequestSync();
        }


        private void _GLP_SpawnMino()
        {
            CurrMinoType = SPN_SpawnMino(currMinoPos);
            GRR_ShowMino(currMinoPos, CurrMinoType);
            CurrMinoAngle = 0;
            HLR_CanHold = true;
            LDC_Reset();
        }


        private void _GLP_ProcessInAnimation()
        {
            // 一部のアクションのみ可能
            ACM_ResolveActionsWhileAnimating();
            // アニメーションが完了フェーズならミノを生成し、次のフレームから通常処理に戻す
            if (GRA_IsAnimationCompleted())
            {
                GRA_FinishAnimation();

                // 消去したうえでゲームオーバーかどうかを判定
                if (GRD_IsGameOver())
                {
                    DRS_AddScoreDelta();
                    GLP_OnGameOver();
                    return;
                }

                _GLP_SpawnMino();
                UIM_Update();
            }
        }




        private void _GLP_UpdateChips(uint score)
        {
            if (_udonChipsRate < 0.0f)
            {
                _udonChipsRate = 0.1f;
            }

            if (Utilities.IsValid(_udonChips))
            {
                const string fieldName = "money";
                float oldValue = (float)_udonChips.GetProgramVariable(fieldName);
                float newValue = oldValue + score * _udonChipsRate;
                _udonChips.SetProgramVariable(fieldName, newValue);
            }
        }




        // デバッグ用
        private void GLP_DebugLevel()
        {
            if (CurrGameState != GameState.Playing) { return; }
            STT_Level = 19;
            STT_Line = 199;
        }
    }
}
