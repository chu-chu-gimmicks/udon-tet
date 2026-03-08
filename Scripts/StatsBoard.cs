
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class StatsBoard : UdonSharpBehaviour
    {
        //[SerializeField] private TextMeshProUGUI debugLabel;
        //private int debugCount = 0;
        // public void Chu_Debug(string text)
        // {
        //     if (Utilities.IsValid(debugLabel)) { return; }

        //     debugCount++;
        //     debugLabel.text = $"{debugCount}: {text}";
        // }


        //public override void OnPostSerialization(SerializationResult result)
        //{
        //    Chu_Debug($"{result.byteCount}B Synced");
        //}




        [Header("Max 16")]
        [SerializeField] private Adapter[] adapters;

        [Space(64)]
        [Header("---- Don't Touch ----")]
        [SerializeField] private TMPro.TextMeshProUGUI[] nameLabels;
        [SerializeField] private TMPro.TextMeshProUGUI[] scoreLabels;
        [SerializeField] private TMPro.TextMeshProUGUI[] recordLabels;

        private const float INTERVAL = 2.0f;
        private const string INITIAL_NAME = "--------";

        private bool isPending = false;

        [UdonSynced] private short[] playIds = new short[16];
        [UdonSynced] private string[] playerNames = new string[8] { INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME };
        [UdonSynced] private int[] scores = new int[8];

        [UdonSynced] private ushort mostLines = 0;
        [UdonSynced] private byte mostCombo = 0;
        [UdonSynced] private ushort mostTSpins = 0;
        [UdonSynced] private byte mostBTB = 0;
        [UdonSynced] private ushort mostPerfects = 0;




        void OnEnable()
        {
            if (isPending) { return; }
            SendCustomEventDelayedSeconds(nameof(CheckResult), INTERVAL);
            isPending = true;
        }


        public void CheckResult()
        {
            if (!Utilities.IsValid(adapters) || adapters.Length == 0) { return; }

            // 予約するか判断
            if (this.gameObject.activeInHierarchy)
            {
                SendCustomEventDelayedSeconds(nameof(CheckResult), INTERVAL);
            }
            else
            {
                isPending = false;
                return;
            }

            if (UpdateStats())
            {
                RequestSerialization();
            }
        }


        private bool UpdateStats()
        {
            if (!Networking.IsOwner(this.gameObject)) { return false; }

            bool isChanged = false;

            for (int i = 0; i < adapters.Length; i++)
            {
                if (!Utilities.IsValid(adapters[i])) { continue; }
                if (adapters[i].inGameManager.CurrentGameState == GameState.Playing) { continue; }

                int playId = adapters[i].inGameManager.GetPlayID();
                string playerName = adapters[i].inGameManager.GetPlayerName();
                int score = adapters[i].inGameManager.GetScore();

                // Play ID が更新されていなければスキップ（新しいゲームが始まっていないため）
                if (playIds[i] >= playId) { continue; }

                bool isChangedLeaderboard = UpdateLeaderboard(playerName, score);
                bool isChangedRecords = UpdateRecords(i, playerName);

                if (isChangedLeaderboard || isChangedRecords)
                {
                    isChanged = true;
                }
            }

            return isChanged;
        }


        private bool UpdateLeaderboard(string name, int score)
        {
            if (score <= scores[scores.Length - 1]) { return false; }

            bool isChanged = false;

            int retryIndex = -1;
            for (int i = 0; i < playerNames.Length; i++)
            {
                if (playerNames[i] == name)
                {
                    retryIndex = i;
                    break;
                }
            }

            // 自分のスコアが既にある場合、一旦そのスコアを消したランキングにする
            if (retryIndex >= 0)
            {
                // 自分のスコアが更新されていなければ何もしない
                if (score <= scores[retryIndex]) { return isChanged; }

                for (int i = retryIndex; i < scores.Length - 1; i++)
                {
                    playerNames[i] = playerNames[i + 1];
                    scores[i] = scores[i + 1];
                }
                playerNames[scores.Length - 1] = string.Empty;
                scores[scores.Length - 1] = 0;
            }

            // 新しいスコアを入れる
            for (int i = 0; i < scores.Length; i++)
            {
                // スコアが入る場所を探す
                if (score <= scores[i]) { continue; }

                for (int j = scores.Length - 1; j > i; j--)
                {
                    playerNames[j] = playerNames[j - 1];
                    scores[j] = scores[j - 1];
                }
                playerNames[i] = name;
                scores[i] = score;

                isChanged = true;
                break;
            }

            UpdateLeaderboardUI();
            return isChanged;
        }


        private void UpdateLeaderboardUI()
        {
            for (int i = 0; i < nameLabels.Length; i++)
            {
                nameLabels[i].text = $"{i}.  {playerNames[i]}";
                scoreLabels[i].text = $"{scores[i]}";
            }
        }


        private bool UpdateRecords(int idx, string name)
        {
            if (recordLabels.Length < 5) { return false; }

            bool isChanged = false;

            ushort lines    = adapters[idx].inGameManager.GetLineStat();
            byte   combo    = adapters[idx].inGameManager.GetComboStat();
            ushort tSpins   = adapters[idx].inGameManager.GetTSpinStat();
            byte   bTB      = adapters[idx].inGameManager.GetBTBStat();
            ushort perfects = adapters[idx].inGameManager.GetPerfectStat();

            if (lines > mostLines)
            {
                mostLines = lines;
                isChanged = true;
            }
            if (combo > mostCombo && combo > 1)
            {
                mostCombo = combo;
                isChanged = true;
            }
            if (tSpins > mostTSpins)
            {
                mostTSpins = tSpins;
                isChanged = true;
            }
            if (bTB > mostBTB && bTB > 1)
            {
                mostBTB = bTB;
                isChanged = true;
            }
            if (perfects > mostPerfects)
            {
                mostPerfects = perfects;
                isChanged = true;
            }

            if (isChanged) { UpdateRecordsUI(); }
            return isChanged;
        }


        private void UpdateRecordsUI()
        {
            if (recordLabels == null || recordLabels.Length < 5) { return; }
            recordLabels[0].text = $"{mostLines}";
            recordLabels[1].text = $"{Mathf.Max(mostCombo - 1, 0)}";
            recordLabels[2].text = $"{mostTSpins}";
            recordLabels[3].text = $"{Mathf.Max(mostBTB - 1, 0)}";
            recordLabels[4].text = $"{mostPerfects}";
        }




        public override void OnDeserialization()
        {
            UpdateLeaderboardUI();
            UpdateRecordsUI();
        }
    }
}