
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public partial class Main : UdonSharpBehaviour
    {
        #region Debug
        [SerializeField] private TMPro.TextMeshProUGUI debugText;
        private uint debugCount = 0;
        public void Chu_Debug(string text)
        {
            if (!Utilities.IsValid(debugText)) { return; }
            debugText.text = $"{debugCount}: {text}";
            debugCount++;
            if (debugCount == uint.MaxValue) { debugCount = 0; }
        }


        // public override void OnPostSerialization(VRC.Udon.Common.SerializationResult result)
        // {
        //     Chu_Debug($"{result.success}, {result.byteCount} bytes");
        // }
        #endregion




        public int GetPlayID() { return PlayId; }
        public void GetGameStats(out string playerName, out int score, out ushort line, out byte combo, out ushort tSpin, out byte bTB, out ushort perfect)
        {
            playerName = PlayerName;
            score = STT_Score;
            line = STT_Line;
            combo = STT_Combo;
            tSpin = STT_TSpin;
            bTB = STT_BTB;
            perfect = STT_Perfect;
        }




        private void OnEnable()
        {
            if (CurrGameState == GameState.Title)
            {
                GLP_Reset();
            }

            if (Utilities.IsValid(authoring))
            {
                if (Utilities.IsValid(udonChips)) { return; }
                if (!Utilities.IsValid(authoring.udonChips)) { return; }
                udonChips = authoring.udonChips;
                udonChipsRate = authoring.udonChipsRate;
            }
        }


        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }
            if (!STM_IsSitting) { return; }

            STM_IsSitting = false;
            UIM_Pause();
        }


        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            // 自分が新しいオーナーでないなら何もしない
            if (!player.isLocal) { return; }

            // プレイ中だったなら
            if (CurrGameState == GameState.Playing)
            {
                GLP_OnGameOver();
            }
        }
    }
}
