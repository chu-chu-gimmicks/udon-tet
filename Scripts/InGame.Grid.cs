
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private MinoType[] grid = new MinoType[WIDTH * HEIGHT];




        private void GRD_Reset()
        {
            for (int i = 0; i < grid.Length; i++)
            {
                grid[i] = MinoType.None;
            }
        }


        private bool GRD_IsMinoSafe(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                if (_GRD_IsBelowYBounds(minoPos[i])) { return false; }
                if (_GRD_IsOutOfXBounds(minoPos[i])) { return false; }
                if (_GRD_IsOverlapped(minoPos[i]))   { return false; }
            }
            return true;
        }


        private bool GRD_IsOccupied(Vector2Int pos)
        {
            if (_GRD_IsOutOfXBounds(pos)) { return true; }
            if (_GRD_IsBelowYBounds(pos)) { return true; }
            if (_GRD_IsOverlapped(pos))   { return true; }
            return false;
        }


        private bool GRD_IsMinoOverlapped(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                if (_GRD_IsOverlapped(minoPos[i])) { return true; }
            }
            return false;
        }


        private bool _GRD_IsOverlapped(Vector2Int pos)
        {
            byte idx = GetIndex(pos.x, pos.y);
            if (grid[idx] != MinoType.None)
            {
                Debug.Log($"OVERLAPPED");
                return true;
            }
            return false;
        }


        private bool _GRD_IsOutOfXBounds(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= WIDTH)
            {
                Debug.Log("OUT OF X BOUNDS");
                return true;
            }
            return false;
        }


        private bool _GRD_IsBelowYBounds(Vector2Int pos)
        {
            if (pos.y < 0)
            {
                Debug.Log("BELOW OF Y BOUNDS");
                return true;
            }
            return false;
        }


        private bool GRD_IsGameOver(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                if (_GRD_IsAboveOfYBounds(minoPos[i]))
                {
                    return true;
                }
            }
            return false;
        }


        private bool _GRD_IsAboveOfYBounds(Vector2Int pos)
        {
            if (pos.y >= DEADLINE)
            {
                Debug.Log("ABOVE OF Y BOUNDS");
                return true;
            }
            return false;
        }


        private bool GRD_CanMoveDown(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                if (minoPos[i].y <= 0) { return false; }

                int belowIdx = GetIndex(minoPos[i].x, minoPos[i].y - 1);
                if (grid[belowIdx] != MinoType.None)
                {
                    return false;
                }
            }
            return true;
        }
    }
}