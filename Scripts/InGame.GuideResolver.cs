
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public enum _GuideState
    {
        Inactive,
        Active
    }


    public partial class InGame
    {
        private ButtonState _GDR_lastInputL = ButtonState.Released;
        private ButtonState _GDR_lastInput = ButtonState.Released;




        private void GDR_Reset()
        {
            _GDR_lastInputL = ButtonState.Released;
            _GDR_lastInput = ButtonState.Released;
        }


        private bool GDR_ResolveGuide()
        {
            if (!_GDR_HasChangedGuideState(out _GuideState guideState)) { return false; }

            if (guideState == _GuideState.Active)
            {
                UIM_ShowGuide();
            }
            else
            {
                UIM_HideGuide();
            }

            return true;
        }


        private bool _GDR_HasChangedGuideState(out _GuideState guideState)
        {
            guideState = _GuideState.Inactive;

            ButtonState inputState = InputStateUseR;
            bool isPressed = inputState == ButtonState.Pressed;
            bool isHeld = _GDR_lastInput == ButtonState.Pressed;
            _GDR_lastInput = inputState;

            if (isPressed && !isHeld)
            {
                guideState = _GuideState.Active;
                return true;
            }
            else if (!isPressed && isHeld)
            {
                guideState = _GuideState.Inactive;
                return true;
            }
            return false;
        }
    }
}
