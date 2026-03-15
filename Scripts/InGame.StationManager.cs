
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private void STM_Reset()
        {
            IsSitting = false;
            station.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }


        public void STM_EnterChair()
        {
            IsSitting = true;
            station.UseStation(Networking.LocalPlayer);
        }


        private void STM_ExitChair()
        {
            IsSitting = false;
            station.ExitStation(Networking.LocalPlayer);
        }


        private void STM_OnGameOver()
        {
            STM_ExitChair();
        }
    }
}