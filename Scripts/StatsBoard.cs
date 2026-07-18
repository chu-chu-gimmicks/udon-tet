
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class StatsBoard : UdonSharpBehaviour
    {
        #region Debug
        [SerializeField] private TMPro.TextMeshProUGUI debugText;
        private int debugCount = 0;
        public void Chu_Debug(string text)
        {
            if (!Utilities.IsValid(debugText)) { return; }

            debugText.text = $"{debugCount}: {text}";
            debugCount++;
            if (debugCount == int.MaxValue) { debugCount = 0; }
        }


        public override void OnPostSerialization(VRC.Udon.Common.SerializationResult result)
        {
            Chu_Debug($"{result.success}, {result.byteCount} bytes");
        }
        #endregion




        [SerializeField] private UDONTETAuthoring[] authorings;
        [SerializeField] private UserDataAccessor userDataAccessor;

        [SerializeField] private TMPro.TextMeshProUGUI yScoreText;
        [SerializeField] private TMPro.TextMeshProUGUI[] lNameTexts;
        [SerializeField] private TMPro.TextMeshProUGUI[] lScoreTexts;
        [SerializeField] private TMPro.TextMeshProUGUI[] rValueTexts;
        [SerializeField] private TMPro.TextMeshProUGUI[] rNameTexts;

        private const float INTERVAL = 2.0f;
        private const string INITIAL_NAME = "--------";

        private bool isPending = false;

        [UdonSynced] private short[] playIds = new short[16];

        [UdonSynced] private string[] lNames = new string[8] { INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME };
        [UdonSynced] private int[] lScores = new int[8];

        [UdonSynced] private ushort rLine = 0;
        [UdonSynced] private byte rCombo = 0;
        [UdonSynced] private ushort rTSpin = 0;
        [UdonSynced] private byte rBTB = 0;
        [UdonSynced] private ushort rPerfect = 0;
        [UdonSynced] private string[] rNames = new string[5] { INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME, INITIAL_NAME };

        private int yScore = 0;




        private void OnEnable()
        {
            InitializeYourHighScore();

            if (!isPending)
            {
                SendCustomEventDelayedSeconds(nameof(CheckResult), INTERVAL);
                isPending = true;
            }
        }


        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }

            InitializeYourHighScore(true);

            if (!isPending)
            {
                SendCustomEventDelayedSeconds(nameof(CheckResult), INTERVAL);
                isPending = true;
            }
        }


        private void InitializeYourHighScore(bool isRestoreVerified = false)
        {
            yScore = 0;
            if (Utilities.IsValid(userDataAccessor) && userDataAccessor.EnsureDataFound(isRestoreVerified))
            {
                if (userDataAccessor.TryGetYourHighScore(out int value))
                {
                    yScore = value;
                }
            }
            UpdateYourHighScoreUI(yScore);
        }


        public void CheckResult()
        {
            if (!Utilities.IsValid(authorings) || authorings.Length == 0)
            {
                isPending = false;
                return;
            }

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

            for (int i = 0; i < authorings.Length; i++)
            {
                if (!Utilities.IsValid(authorings[i])) { continue; }
                if (authorings[i].main.CurrGameState == GameState.Playing) { continue; }

                // Play ID が更新されていなければスキップ（新しいゲームが始まっていないため）
                int playId = authorings[i].main.GetPlayID();
                if (playIds[i] >= playId) { continue; }

                authorings[i].main.GetGameStats(out string playerName, out int score, out ushort line, out byte combo, out ushort tSpin, out byte bTB, out ushort perfect);
                UpdateYourHighScore(playerName, score);
                bool isChangedLeaderboard = UpdateLeaderboard(playerName, score);
                bool isChangedRecords = UpdateRecords(playerName, line, combo, tSpin, bTB, perfect);

                if (isChangedLeaderboard || isChangedRecords)
                {
                    isChanged = true;
                }
            }
            return isChanged;
        }


        private void UpdateYourHighScore(string name, int score)
        {
            if (name != Networking.LocalPlayer.displayName) { return; }
            if (score < yScore) { return; }

            yScore = score;
            if (Utilities.IsValid(userDataAccessor)) { userDataAccessor.TrySetYourHighScore(yScore); }

            UpdateYourHighScoreUI(yScore);
        }


        private void UpdateYourHighScoreUI(int score)
        {
            yScoreText.text = $"{score}";
        }


        private bool UpdateLeaderboard(string name, int score)
        {
            if (score <= lScores[lScores.Length - 1]) { return false; }

            bool isChanged = false;

            int retryIndex = -1;
            for (int i = 0; i < lNames.Length; i++)
            {
                if (lNames[i] == name)
                {
                    retryIndex = i;
                    break;
                }
            }

            // 自分のスコアが既にある場合、一旦そのスコアを消したランキングにする
            if (retryIndex >= 0)
            {
                // 自分のスコアが更新されていなければ何もしない
                if (score <= lScores[retryIndex]) { return isChanged; }

                for (int i = retryIndex; i < lScores.Length - 1; i++)
                {
                    lNames[i] = lNames[i + 1];
                    lScores[i] = lScores[i + 1];
                }
                lNames[lScores.Length - 1] = string.Empty;
                lScores[lScores.Length - 1] = 0;
            }

            // 新しいスコアを入れる
            for (int i = 0; i < lScores.Length; i++)
            {
                // スコアが入る場所を探す
                if (score <= lScores[i]) { continue; }

                for (int j = lScores.Length - 1; j > i; j--)
                {
                    lNames[j] = lNames[j - 1];
                    lScores[j] = lScores[j - 1];
                }
                lNames[i] = name;
                lScores[i] = score;

                isChanged = true;
                break;
            }

            UpdateLeaderboardUI();
            return isChanged;
        }


        private void UpdateLeaderboardUI()
        {
            for (int i = 0; i < lNameTexts.Length; i++)
            {
                lNameTexts[i].text = $"{i}.  {lNames[i]}";
                lScoreTexts[i].text = $"{lScores[i]}";
            }
        }


        private bool UpdateRecords(string name, ushort line, byte combo, ushort tSpin, byte bTB, ushort perfect)
        {
            bool isChanged = false;

            if (line > rLine)
            {
                rLine = line;
                rNames[0] = name;
                isChanged = true;
            }
            if (combo > rCombo && combo > 1)
            {
                rCombo = combo;
                rNames[1] = name;
                isChanged = true;
            }
            if (tSpin > rTSpin)
            {
                rTSpin = tSpin;
                rNames[2] = name;
                isChanged = true;
            }
            if (bTB > rBTB && bTB > 1)
            {
                rBTB = bTB;
                rNames[3] = name;
                isChanged = true;
            }
            if (perfect > rPerfect)
            {
                rPerfect = perfect;
                rNames[4] = name;
                isChanged = true;
            }

            if (isChanged) { UpdateRecordsUI(); }
            return isChanged;
        }


        private void UpdateRecordsUI()
        {
            rValueTexts[0].text = $"{rLine}";
            rValueTexts[1].text = $"{Mathf.Max(rCombo - 1, 0)}";
            rValueTexts[2].text = $"{rTSpin}";
            rValueTexts[3].text = $"{Mathf.Max(rBTB - 1, 0)}";
            rValueTexts[4].text = $"{rPerfect}";
            for (int i = 0; i < rNameTexts.Length; i++)
            {
                rNameTexts[i].text = $"{rNames[i]}";
            }
        }




        public override void OnDeserialization()
        {
            UpdateLeaderboardUI();
            UpdateRecordsUI();
        }
    }
}