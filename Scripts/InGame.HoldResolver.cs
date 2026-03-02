
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private MinoType HR_HoldMinoType { get; set; } = MinoType.None;

        private AxisState _HR_lastInput = AxisState.Neutral;

        private Vector2Int[] _HR_minoBuffer = new Vector2Int[4];




        private void HR_Reset()
        {
            HR_HoldMinoType = MinoType.None;
            _HR_lastInput = AxisState.Neutral;
        }


        private bool HR_ResolveHold(Vector2Int[] minoPos, out bool isFirstHold)
        {
            isFirstHold = false;

            if (!CanHold) { return false; }
            if (!_HR_NeedsHold()) { return false; }
            if (!GS_CanReflectInput()) { return false; }

            CopyMino(minoPos, _HR_minoBuffer);

            if (HR_HoldMinoType == MinoType.None)
            {
                HR_HoldMinoType = CurrentMinoType;
                isFirstHold = true;
            }
            else
            {
                MinoType tmp = HR_HoldMinoType;
                HR_HoldMinoType = CurrentMinoType;
                CurrentMinoType = tmp;
            }
            return true;
        }


        private bool _HR_NeedsHold()
        {
            AxisState inputState = GrabInputState;
            bool isJustPressed = (inputState != AxisState.Neutral && _HR_lastInput == AxisState.Neutral);
            _HR_lastInput = inputState;
            return isJustPressed;
        }
    }
}