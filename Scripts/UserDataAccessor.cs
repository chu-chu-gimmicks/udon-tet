
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

        private bool isRestored = false;




        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) { return; }
            isRestored = true;
            EnsureAccessToUserData();
        }


        public int GetYourHighScore()
        {
            if (!EnsureAccessToUserData()) { return 0; }
            return userData.YourHighScore;
        }


        public void SetYourHighScore(int value)
        {
            if (!EnsureAccessToUserData()) { return; }
            userData.YourHighScore = value;
        }


        private bool EnsureAccessToUserData()
        {
            if (!isRestored) { return false; }

            if (Utilities.IsValid(userData)) { return true; }
            
            userData = (ChuChuGimmicks.UDONTET.UserData)Networking.FindComponentInPlayerObjects(Networking.LocalPlayer, referenceUserData);

            if (Utilities.IsValid(userData))
            {
                return true;
            }
            return false;
        }
    }
}
