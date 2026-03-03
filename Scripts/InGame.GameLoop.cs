
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

        private Vector2Int[] _GL_minoBuffer = new Vector2Int[4];




        private void GL_UpdateGameState()
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
                GL_Reset();
            }
            else if (CurrentGameState == GameState.Title)
            {
                CurrentGameState = GameState.Playing;
                _GL_Start();
            }

            SYS_RequestSync();
        }


        private void GL_Reset()
        {
            D_Reset();
            LC_Reset();
            GM_Reset();
            GA_Reset();
            G_Reset();
            GR_Reset();

            S_Reset();
            PR_Reset();

            IR_Reset();
            MR_Reset();
            SR_Reset();
            SDR_Reset();
            HDR_Reset();
            HR_Reset();
            CAR_Reset();

            UIR_Reset();

            CM_Reset();

            SYD_Reset();
            SYS_Reset();
            SYR_Reset();

            updateHandler.enabled = false;
            rangeCollider.enabled = false;
        }


        private void _GL_Start()
        {
            // ランキング反映までの猶予を稼ぐため、ここで初期化する
            // 演出で数秒稼ぐ、ボタンを数秒後に表示等もあり
            GS_Reset();
            SC_Reset();
            DR_Reset();
            ST_Reset();

            _GL_SpawnMino();
            UI_Start();

            updateHandler.enabled = true;
            rangeCollider.enabled = false;

            SYS_RequestPeriodicSync();
        }


        public void GL_Update()
        {
            IR_Update();
        }


        public void GL_LateUpdate()
        {
            if (!Networking.IsOwner(this.gameObject)) { return; }
            if (CurrentGameState != GameState.Playing) { return; }
            if (CurrentClearAnimState != ClearAnimationState.Idle)
            {
                if (CurrentClearAnimState == ClearAnimationState.Completed)
                {
                    _GL_SpawnAfterAnimation();
                }
                return;
            }

            CopyMino(currentMinoPos, _GL_minoBuffer);

            int actions = AM_ResolvedActions();

            if ((actions & (int)PlayerAction.FirstHold) != 0)
            {
                _GL_SpawnMino();
            }
            else if ((actions & (int)PlayerAction.Hold) != 0)
            {
                _GL_SpawnBySubsequentHold();
            }
            else if ((actions & (int)PlayerAction.HardDrop) != 0)
            {
                _GL_LockDown();
            }
            else
            {
                _GL_Drop();
                if (LC_NeedsLockDown(currentMinoPos))
                {
                    _GL_LockDown();
                }
            }
        }


        private void _GL_Drop()
        {
            D_Drop(currentMinoPos, out bool hasDropped);

            if (hasDropped)
            {
                DR_TSpin = TSpinState.None;

                LC_UpdateByDrop(currentMinoPos);

                GR_HideMino(_GL_minoBuffer);
                GR_ShowMino(currentMinoPos, CurrentMinoType);
                CopyMino(currentMinoPos, _GL_minoBuffer);
            }
        }


        private void _GL_LockDown()
        {
            SC_AddScoreDelta();

            GM_SaveMino(currentMinoPos, CurrentMinoType);
            int completeLines = GM_CompleteLines(currentMinoPos);
            if (completeLines > 0)
            {
                GM_ClearLines();
                GM_ShiftLinesDown();

                ST_UpdateLevel(out bool hasLeveledUp);
                if (hasLeveledUp)
                {
                    D_UpdateDropSpeed();
                }

                SC_CalculateScoreDelta();

                GA_AnimateClear();
            }
            else
            {
                if (G_IsGameOver(currentMinoPos))
                {
                    GL_OnGameOver();
                    return;
                }

                _GL_SpawnMino();
                UI_Update();
            }
        }


        private void GL_OnGameOver()
        {
            CurrentGameState = GameState.GameOver;

            UI_OnGameOver();
            CM_OnGameOver();

            PlayId++;
            UpdateUCS(ST_Score);

            updateHandler.enabled = false;
            rangeCollider.enabled = false;

            SYS_RequestSync();
        }


        private void _GL_SpawnMino()
        {
            CurrentMinoType = S_SpawnMino(currentMinoPos);
            _GL_InitializeWhenSpawn();
        }


        private  void _GL_SpawnBySubsequentHold()
        {
            S_SetMino(currentMinoPos, CurrentMinoType);
            _GL_InitializeWhenSpawn(false);
        }


        private void _GL_InitializeWhenSpawn(bool canHold = true)
        {
            GR_ShowMino(currentMinoPos, CurrentMinoType);
            Angle = 0;
            CanHold = canHold;
            DR_TSpin = TSpinState.None;
            LC_Reset();
        }


        private void _GL_SpawnAfterAnimation()
        {
            CurrentClearAnimState = ClearAnimationState.Idle;

            if (G_IsGameOver(currentMinoPos))
            {
                GL_OnGameOver();
                return;
            }

            _GL_SpawnMino();
            UI_Update();
        }




        // デバッグ用
        private void GL_DebugLevel()
        {
            if (CurrentGameState != GameState.Playing) { return; }
            ST_Level = 19;
            ST_Line = 199;
        }
    }
}
