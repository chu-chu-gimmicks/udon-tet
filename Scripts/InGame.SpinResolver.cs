
using UdonSharp;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public enum TSpinState : int
    {
        None = 0,
        Mini = 1,
        Normal = 2
    }


    public partial class InGame
    {
        private readonly Vector2Int[][][][] _SR_srsTable =
        {
            // Clockwise
            new Vector2Int[][][]
            {
                // Normal minoPos
                new Vector2Int[][]
                {
                    new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(-1, -2) }, // 0 -> 270
                    new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },  // 90 -> 0
                    new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) },    // 180 -> 90
                    new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2) }      // 270 -> 180
                },
                // I minoPos
                new Vector2Int[][]
                {
                    new[] { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) },   // 0 -> 270
                    new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(1, -2), new Vector2Int(-2, 1) },   // 90 -> 0
                    new[] { new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2) },   // 180 -> 90
                    new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1) }    // 270 -> 180
                }
            },
            // Not Clockwise
            new Vector2Int[][][]
            {
                // Normal minoPos
                new Vector2Int[][]
                {
                    new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) },    // 0 -> 90
                    new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },  // 90 -> 180
                    new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(-1, -2) }, // 180 -> 270
                    new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2) }      // 270 -> 0
                },
                // I minoPos
                new Vector2Int[][]
                {
                    new[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1) },   // 0 -> 90
                    new[] { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) },   // 90 -> 180
                    new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(1, -2), new Vector2Int(-2, 1) },   // 180 -> 270
                    new[] { new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2) }    // 270 -> 0
                }
            }
        };

        private AxisState _SR_lastInput = AxisState.Neutral;

        private Vector2Int[] _SR_minoBuffer = new Vector2Int[4];




        private void SR_Reset()
        {
            _SR_lastInput = AxisState.Neutral;
        }


        private void SR_ResolveSpin(Vector2Int[] minoPos, out bool hasAppliedSpin)
        {
            hasAppliedSpin = false;

            if (!_SR_NeedsSpin(out bool isClockwise)) { return; }
            if (!GS_CanReflectInput()) { return; }
            if (CurrentMinoType == MinoType.O) { return; }

            CopyMino(minoPos, _SR_minoBuffer);
            _SR_ApplyBaseSpin(CurrentMinoType, minoPos, _SR_minoBuffer, isClockwise);
            bool success = _SR_TryApplySRS(CurrentMinoType, _SR_minoBuffer, isClockwise, Angle);
            if (success)
            {
                hasAppliedSpin = true;
                Angle += isClockwise ? -90 : 90;
                CopyMino(_SR_minoBuffer, minoPos);
            }
        }


        private bool _SR_NeedsSpin(out bool isClockwise)
        {
            isClockwise = false;

            AxisState inputState = UseInputState;

            switch (inputState)
            {
                case AxisState.Negative: isClockwise = false; break;
                case AxisState.Positive: isClockwise = true;  break;
            }

            bool isChanged = (_SR_lastInput != inputState);
            _SR_lastInput = inputState;

            return isChanged && (inputState != AxisState.Neutral);
        }


        private void _SR_ApplyBaseSpin(MinoType minoType, Vector2Int[] source, Vector2Int[] buffer, bool isClockwise)
        {
            // Iミノは他のミノと回転の中心が異なるため、特別に回転処理を行う
            if (minoType == MinoType.I)
            {
                if (isClockwise)
                {
                    int xToAdjust = source[1].x - source[0].x;
                    int yToAdjust = source[1].y - source[0].y;

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        int x = source[i].x - source[0].x;
                        int y = source[i].y - source[0].y;

                        buffer[i] = new Vector2Int(y - xToAdjust + source[0].x, -x - yToAdjust + source[0].y);
                    }
                }
                else
                {
                    int xToAdjust = source[1].x - source[0].x;
                    int yToAdjust = source[1].y - source[0].y;

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        int x = source[i].x - source[0].x;
                        int y = source[i].y - source[0].y;

                        buffer[i] = new Vector2Int(-y - yToAdjust + source[0].x, x + xToAdjust + source[0].y);
                    }
                }
            }
            else
            {
                if (isClockwise)
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        int x = source[i].x - source[0].x;
                        int y = source[i].y - source[0].y;

                        buffer[i] = new Vector2Int(y + source[0].x, -x + source[0].y);
                    }
                }
                else
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        int x = source[i].x - source[0].x;
                        int y = source[i].y - source[0].y;

                        buffer[i] = new Vector2Int(-y + source[0].x, x + source[0].y);
                    }
                }
            }
        }


        private bool _SR_TryApplySRS(MinoType minoType, Vector2Int[] buffer, bool isClockwise, int angle)
        {
            int dirIndex = isClockwise ? 0 : 1;
            int minoTypeIndex = (minoType == MinoType.I) ? 1 : 0;
            int angleIndex = angle / 90;

            Vector2Int[] offsets = _SR_srsTable[dirIndex][minoTypeIndex][angleIndex];

            for (int i = 0; i < offsets.Length; i++)
            {
                // SRSを適用
                for (int j = 0; j < buffer.Length; j++)
                {
                    buffer[j] += offsets[i];
                }

                // 判定
                if (G_IsMinoSafe(buffer))
                {
                    if (minoType == MinoType.T)
                    {
                        bool hasAppliedPointFive = (i == offsets.Length - 1);
                        SR_CheckTSpin(buffer, hasAppliedPointFive);
                    }
                    return true;
                }
                // リセット
                else
                {
                    for (int j = 0; j < buffer.Length; j++)
                    {
                        buffer[j] -= offsets[i];
                    }
                }
            }

            return false;
        }


        private void SR_CheckTSpin(Vector2Int[] minoPos, bool hasAppliedPointFive)
        {
            TSpinState tSpinState = TSpinState.None;
            int point = 0;

            Vector2Int center = minoPos[0];
            Vector2Int convex = minoPos[1];
            Vector2Int dir = convex - center;
            Vector2Int rightDir = new Vector2Int(dir.y, dir.x);

            // 凸側
            // 左上
            if (_GM_IsOccupied(convex - rightDir)) { point += 105; }
            // 右上
            if (_GM_IsOccupied(convex + rightDir)) { point += 105; }

            // 直線側
            // 左下
            if (_GM_IsOccupied(center - dir - rightDir)) { point += 100; }
            // 右下
            if (_GM_IsOccupied(center - dir + rightDir)) { point += 100; }

            Debug.Log($"TSpin Point = {point}");

            // Tスピンミニ
            if (point == 305)
            {
                // SRSの5番目を使っているならTスピンになる
                if (hasAppliedPointFive)
                {
                    tSpinState = TSpinState.Normal;
                }
                else
                {
                    tSpinState = TSpinState.Mini;
                }
            }
            // Tスピン
            else if (point >= 310)
            {
                tSpinState = TSpinState.Normal;
            }

            DR_TSpin = tSpinState;
        }
    }
}