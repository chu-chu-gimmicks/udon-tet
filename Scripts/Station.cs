
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Station : UdonSharpBehaviour
    {
        [SerializeField] private Transform viewHeight;




        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }

            AdjustHeightAutomatically();
        }


        public override void OnStationExited(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }

            this.gameObject.transform.localPosition = Vector3.zero;
        }


        private void AdjustHeightAutomatically()
        {
            float currViewHeight = VRC.SDK3.Rendering.VRCCameraSettings.ScreenCamera.Position.y;
            float targetViewHeight = viewHeight.position.y;
            float diff = targetViewHeight - currViewHeight;
            this.gameObject.transform.position += new Vector3(0, diff, 0);
        }
    }
}