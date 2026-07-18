
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


    public partial class Main
    {
        private _GuideState guideState = _GuideState.Inactive;

        private ButtonState _GDR_lastInputL = ButtonState.Released;
        private ButtonState _GDR_lastInput = ButtonState.Released;




        private void GDR_Reset()
        {
            guideState = _GuideState.Inactive;

            _GDR_lastInputL = ButtonState.Released;
            _GDR_lastInput = ButtonState.Released;
        }


        private bool GDR_ResolveGuide()
        {
            if (!_GDR_HasChangedGuideState()) { return false; }

            if (guideState == _GuideState.Active)
            {
                //UIM_ShowGuide();
            }
            else
            {
                //UIM_HideGuide();
            }

            return true;
        }


        private bool _GDR_HasChangedGuideState()
        {
            ButtonState input = InputStateUseR;
            bool justPressed = (input == ButtonState.Pressed && _GDR_lastInput != ButtonState.Pressed);
            _GDR_lastInput = input;
            if (!justPressed) { return false; }

            bool isGuideActive = (guideState == _GuideState.Active);
            guideState = isGuideActive ? _GuideState.Inactive : _GuideState.Active;
            return true;
        }
    }
}
