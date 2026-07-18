
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private int PlayId { get; set; } = 0;
        private string PlayerName { get; set; } = string.Empty;
        public GameState CurrGameState { get; set;} = GameState.Title;




        private void GCT_Reset()
        {
            PlayerName = Networking.GetOwner(this.gameObject).displayName;
        }


        private bool GCT_IsPlayingGame()
        {
            if (CurrGameState == GameState.Playing)
            {
                return true;
            }
            return false;
        }
    }
}