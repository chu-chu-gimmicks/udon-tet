
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const int _LDC_MAX_COUNT = 15;
        private const float _LDC_INTERVAL = 0.5f;

        private int _LDC_Count { get; set; } = -1;
        private float _LDC_Timer { get; set; } = 0.0f;

        private int _LDC_lastMinHeight { get; set; } = int.MaxValue;




        private void LDC_Reset()
        {
            _LDC_Count = -1;
            _LDC_Timer = 0.0f;
            _LDC_lastMinHeight = int.MaxValue;
        }


        private bool LDC_NeedsLockDown(Vector2Int[] minoPos)
        {
            _LDC_Timer += Time.deltaTime;
            if (_LDC_Timer >= _LDC_INTERVAL && !GRD_CanMoveDown(minoPos))
            {
                LDC_Reset();
                return true;
            }
            return false;
        }


        private void LDC_UpdateByInput(Vector2Int[] minoPos)
        {
            int minHeight = _LDC_GetMinHeight(minoPos);
            if (minHeight < _LDC_lastMinHeight)
            {
                _LDC_Count = -1;
                _LDC_lastMinHeight = minHeight;
            }

            if (GRD_CanMoveDown(minoPos))
            {
                if (_LDC_Count >= 0)
                {
                    _LDC_Count++;
                    _LDC_Timer = 0.0f;
                }
            }
            else
            {
                _LDC_Count++;
                _LDC_Timer = (_LDC_Count >= _LDC_MAX_COUNT) ? _LDC_INTERVAL + 1.0f : 0.0f;
            }
        }


        private void LDC_UpdateByDrop(Vector2Int[] minoPos)
        {
            int minHeight = _LDC_GetMinHeight(minoPos);
            if (minHeight < _LDC_lastMinHeight)
            {
                _LDC_Count = -1;
                _LDC_lastMinHeight = minHeight;
            }

            if (!GRD_CanMoveDown(minoPos))
            {
                if (_LDC_Count < 0)
                {
                    _LDC_Count = 0;
                    _LDC_Timer = 0.0f;
                }
                else if (_LDC_Count >= _LDC_MAX_COUNT)
                {
                    _LDC_Timer = _LDC_INTERVAL + 1.0f;
                }
            }
        }


        private int _LDC_GetMinHeight(Vector2Int[] minoPos)
        {
            int minHeight = int.MaxValue;
            for (int i = 0; i < minoPos.Length; i++)
            {
                minHeight = Mathf.Min(minHeight, minoPos[i].y);
            }
            return minHeight;
        }
    }
}