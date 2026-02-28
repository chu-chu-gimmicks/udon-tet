
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const float _GA_START_INTERVAL = 0.5f;
        private const float _GA_CLEANING_INTERVAL = 0.05f;
        private const float _GA_SHIFTDOWN_INTERVAL = 0.25f;
        private const float _GA_FINISH_INTERVAL = 0.5f;

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
            if (CurrentClearState == ClearState.Start)
            {
                CurrentClearState = ClearState.Cleaning;
                SendCustomEventDelayedSeconds(nameof(GA_AnimateClear), _GA_START_INTERVAL);
                return;
            }
            if (CurrentClearState == ClearState.Cleaning)
            {
                _GA_AnimateClearStep(out bool isFinished);
                if (isFinished)
                {
                    CurrentClearState = ClearState.ShiftDown;
                    SendCustomEventDelayedSeconds(nameof(GA_AnimateClear), _GA_SHIFTDOWN_INTERVAL);
                }
                else
                {
                    SendCustomEventDelayedSeconds(nameof(_GA_AnimateClearStep), _GA_CLEANING_INTERVAL);
                }
            }
            else if (CurrentClearState == ClearState.ShiftDown)
            {
                _GA_AnimateShiftDown();
                GA_Reset();
                CurrentClearState = ClearState.Finish;
                SendCustomEventDelayedSeconds(nameof(GA_AnimateClear), _GA_FINISH_INTERVAL);
            }
        }


        public void _GA_AnimateClearStep(out bool isFinished)
        {
            isFinished = false;

            // この処理をwidthの分繰り返す
            for (int i = 0; i < _GA_completeHeights.Length; i++)
            {
                int y = _GA_completeHeights[i];
                if (y == -1) { continue; }

                byte idx = GetIndex(_GA_clearCount, y);
                GR_UpdateBlock(idx, MinoType.None);
            }

            _GA_clearCount++;

            if (_GA_clearCount == WIDTH)
            {
                isFinished = true;
            }
        }


        public void _GA_AnimateShiftDown()
        {
            int minHeight = _GA_completeHeights[0];

            for (int y = minHeight; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    byte idx = GetIndex(x, y);
                    GR_UpdateBlock(idx, G_grid[idx]);
                }
            }
        }
    }
}
