
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private MinoType HLR_HoldMinoType { get; set; } = MinoType.None;

        private bool HLR_CanHold { get; set; } = true;

        private ButtonState _HLR_lastInputL = ButtonState.Released;
        private ButtonState _HLR_lastInputR = ButtonState.Released;

        private Vector2Int[] _HLR_minoBuffer = new Vector2Int[4];




        private void HLR_Reset()
        {
            HLR_HoldMinoType = MinoType.None;
            HLR_CanHold = true;
            _HLR_lastInputL = ButtonState.Released;
            _HLR_lastInputR = ButtonState.Released;
        }


        private bool HLR_ResolveHold(Vector2Int[] minoPos, out bool isFirstHold)
        {
            isFirstHold = false;

            if (!HLR_CanHold) { return false; }
            if (!_HLR_NeedsHold()) { return false; }

            CopyMino(minoPos, _HLR_minoBuffer);

            if (HLR_HoldMinoType == MinoType.None)
            {
                HLR_HoldMinoType = CurrMinoType;
                isFirstHold = true;
            }
            else
            {
                MinoType tmp = HLR_HoldMinoType;
                HLR_HoldMinoType = CurrMinoType;
                CurrMinoType = tmp;
            }
            return true;
        }


        private bool _HLR_NeedsHold()
        {
            ButtonState inputL = InputStateGrabL;
            ButtonState inputR = InputStateGrabR;
            bool justPressed = (inputL != ButtonState.Released && _HLR_lastInputL == ButtonState.Released) || (inputR != ButtonState.Released && _HLR_lastInputR == ButtonState.Released);
            _HLR_lastInputL = inputL;
            _HLR_lastInputR = inputR;
            return justPressed;
        }
    }
}