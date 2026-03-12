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
        private ButtonState _GDR_lastInputR = ButtonState.Released;




        private void GDR_Reset()
        {
            _GDR_lastInputL = ButtonState.Released;
            _GDR_lastInputR = ButtonState.Released;
        }


        private bool GDR_ResolveGuide()
        {
            if (!_GDR_HasChangedGuideState(out _GuideState guideState)) { return false; }
            if (!GST_IsInGame()) { return false; }

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

            ButtonState inputStateL = InputStateUseL;
            ButtonState inputStateR = InputStateUseR;
            bool isPressedL = inputStateL == ButtonState.Pressed;
            bool isPressedR = inputStateR == ButtonState.Pressed;
            bool isHeldL = inputStateL == ButtonState.Released;
            bool isHeldR = inputStateR == ButtonState.Released;

            if ((isPressedL || isPressedR) && (!isHeldL && !isHeldR))
            {
                guideState = _GuideState.Active;
                return true;
            }
            else if ((!isPressedL && !isPressedR) && (isHeldL || isHeldR))
            {
                guideState = _GuideState.Inactive;
                return true;
            }
            return false;
        }
    }
}
