
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
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
            float heightDelta = dir.y * speed;
            float newHeadHeight = VRC.SDK3.Rendering.VRCCameraSettings.ScreenCamera.Position.y + heightDelta;

            if (false
                || (dir.y > 0 && newHeadHeight > _maxHeight.position.y)
                || (dir.y < 0 && newHeadHeight < _minHeight.position.y)
            ){
                return false;
            }

            Vector3 posDelta = new Vector3(0, heightDelta, 0);
            _station.transform.localPosition += posDelta;
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