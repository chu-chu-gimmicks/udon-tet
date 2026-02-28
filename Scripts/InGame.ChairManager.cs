
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        public bool CM_IsSitting { get; private set; } = false;




        private void CM_Reset()
        {
            CM_IsSitting = false;
            chair.SetActive(false);
        }


        public void CM_EnterChair()
        {
            CM_IsSitting = true;
            chair.SetActive(true);
            station.UseStation(Networking.LocalPlayer);
        }


        private void CM_ExitChair(bool disableChair)
        {
            CM_IsSitting = false;
            station.ExitStation(Networking.LocalPlayer);
            if (disableChair) { chair.SetActive(false); }
        }


        private void CM_OnGameOver()
        {
            CM_ExitChair(disableChair: true);
        }


        public void CM_OnRespawnedInPlay()
        {
            CM_IsSitting = false;
        }
    }
}