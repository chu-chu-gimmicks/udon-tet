
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private ButtonState _PAR_lastInput = ButtonState.Released;




        private void PAR_Reset()
        {
            _PAR_lastInput = ButtonState.Released;
        }


        private bool PAR_ResolvePause()
        {
            if (!PAR_NeedsPause()) { return false; }
            if (!GST_IsInGame()) { return false; }

            STM_ExitChair();
            UIM_Pause();
            return true;
        }


        private bool PAR_NeedsPause()
        {
            ButtonState inputState = InputStateJump;
            bool justPressed = inputState == ButtonState.Pressed && _PAR_lastInput == ButtonState.Released;
            _PAR_lastInput = inputState;
            return justPressed;
        }
    }
}