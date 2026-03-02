
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
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
            if (!GS_CanReflectInput()) { return false; }

            CopyMino(minoPos, _HDR_minoBuffer);
            while (G_CanMoveDown(_HDR_minoBuffer))
            {
                D_MoveDown(_HDR_minoBuffer);
            }
            CopyMino(_HDR_minoBuffer, minoPos);

            return true;
        }


        private bool _HDR_NeedsHardDrop()
        {
            AxisState inputState = LVInputState;
            bool isJustPressed = (inputState == AxisState.Positive && _HDR_lastInput != AxisState.Positive);
            _HDR_lastInput = inputState;
            return isJustPressed;
        }
    }
}