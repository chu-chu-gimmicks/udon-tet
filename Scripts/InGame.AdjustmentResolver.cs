
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const float _ADR_SPEED_VR = 0.5f;
        private const float _ADR_SPEED_DESKTOP = 0.02f;




        private void ADR_Reset()
        {
        }


        private bool ADR_ResolveAdjustment()
        {
            if (!ADR_NeedsAdjustment(out Vector3 dir)) { return false; }

            float speed = Networking.LocalPlayer.IsUserInVR() ? _ADR_SPEED_VR * Time.deltaTime : _ADR_SPEED_DESKTOP;
            float newHeight = station.transform.localPosition.y + dir.y * speed;

            if ((dir.y > 0 && newHeight > stationUpperLimit.localPosition.y) ||
                (dir.y < 0 && newHeight < stationLowerLimit.localPosition.y))
            {
                return false;
            }

            station.transform.localPosition = new Vector3(0, newHeight, 0);
            return true;
        }


        private bool ADR_NeedsAdjustment(out Vector3 dir)
        {
            AxisState input = InputStateRY;
            switch (input)
            {
                case AxisState.Negative: dir = Vector3.down; break;
                case AxisState.Positive: dir = Vector3.up;   break;
                default: dir = Vector3.zero; break;
            }
            return dir != Vector3.zero;
        }
    }
}