
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
        public int    GetScore()      { return ST_Score; }

        public ushort GetLineStat()    { return ST_Line; }
        public byte   GetComboStat()   { return ST_Combo; }
        public ushort GetTSpinStat()   { return ST_TSpin; }
        public byte   GetBTBStat()     { return ST_BTB; }
        public ushort GetPerfectStat() { return ST_Perfect; }




        private void OnEnable()
        {
            ReflectOnlyInCollider = gameContext.reflectOnlyInCollider;

            UI_HideFake();

            if (CurrentGameState == GameState.Title)
            {
                GL_Reset();
            }
        }


        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            // オーナーがインスタンスから去った後、自分が新しいオーナーなら
            if (player == Networking.LocalPlayer)
            {
                // プレイ中だったなら
                if (CurrentGameState == GameState.Playing)
                {
                    GL_OnGameOver();
                }
            }
        }
    }
}
