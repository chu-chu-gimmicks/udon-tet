
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const float _GRA_START_DELAY = 0.5f;
        private const float _GRA_CLEANING_INTERVAL = 0.05f;
        private const float _GRA_SHIFTDOWN_DELAY = 0.25f;
        private const float _GRA_FINISH_DELAY = 0.25f;

        private int _GRA_clearCount = 0;

        private int[] _GRA_completeHeights = new int[4];




        private void GRA_Reset()
        {
            _GRA_clearCount = 0;

            for (int i = 0; i < _GRA_completeHeights.Length; i++)
            {
                _GRA_completeHeights[i] = -1;
            }
        }


        private void GRA_CopyCompleteHeights(int[] heights)
        {
            for (int i = 0; i < heights.Length; i++)
            {
                _GRA_completeHeights[i] = heights[i];
            }
        }


        public void GRA_AnimateClear()
        {
            if (CurrentClearAnimState == ClearAnimationState.Idle)
            {
                CurrentClearAnimState = ClearAnimationState.Cleaning;
                SendCustomEventDelayedSeconds(nameof(GRA_AnimateClear), _GRA_START_DELAY);
            }
            else if (CurrentClearAnimState == ClearAnimationState.Cleaning)
            {
                bool isFinished = _GRA_AnimateClearStep();
                if (isFinished)
                {
                    CurrentClearAnimState = ClearAnimationState.Shifting;
                    SendCustomEventDelayedSeconds(nameof(GRA_AnimateClear), _GRA_SHIFTDOWN_DELAY);
                }
                else
                {
                    SendCustomEventDelayedSeconds(nameof(GRA_AnimateClear), _GRA_CLEANING_INTERVAL);
                }
            }
            else if (CurrentClearAnimState == ClearAnimationState.Shifting)
            {
                _GRA_AnimateShiftDown();
                SendCustomEventDelayedSeconds(nameof(_GRA_FinishAnimation), _GRA_FINISH_DELAY);
            }
        }


        public bool _GRA_AnimateClearStep()
        {
            // この処理をwidthの分繰り返す
            for (int i = 0; i < _GRA_completeHeights.Length; i++)
            {
                int y = _GRA_completeHeights[i];
                if (y == -1) { continue; }

                byte idx = GetIndex(_GRA_clearCount, y);
                GRR_HideBlock(idx);
            }

            _GRA_clearCount++;

            if (_GRA_clearCount == WIDTH)
            {
                return true;
            }
            return false;
        }


        public void _GRA_AnimateShiftDown()
        {
            int minHeight = _GRA_completeHeights[0];

            for (int y = minHeight; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    byte idx = GetIndex(x, y);
                    if (grid[idx] == MinoType.None)
                    {
                        GRR_HideBlock(idx);
                    }
                    else
                    {
                        GRR_ShowBlock(idx, grid[idx]);
                    }
                }
            }
        }


        public void _GRA_FinishAnimation()
        {
            GRA_Reset();
            CurrentClearAnimState = ClearAnimationState.Completed;
        }
    }
}