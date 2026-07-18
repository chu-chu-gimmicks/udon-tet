
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UserDataAccessor : UdonSharpBehaviour
    {
        [SerializeField] private ChuChuGimmicks.UDONTET.UserData referenceUserData;

        private ChuChuGimmicks.UDONTET.UserData userData;
        private bool isDataRestored = false;
        private bool isDataFound = false;




        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }

            EnsureDataFound(true);
        }


        public bool EnsureDataFound(bool isRestoreVerified = false)
        {
            isDataRestored = isDataRestored || isRestoreVerified;
            if (isDataRestored && !isDataFound)
            {
                isDataFound = TryFindUserData();
            }
            return isDataFound;
        }


        private bool TryFindUserData()
        {
            userData = (ChuChuGimmicks.UDONTET.UserData)Networking.FindComponentInPlayerObjects(Networking.LocalPlayer, referenceUserData);
            return Utilities.IsValid(userData);
        }


        public bool TryGetYourHighScore(out int value)
        {
            if (isDataFound)
            {
                value = userData.YourHighScore;
                return true;
            }
            value = 0;
            return false;
        }


        public bool TrySetYourHighScore(int value)
        {
            if (isDataFound)
            {
                userData.YourHighScore = value;
                return true;
            }
            return false;
        }
    }
}
