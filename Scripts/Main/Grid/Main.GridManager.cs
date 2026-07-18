
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private int[] _GRM_completeHeights = new int[4];




        private void GRM_Reset()
        {
            for (int i = 0; i < _GRM_completeHeights.Length; i++)
            {
                _GRM_completeHeights[i] = -1;
            }
        }


        private void GRM_SaveMino(Vector2Int[] minoPos, MinoType minoType)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                int idx = GetIndex(minoPos[i].x, minoPos[i].y);
                grid[idx] = minoType;
            }

            DRS_Block += (byte)minoPos.Length;
        }


        // 行がそろっているか判定
        private byte GRM_CompleteLines(Vector2Int[] minoPos)
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
                    if (grid[idx] == MinoType.None)
                    {
                        isLineComplete = false;
                        break;
                    }
                }

                if (isLineComplete)
                {
                    _GRM_completeHeights[count] = y;
                    count++;
                    Debug.Log($"COMPLETED : {_GRM_completeHeights[count - 1]}");
                }
            }

            // アニメーション用のクラスに配列を渡しておく
            GRA_CopyCompleteHeights(_GRM_completeHeights);

            // TSpin/Perfect 以外はここで算出
            if (count > 0)
            {
                DRS_Line = count;
                DRS_Combo++;

                if (count == 4 || DRS_TSpin != TSpinState.None)
                {
                    DRS_BTB++;
                }
                else
                {
                    DRS_BTB = 0;
                }
            }
            else
            {
                DRS_Line = 0;
                DRS_Combo = 0;
                DRS_TSpin = TSpinState.None;
            }

            return count;
        }


        private void GRM_ClearLines()
        {
            for (int i = 0; i < _GRM_completeHeights.Length; i++)
            {
                int y = _GRM_completeHeights[i];
                if (y == -1) { continue; }

                for (int x = 0; x < WIDTH; x++)
                {
                    byte idx = GetIndex(x, y);
                    grid[idx] = MinoType.None;

                    DRS_Block--;
                }
            }
        }


        private void GRM_ShiftLinesDown()
        {
            int minHeight = _GRM_completeHeights[0];
            int yDest = minHeight;

            for (int ySrc = minHeight; ySrc < HEIGHT; ySrc++)
            {
                // ySrc が消える行かどうかチェック
                bool isComplete = false;
                for (int i = 0; i < _GRM_completeHeights.Length; i++)
                {
                    if (ySrc == _GRM_completeHeights[i])
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
                    grid[idxDest] = grid[idxSrc];
                }

                yDest++;
            }

            // 残りの行を空にする
            for (int y = yDest; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    byte idxDest = GetIndex(x, y);
                    grid[idxDest] = MinoType.None;
                }
            }

            // 配列をリセット
            for (int i = 0; i < _GRM_completeHeights.Length; i++)
            {
                _GRM_completeHeights[i] = -1;
            }
        }
    }
}
