
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Chair : UdonSharpBehaviour
    {
        [SerializeField] private InGame inGameManager;

        //[SerializeField] private Transform initialHeadPos;
        //[SerializeField] private Transform enterObj;


        public override void Interact()
        {
            inGameManager.CM_EnterChair();
        }


        //public override void OnStationEntered(VRCPlayerApi player)
        //{
        //    if (player == Networking.LocalPlayer)
        //    {
        //        float headHeight = player.GetTrackingData(TrackingDataType.Head).position.y;
        //        float difference = initialHeadPos.position.y - headHeight;
        //        enterObj.transform.position += new Vector3(0, difference, 0);
        //    }
        //}


        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) { return; }
            if (!inGameManager.CM_IsSitting) { return; }

            inGameManager.CM_IsSitting = false;
        }
    }
}