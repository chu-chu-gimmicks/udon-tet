
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private readonly int[] gravitySteps = { 1, 2, 3, 4, 5, 6, 9, 100 };
        private const float _D_INITIAL_INTERVAL = 1.0f;

        private float D_interval = _D_INITIAL_INTERVAL;
        private float D_timer = 0.0f;

        private Vector2Int[] _D_minoBuffer = new Vector2Int[4];




        private void D_Reset()
        {
            D_interval = _D_INITIAL_INTERVAL;
            D_timer = 0.0f;
        }


        private void D_Drop(Vector2Int[] minoPos, out bool hasDropped)
        {
            hasDropped = false;

            D_timer += Time.deltaTime;

            // 通常の落下
            if (D_timer >= D_interval && G_CanMoveDown(minoPos))
            {
                hasDropped = true;
                D_timer = 0.0f;
                _D_DropByLines(minoPos, ST_Level,  ST_THRESHOLD_LEVEL);
            }
        }


        private void _D_DropByLines(Vector2Int[] minoPos, byte level, byte threshold)
        {
            int index = Mathf.Clamp(level - threshold + 1, 0, gravitySteps.Length - 1);
            int lines = gravitySteps[index];

            for (int i = 0; i < lines; i++)
            {
                if (G_CanMoveDown(minoPos))
                {
                    D_MoveDown(minoPos);
                }
                else
                {
                    break;
                }
            }
        }


        private void D_MoveDown(Vector2Int[] minoPos)
        {
            Vector2Int down = Vector2Int.down;
            for (int i = 0; i < minoPos.Length; i++)
            {
                minoPos[i] += down;
            }
        }


        private void D_UpdateDropSpeed()
        {
            if (ST_Level < ST_THRESHOLD_LEVEL)
            {
                D_interval = (100.0f - 5.0f * ST_Level) / 100.0f;
            }
            else
            {
                D_interval = 0.05f;
            }
        }
    }
}