
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UserData : UdonSharpBehaviour
    {
        [UdonSynced] private int yourHighScore = 0;
        public int GetYourHighScore()
        {
            return yourHighScore;
        }
        public void SetYourHighScore(int score)
        {
            yourHighScore = score;
            RequestSerialization();
        }


        private bool isRestored = false;
        public bool GetIsRestored() { return isRestored; }




        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (player == Networking.LocalPlayer)
            {
                isRestored = true;
            }
        }
    }
}
