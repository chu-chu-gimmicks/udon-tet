
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public enum _SoftDropActive
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


        private void SDR_ResolveSoftDrop()
        {
            if (!_SDR_HasChangedSoftDropState(out _SoftDropActive softDropState)) { return; }
            if (!GS_CanReflectSoftDrop()) { return; }

            if (softDropState == _SoftDropActive.Active)
            {
                D_interval = Mathf.Min(D_interval, _SDR_SOFTDROP_INTERVAL);
                D_timer += D_interval;
            }
            else if (softDropState == _SoftDropActive.Inactive)
            {
                D_UpdateDropSpeed();
            }
        }


        private bool _SDR_HasChangedSoftDropState(out _SoftDropActive softDropState)
        {
            softDropState = _SoftDropActive.Inactive;

            AxisState inputState = LVInputState;
            if (inputState == AxisState.Negative)
            {
                if (_SDR_lastInput != AxisState.Negative)
                {
                    softDropState = _SoftDropActive.Active;
                    _SDR_lastInput = AxisState.Negative;
                    return true;
                }
            }
            else
            {
                if (_SDR_lastInput != AxisState.Positive)
                {
                    softDropState = _SoftDropActive.Inactive;
                    _SDR_lastInput = AxisState.Positive;
                    return true;
                }
            }

            return false;
        }
    }
}