
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
        public int YourHighScore
        {
            get { return yourHighScore; }
            set
            {
                if (value <= yourHighScore) { return; }
                yourHighScore = value;
                RequestSerialization();
            }
        }
    }
}
