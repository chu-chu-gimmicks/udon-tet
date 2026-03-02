
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const float _GA_START_DELAY = 0.5f;
        private const float _GA_CLEANING_INTERVAL = 0.05f;
        private const float _GA_SHIFTDOWN_DELAY = 0.25f;
        private const float _GA_FINISH_DELAY = 0.25f;

        private int _GA_clearCount = 0;

        private int[] _GA_completeHeights = new int[4];




        private void GA_Reset()
        {
            _GA_clearCount = 0;

            for (int i = 0; i < _GA_completeHeights.Length; i++)
            {
                _GA_completeHeights[i] = -1;
            }
        }


        private void GA_CopyCompleteHeights(int[] heights)
        {
            for (int i = 0; i < heights.Length; i++)
            {
                _GA_completeHeights[i] = heights[i];
            }
        }


        public void GA_AnimateClear()
        {
            if (CurrentClearAnimState == ClearAnimationState.Idle)
            {
                CurrentClearAnimState = ClearAnimationState.Cleaning;
                SendCustomEventDelayedSeconds(nameof(GA_AnimateClear), _GA_START_DELAY);
            }
            else if (CurrentClearAnimState == ClearAnimationState.Cleaning)
            {
                bool isFinished = _GA_AnimateClearStep();
                if (isFinished)
                {
                    CurrentClearAnimState = ClearAnimationState.Shifting;
                    SendCustomEventDelayedSeconds(nameof(GA_AnimateClear), _GA_SHIFTDOWN_DELAY);
                }
                else
                {
                    SendCustomEventDelayedSeconds(nameof(GA_AnimateClear), _GA_CLEANING_INTERVAL);
                }
            }
            else if (CurrentClearAnimState == ClearAnimationState.Shifting)
            {
                _GA_AnimateShiftDown();
                SendCustomEventDelayedSeconds(nameof(_GA_FinishAnimation), _GA_FINISH_DELAY);
            }
        }


        public bool _GA_AnimateClearStep()
        {
            // この処理をwidthの分繰り返す
            for (int i = 0; i < _GA_completeHeights.Length; i++)
            {
                int y = _GA_completeHeights[i];
                if (y == -1) { continue; }

                byte idx = GetIndex(_GA_clearCount, y);
                GR_HideBlock(idx);
            }

            _GA_clearCount++;

            if (_GA_clearCount == WIDTH)
            {
                return true;
            }
            return false;
        }


        public void _GA_AnimateShiftDown()
        {
            int minHeight = _GA_completeHeights[0];

            for (int y = minHeight; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    byte idx = GetIndex(x, y);
                    if (G_grid[idx] == MinoType.None)
                    {
                        GR_HideBlock(idx);
                    }
                    else
                    {
                        GR_ShowBlock(idx, G_grid[idx]);
                    }
                }
            }
        }


        public void _GA_FinishAnimation()
        {
            GA_Reset();
            CurrentClearAnimState = ClearAnimationState.Completed;
        }
    }
}