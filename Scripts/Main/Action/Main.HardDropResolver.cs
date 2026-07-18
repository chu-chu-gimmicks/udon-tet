
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private AxisState _HDR_lastInput = AxisState.Neutral;

        private Vector2Int[] _HDR_minoBuffer = new Vector2Int[4];




        private void HDR_Reset()
        {
            _HDR_lastInput = AxisState.Neutral;
        }


        private bool HDR_ResolveHardDrop(Vector2Int[] minoPos)
        {
            if (!_HDR_NeedsHardDrop()) { return false; }

            CopyMino(minoPos, _HDR_minoBuffer);
            while (GRD_CanMoveDown(_HDR_minoBuffer))
            {
                DRP_MoveDown(_HDR_minoBuffer);
            }
            CopyMino(_HDR_minoBuffer, minoPos);

            return true;
        }


        private bool _HDR_NeedsHardDrop()
        {
            AxisState input = InputStateLY;
            bool isJustPressed = (input == AxisState.Positive && _HDR_lastInput != AxisState.Positive);
            _HDR_lastInput = input;
            return isJustPressed;
        }
    }
}