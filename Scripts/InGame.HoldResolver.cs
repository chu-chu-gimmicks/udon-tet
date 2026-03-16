
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private MinoType HLR_HoldMinoType { get; set; } = MinoType.None;

        private ButtonState _HLR_lastInputL = ButtonState.Released;
        private ButtonState _HLR_lastInputR = ButtonState.Released;

        private Vector2Int[] _HLR_minoBuffer = new Vector2Int[4];




        private void HLR_Reset()
        {
            HLR_HoldMinoType = MinoType.None;
            _HLR_lastInputL = ButtonState.Released;
            _HLR_lastInputR = ButtonState.Released;
        }


        private bool HLR_ResolveHold(Vector2Int[] minoPos, out bool isFirstHold)
        {
            isFirstHold = false;

            if (!CanHold) { return false; }
            if (!_HLR_NeedsHold()) { return false; }

            CopyMino(minoPos, _HLR_minoBuffer);

            if (HLR_HoldMinoType == MinoType.None)
            {
                HLR_HoldMinoType = CurrentMinoType;
                isFirstHold = true;
            }
            else
            {
                MinoType tmp = HLR_HoldMinoType;
                HLR_HoldMinoType = CurrentMinoType;
                CurrentMinoType = tmp;
            }
            return true;
        }


        private bool _HLR_NeedsHold()
        {
            ButtonState inputStateL = InputStateGrabL;
            ButtonState inputStateR = InputStateGrabR;
            bool justPressed = (inputStateL != ButtonState.Released && _HLR_lastInputL == ButtonState.Released) || (inputStateR != ButtonState.Released && _HLR_lastInputR == ButtonState.Released);
            _HLR_lastInputL = inputStateL;
            _HLR_lastInputR = inputStateR;
            return justPressed;
        }
    }
}