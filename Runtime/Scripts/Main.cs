
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public partial class Main : UdonSharpBehaviour
    {
        private Manager _manager;




        private void OnEnable()
        {
            if (CurrGameState == GameState.Title)
            {
                GLP_Reset();
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


        public uint GetPlayID()
        {
            return PlayId;
        }


        public void GetGameStats(out string playerName, out uint level, out uint score, out uint count, out ushort line, out ushort comb, out ushort tspn, out ushort btob, out ushort perf)
        {
            playerName = PlayerName;
            level = STT_Level;
            score = STT_Score;
            count = 1;
            line = STT_Line;
            comb = STT_Comb;
            tspn = STT_Tspn;
            btob = STT_Btob;
            perf = STT_Perf;
        }


        public void OnStatsBoardUpdated()
        {
            if (!Networking.IsOwner(this.gameObject)) { return; }

            Log("Hub", $"{nameof(OnStatsBoardUpdated)}");

            if (CurrGameState == GameState.GameOver)
            {
                CurrGameState = GameState.Resettable;
                UIM_OnResettable();

                SYS_RequestSync();
            }
        }


        public void Subscribe(Manager manager)
        {
            _manager = manager;
        }
    }
}
