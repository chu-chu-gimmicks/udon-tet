
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private readonly int[] _DRP_gravitySteps = { 1, 2, 3, 4, 5, 6, 9, 100 };
        private const float _DRP_INITIAL_INTERVAL = 1.0f;

        private float DRP_Interval { get; set; } = _DRP_INITIAL_INTERVAL;
        private float DRP_Timer { get; set; } = 0.0f;

        private Vector2Int[] _DRP_minoBuffer = new Vector2Int[4];




        private void DRP_Reset()
        {
            DRP_Interval = _DRP_INITIAL_INTERVAL;
            DRP_Timer = 0.0f;
        }


        private void DRP_Drop(Vector2Int[] minoPos, out bool hasDropped)
        {
            hasDropped = false;

            DRP_Timer += Time.deltaTime;

            if (DRP_Timer >= DRP_Interval && GRD_CanMoveDown(minoPos))
            {
                hasDropped = true;
                DRP_Timer = 0.0f;
                _DRP_DropByLines(minoPos, STT_Level,  STT_THRESHOLD_LEVEL);
            }
        }


        private void _DRP_DropByLines(Vector2Int[] minoPos, byte level, byte threshold)
        {
            int index = Mathf.Clamp(level - threshold + 1, 0, _DRP_gravitySteps.Length - 1);
            int lines = _DRP_gravitySteps[index];

            for (int i = 0; i < lines; i++)
            {
                if (GRD_CanMoveDown(minoPos))
                {
                    DRP_MoveDown(minoPos);
                }
                else
                {
                    break;
                }
            }
        }


        private void DRP_MoveDown(Vector2Int[] minoPos)
        {
            Vector2Int down = Vector2Int.down;
            for (int i = 0; i < minoPos.Length; i++)
            {
                minoPos[i] += down;
            }
        }


        private void DRP_UpdateInterval()
        {
            if (STT_Level < STT_THRESHOLD_LEVEL)
            {
                DRP_Interval = (100.0f - 5.0f * STT_Level) / 100.0f;
            }
            else
            {
                DRP_Interval = 0.05f;
            }
        }
    }
}