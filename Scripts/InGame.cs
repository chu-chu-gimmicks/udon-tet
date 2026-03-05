
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public partial class InGame : UdonSharpBehaviour
    {
        #region UCS
        private void UpdateUCS(int newScore)
        {
            //if (Utilities.IsValid(gameContext.udonChips))
            //{
            //    gameContext.SetUdonChips(newScore);
            //}
        }
        #endregion


        #region Debug
        [SerializeField] private TMPro.TextMeshProUGUI debugTMP;
        //private int debugCount = 0;
        //public void Chu_Debug(string text)
        //{
        //    if (debugTMP != null)
        //    {
        //        debugCount++;
        //        debugTMP.text = $"{debugCount}: {text}";
        //    }
        //}


        //public override void OnPostSerialization(SerializationResult result)
        //{
        //    Chu_Debug($"{result.byteCount}B Synced");
        //}
        #endregion




        public int    GetPlayID()     { return PlayId; }
        public string GetPlayerName() { return PlayerName; }
        public int    GetScore()      { return STT_Score; }

        public ushort GetLineStat()    { return STT_Line; }
        public byte   GetComboStat()   { return STT_Combo; }
        public ushort GetTSpinStat()   { return STT_TSpin; }
        public byte   GetBTBStat()     { return STT_BTB; }
        public ushort GetPerfectStat() { return STT_Perfect; }




        private void OnEnable()
        {
            ReflectOnlyInCollider = gameContext.reflectOnlyInCollider;

            UIM_HideFake();

            if (CurrentGameState == GameState.Title)
            {
                GLP_Reset();
            }
        }


        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (player == Networking.LocalPlayer)
            {
                
            }
        }


        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) { return; }
            if (!STM_IsSitting) { return; }

            STM_IsSitting = false;
        }


        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            // オーナーがインスタンスから去った後、自分が新しいオーナーなら
            if (player == Networking.LocalPlayer)
            {
                // プレイ中だったなら
                if (CurrentGameState == GameState.Playing)
                {
                    GLP_OnGameOver();
                }
            }
        }
    }
}
