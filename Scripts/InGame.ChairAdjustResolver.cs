
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const float _CAR_SPEED_VR = 0.5f;
        private const float _CAR_SPEED_DESKTOP = 0.02f;




        private void CAR_Reset()
        {
        }


        private void CAR_ResolveChairAdjust()
        {
            Vector3 dir;
            AxisState inputState = RVInputState;
            switch (inputState)
            {
                case AxisState.Negative: dir = Vector3.down; break;
                case AxisState.Positive: dir = Vector3.up;   break;
                default: return;
            }

            float speed = Networking.LocalPlayer.IsUserInVR() ? _CAR_SPEED_VR * Time.deltaTime : _CAR_SPEED_DESKTOP;
            float newHeight = station.transform.localPosition.y + dir.y * speed;

            if ((dir.y > 0 && newHeight > chairUpperLimit.localPosition.y) ||
                (dir.y < 0 && newHeight < chairLowerLimit.localPosition.y))
            {
                return;
            }

            station.transform.localPosition = new Vector3(0, newHeight, 0);
        }
    }
}