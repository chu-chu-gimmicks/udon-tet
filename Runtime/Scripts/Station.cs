
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Station : UdonSharpBehaviour
    {
        [SerializeField] private Transform _viewHeight;
        [SerializeField] private Transform _exitPoint;




        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }

            AdjustHeightAutomatically();
        }


        public override void OnStationExited(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }

            this.gameObject.transform.localPosition = Vector3.zero;

            if (Physics.Raycast(this.transform.position, -transform.up, out RaycastHit hit, 4.0f, ~0, QueryTriggerInteraction.Ignore))
            {
                Vector3 target = _exitPoint.position;
                target.y = hit.point.y;
                _exitPoint.position = target;
            }
        }


        private void AdjustHeightAutomatically()
        {
            float currViewHeight = VRC.SDK3.Rendering.VRCCameraSettings.ScreenCamera.Position.y;
            float targetViewHeight = _viewHeight.position.y;
            float diff = targetViewHeight - currViewHeight;
            this.gameObject.transform.position += new Vector3(0, diff, 0);
        }
    }
}