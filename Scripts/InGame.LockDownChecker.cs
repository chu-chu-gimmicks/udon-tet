
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const int _LC_MAX_COUNT = 15;
        private const float _LC_INTERVAL = 0.5f;

        private int _LC_Count { get; set; } = -1;
        private float _LC_Timer { get; set; } = 0.0f;

        private int _LC_lastMinHeight { get; set; } = int.MaxValue;




        private void LC_Reset()
        {
            _LC_Count = -1;
            _LC_Timer = 0.0f;
            _LC_lastMinHeight = int.MaxValue;
        }


        private bool LC_NeedsLockDown(Vector2Int[] minoPos)
        {
            _LC_Timer += Time.deltaTime;
            if (_LC_Timer >= _LC_INTERVAL && !G_CanMoveDown(minoPos))
            {
                LC_Reset();
                return true;
            }
            return false;
        }


        private void LC_UpdateByInput(Vector2Int[] minoPos)
        {
            int minHeight = _LC_GetMinHeight(minoPos);
            if (minHeight < _LC_lastMinHeight)
            {
                _LC_Count = -1;
                _LC_lastMinHeight = minHeight;
            }

            if (G_CanMoveDown(minoPos))
            {
                if (_LC_Count >= 0)
                {
                    _LC_Count++;
                    _LC_Timer = 0.0f;
                }
            }
            else
            {
                _LC_Count++;
                _LC_Timer = (_LC_Count >= _LC_MAX_COUNT) ? _LC_INTERVAL + 1.0f : 0.0f;
            }
        }


        private void LC_UpdateByDrop(Vector2Int[] minoPos)
        {
            int minHeight = _LC_GetMinHeight(minoPos);
            if (minHeight < _LC_lastMinHeight)
            {
                _LC_Count = -1;
                _LC_lastMinHeight = minHeight;
            }

            if (!G_CanMoveDown(minoPos))
            {
                if (_LC_Count < 0)
                {
                    _LC_Count = 0;
                    _LC_Timer = 0.0f;
                }
                else if (_LC_Count >= _LC_MAX_COUNT)
                {
                    _LC_Timer = _LC_INTERVAL + 1.0f;
                }
            }
        }


        private int _LC_GetMinHeight(Vector2Int[] minoPos)
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