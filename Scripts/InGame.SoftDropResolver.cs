
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public enum _SoftDropState
    {
        Inactive,
        Active
    }


    public partial class InGame
    {
        private const float _SDR_SOFTDROP_INTERVAL = 0.1f;

        private AxisState _SDR_lastInput = AxisState.Neutral;




        private void SDR_Reset()
        {
            _SDR_lastInput = AxisState.Neutral;
        }


        private bool SDR_ResolveSoftDrop()
        {
            if (!_SDR_HasChangedSoftDropState(out _SoftDropState softDropState)) { return false; }
            if (!GST_IsInGame()) { return false; }

            if (softDropState == _SoftDropState.Inactive)
            {
                DRP_UpdateInterval();
            }
            else
            {
                DRP_Interval = Mathf.Min(DRP_Interval, _SDR_SOFTDROP_INTERVAL);
                DRP_Timer += DRP_Interval;
            }
            return true;
        }


        private bool _SDR_HasChangedSoftDropState(out _SoftDropState softDropState)
        {
            softDropState = _SoftDropState.Inactive;

            AxisState inputState = InputStateLY;
            if (inputState == AxisState.Negative)
            {
                if (_SDR_lastInput != AxisState.Negative)
                {
                    softDropState = _SoftDropState.Active;
                    _SDR_lastInput = AxisState.Negative;
                    return true;
                }
            }
            else
            {
                if (_SDR_lastInput != AxisState.Positive)
                {
                    softDropState = _SoftDropState.Inactive;
                    _SDR_lastInput = AxisState.Positive;
                    return true;
                }
            }

            return false;
        }
    }
}