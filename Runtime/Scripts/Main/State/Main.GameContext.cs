
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        private uint _playId = 0;
        private uint PlayId
        {
            get => _playId;
            set
            {
                if (value == uint.MaxValue) { value = 0; }
                _playId = value;
            }
        }
        private string PlayerName { get; set; } = string.Empty;
        public GameState CurrGameState { get; set;} = GameState.Title;




        private void GCT_Reset()
        {
            PlayerName = string.Empty;
        }


        private void GCT_Start()
        {
            PlayerName = Networking.GetOwner(this.gameObject).displayName;
        }
    }
}