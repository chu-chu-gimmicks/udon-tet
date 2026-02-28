
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private MinoType[] G_grid = new MinoType[WIDTH * HEIGHT];




        private void G_Reset()
        {
            for (int i = 0; i < G_grid.Length; i++)
            {
                G_grid[i] = MinoType.None;
            }
        }


        private bool G_IsMinoSafe(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                if (G_IsBelowYBounds(minoPos[i])) { return false; }
                if (G_IsOutOfXBounds(minoPos[i])) { return false; }
                if (G_IsOverlapped(minoPos[i])) { return false; }
            }
            return true;
        }


        private bool G_IsMinoOverlapped(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                if (G_IsOverlapped(minoPos[i])) { return true; }
            }
            return false;
        }


        private bool G_IsOverlapped(Vector2Int pos)
        {
            byte idx = GetIndex(pos.x, pos.y);
            if (G_grid[idx] != MinoType.None)
            {
                Debug.Log($"OVERLAPPED");
                return true;
            }
            return false;
        }


        private bool G_IsOutOfXBounds(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= WIDTH)
            {
                Debug.Log("OUT OF X BOUNDS");
                return true;
            }
            return false;
        }


        private bool G_IsBelowYBounds(Vector2Int pos)
        {
            if (pos.y < 0)
            {
                Debug.Log("BELOW OF Y BOUNDS");
                return true;
            }
            return false;
        }


        private bool G_IsGameOver(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                if (_G_IsAboveOfYBounds(minoPos[i]))
                {
                    return true;
                }
            }
            return false;
        }


        private bool _G_IsAboveOfYBounds(Vector2Int pos)
        {
            if (pos.y >= DEADLINE)
            {
                Debug.Log("ABOVE OF Y BOUNDS");
                return true;
            }
            return false;
        }


        private bool G_CanMoveDown(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                if (minoPos[i].y <= 0) { return false; }

                int belowIdx = GetIndex(minoPos[i].x, minoPos[i].y - 1);
                if (G_grid[belowIdx] != MinoType.None)
                {
                    return false;
                }
            }
            return true;
        }
    }
}