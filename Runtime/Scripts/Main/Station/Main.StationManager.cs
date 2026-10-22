
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        private bool STM_IsSitting { get; set; } = false;




        private void STM_Reset()
        {
            STM_IsSitting = false;
            _station.gameObject.SetActive(true);
            _station.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }


        public void STM_EnterChair()
        {
            STM_IsSitting = true;
            _station.UseStation(Networking.LocalPlayer);
        }


        private void STM_ExitChair()
        {
            STM_IsSitting = false;
            _station.ExitStation(Networking.LocalPlayer);
        }


        private void STM_OnGameOver()
        {
            STM_ExitChair();
        }


        private void STM_DisableStation()
        {
            _station.gameObject.SetActive(false);
        }
    }
}