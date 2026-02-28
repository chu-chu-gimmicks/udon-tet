
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private int[] _GM_completeHeights = new int[4];




        private void GM_Reset()
        {
            for (int i = 0; i < _GM_completeHeights.Length; i++)
            {
                _GM_completeHeights[i] = -1;
            }
        }


        private bool _GM_IsOccupied(Vector2Int pos)
        {
            if (G_IsOutOfXBounds(pos) || G_IsBelowYBounds(pos) || G_IsOverlapped(pos))
            {
                return true;
            }
            return false;
        }


        private void GM_SaveMino(Vector2Int[] minoPos, MinoType minoType)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                int idx = GetIndex(minoPos[i].x, minoPos[i].y);
                G_grid[idx] = minoType;
            }

            DR_Block += (byte)minoPos.Length;
        }


        // 行がそろっているか判定
        private byte GM_CompleteLines(Vector2Int[] minoPos)
        {
            int minHeight = int.MaxValue;
            int maxHeight = int.MinValue;

            byte count = 0;

            // ミノのy座標の最小値と最大値を調べる
            for (int i = 0; i < minoPos.Length; i++)
            {
                minHeight = Mathf.Min(minHeight, minoPos[i].y);
                maxHeight = Mathf.Max(maxHeight, minoPos[i].y);
            }

            // ミノのy座標の最小値と最大値の間で、行がそろっているか調べる
            for (int y = minHeight; y <= maxHeight; y++)
            {
                bool isLineComplete = true;

                for (int x = 0; x < WIDTH; x++)
                {
                    int idx = GetIndex(x, y);
                    if (G_grid[idx] == MinoType.None)
                    {
                        isLineComplete = false;
                        break;
                    }
                }

                if (isLineComplete)
                {
                    _GM_completeHeights[count] = y;
                    count++;
                    Debug.Log($"COMPLETED : {_GM_completeHeights[count - 1]}");
                }
            }

            // アニメーション用のクラスに配列を渡しておく
            GA_CopyCompleteHeights(_GM_completeHeights);

            // TSpin/Perfect 以外はここで算出
            if (count > 0)
            {
                DR_Line = count;
                DR_Combo++;

                if (count == 4 || DR_TSpin != TSpinState.None)
                {
                    DR_BTB++;
                }
                else
                {
                    DR_BTB = 0;
                }
            }
            else
            {
                DR_Line = 0;
                DR_Combo = 0;
                DR_TSpin = TSpinState.None;
            }

            return count;
        }


        private void GM_ClearLines()
        {
            for (int i = 0; i < _GM_completeHeights.Length; i++)
            {
                int y = _GM_completeHeights[i];
                if (y == -1) { continue; }

                for (int x = 0; x < WIDTH; x++)
                {
                    byte idx = GetIndex(x, y);
                    G_grid[idx] = MinoType.None;

                    DR_Block--;
                }
            }
        }


        private void GM_ShiftLinesDown()
        {
            int minHeight = _GM_completeHeights[0];
            int yDest = minHeight;

            for (int ySrc = minHeight; ySrc < HEIGHT; ySrc++)
            {
                // ySrc が消える行かどうかチェック
                bool isComplete = false;
                for (int i = 0; i < _GM_completeHeights.Length; i++)
                {
                    if (ySrc == _GM_completeHeights[i])
                    {
                        isComplete = true;
                        break;
                    }
                }

                // ySrc が消える行なら、コピーできないのでスキップ
                if (isComplete) { continue;}

                // ySrc を yDest にコピー
                for (int x = 0; x < WIDTH; x++)
                {
                    byte idxSrc = GetIndex(x, ySrc);
                    byte idxDest = GetIndex(x, yDest);
                    G_grid[idxDest] = G_grid[idxSrc];
                }

                yDest++;
            }

            // 残りの行を空にする
            for (int y = yDest; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    byte idxDest = GetIndex(x, y);
                    G_grid[idxDest] = MinoType.None;
                }
            }

            // 配列をリセット
            for (int i = 0; i < _GM_completeHeights.Length; i++)
            {
                _GM_completeHeights[i] = -1;
            }
        }
    }
}
