
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private MinoType HR_HoldMinoType { get; set; } = MinoType.None;

        private ButtonState _HR_lastInputL = ButtonState.Released;
        private ButtonState _HR_lastInputR = ButtonState.Released;

        private Vector2Int[] _HR_minoBuffer = new Vector2Int[4];




        private void HR_Reset()
        {
            HR_HoldMinoType = MinoType.None;
            _HR_lastInputL = ButtonState.Released;
            _HR_lastInputR = ButtonState.Released;
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
            ButtonState inputStateL = LGrabInputState;
            ButtonState inputStateR = RGrabInputState;
            bool isJustPressed = (inputStateL != ButtonState.Released && _HR_lastInputL == ButtonState.Released) || (inputStateR != ButtonState.Released && _HR_lastInputR == ButtonState.Released);
            _HR_lastInputL = inputStateL;
            _HR_lastInputR = inputStateR;
            return isJustPressed;
        }
    }
}