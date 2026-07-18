
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private const int _DRP_THRESHOLD_LEVEL = 23; // interval = 0.05s になるレベル
        private readonly float[] _DRP_INTERVALS =
        {
            1.00f, 0.95f, 0.90f, 0.85f, 0.80f, 0.75f,
            0.70f, 0.65f, 0.60f, 0.55f, 0.50f, 0.45f,
            0.40f, 0.35f, 0.30f, 0.25f, 0.20f, 0.15f,
            0.10f, 0.09f, 0.08f, 0.07f, 0.06f, 0.05f
        };
        private readonly int[] _DRP_GRAVITY_STEPS = { 1, 2, 3, 4, 5, 6, 9, 100 };

        private float DRP_Interval { get; set; } = 1.0f;
        private float DRP_Timer { get; set; } = 0.0f;

        private Vector2Int[] _DRP_minoBuffer = new Vector2Int[4];




        private void DRP_Reset()
        {
            DRP_Interval = _DRP_INTERVALS[0];
            DRP_Timer = 0.0f;
        }


        private bool DRP_Drop(Vector2Int[] minoPos)
        {
            DRP_Timer += Time.deltaTime;

            if (DRP_Timer >= DRP_Interval && GRD_CanMoveDown(minoPos))
            {
                DRP_Timer = 0.0f;
                _DRP_DropByLines(minoPos, STT_Level);
                return true;
            }
            return false;
        }


        private void _DRP_DropByLines(Vector2Int[] minoPos, byte level)
        {
            int index = level > _DRP_THRESHOLD_LEVEL ? level - _DRP_THRESHOLD_LEVEL : 0;
            int lines = _DRP_GRAVITY_STEPS[index];
            Chu_Debug($"Interval={DRP_Interval}, Line={lines}");

            for (int i = 0; i < lines; i++)
            {
                if (GRD_CanMoveDown(minoPos))
                {
                    DRP_MoveDown(minoPos);
                    continue;
                }
                break;
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
            int index = Mathf.Clamp(STT_Level, 0, _DRP_INTERVALS.Length - 1);
            DRP_Interval = _DRP_INTERVALS[index];
        }
    }
}