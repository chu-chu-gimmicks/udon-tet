
using System;
using System.Reflection;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ScoreBoard : UdonSharpBehaviour
    {
        //[SerializeField] private TextMeshProUGUI debugTMP;
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




        private float timer = 0.0f;
        private const float Interval = 2.0f;
        private const float Epsilon = 0.01f;

        [Header("Max 16")]
        [SerializeField] private GameContext[] udontets;

        [Space(80)]
        [Header("---- Don't Touch ----")]
        [SerializeField] private TextMeshProUGUI yourHighScoreTMP;
        [SerializeField] private TextMeshProUGUI[] nameTMPs, scoreTMPs, achievementsValueTMP, achievementsNameTMP;


        [SerializeField] private ChuChuGimmicks.UDONTET.UserData referenceUserData;
        private ChuChuGimmicks.UDONTET.UserData userData;
        private bool isRestored = false;
        private int yourHighScore = 0;

        private string localPlayerName = string.Empty;
        [UdonSynced] private short[] playIds = new short[16];
        [UdonSynced] private string[] playerNames = new string[8];
        [UdonSynced] private int[] scores = new int[8];

        [UdonSynced] private ushort maxTotalLines = 0;
        [UdonSynced] private byte maxMaxCombo = 0;
        [UdonSynced] private ushort maxTotalTSpins = 0;
        [UdonSynced] private byte maxMaxBTB = 0;
        [UdonSynced] private ushort maxTotalPerfects = 0;
        [UdonSynced] private string maxTotalLinesName = "--------";
        [UdonSynced] private string maxMaxComboName = "--------";
        [UdonSynced] private string maxTotalTSpinsName = "--------";
        [UdonSynced] private string maxMaxBTBName = "--------";
        [UdonSynced] private string maxTotalPerfectsName = "--------";




        void OnEnable()
        {
            ResetYourHighScore();
            SendCustomEventDelayedSeconds(nameof(CheckResult), Interval);
            timer = Time.time - Interval / 2.0f;

            if (localPlayerName == string.Empty)
            {
                localPlayerName = Networking.LocalPlayer.displayName;
            }

            for (int i = 0; i < playerNames.Length; i++)
            {
                playerNames[i] = "--------";
            }
        }




        public void CheckResult()
        {
            if (udontets == null || udontets.Length == 0) { return; }

            if (Time.time + Epsilon < timer + Interval) { return; }
            SendCustomEventDelayedSeconds(nameof(CheckResult), Interval);
            timer = Time.time;

            bool isOwner = Networking.IsOwner(this.gameObject);

            for (int i = 0; i < udontets.Length; i++)
            {
                if (!Utilities.IsValid(udontets[i])) { continue; }
                if (udontets[i].inGameManager.CurrentGameState == GameState.Playing) { continue; }

                int playId = udontets[i].inGameManager.GetPlayID();
                string playerName = udontets[i].inGameManager.GetPlayerName();
                int score = udontets[i].inGameManager.GetScore();

                if (localPlayerName == playerName && isRestored)
                {
                    UpdateYourHighScore(score);
                }

                if (!isOwner) { continue; }

                if (playIds[i] != playId)
                {
                    playIds[i] = (short)playId;

                    UpdateRanking(playerName, score, out bool rChanged);
                    UpdateAchievements(i, playerName, out bool aChanged);

                    if (rChanged || aChanged)
                    {
                        RequestSerialization();
                    }
                }
            }
        }




        public void ResetYourHighScore()
        {
            if (!Utilities.IsValid(userData))
            {
                userData = (ChuChuGimmicks.UDONTET.UserData)Networking.FindComponentInPlayerObjects(Networking.LocalPlayer, referenceUserData);
                if (!Utilities.IsValid(userData)) { return; }
            }

            if (!userData.GetIsRestored())
            {
                SendCustomEventDelayedSeconds(nameof(ResetYourHighScore), 1.0f);
                isRestored = false;
                return;
            }
            else
            {
                isRestored = true;
            }

            yourHighScore = userData.GetYourHighScore();
            yourHighScoreTMP.text = $"{yourHighScore}";
        }


        private void UpdateYourHighScore(int score)
        {
            if (!Utilities.IsValid(userData)) { return; }
            if (score <= yourHighScore) { return; }

            yourHighScore = score;
            yourHighScoreTMP.text = $"{yourHighScore}";
            userData.SetYourHighScore(yourHighScore);
        }


        private void UpdateRanking(string name, int score, out bool changed)
        {
            changed = false;

            if (score <= scores[scores.Length - 1]) { return; }

            int retryIndex = -1;
            for (int i = 0; i < playerNames.Length; i++)
            {
                if (playerNames[i] == name)
                {
                    retryIndex = i;
                    break;
                }
            }

            if (retryIndex >= 0)
            {
                if (score <= scores[retryIndex])
                {
                    return;
                }
                else
                {
                    for (int i = retryIndex; i < scores.Length - 1; i++)
                    {
                        playerNames[i] = playerNames[i + 1];
                        scores[i] = scores[i + 1];
                    }
                    playerNames[scores.Length - 1] = string.Empty;
                    scores[scores.Length - 1] = 0;
                }
            }

            for (int i = 0; i < scores.Length; i++)
            {
                if (score > scores[i])
                {
                    for (int j = scores.Length - 1; j > i; j--)
                    {
                        playerNames[j] = playerNames[j - 1];
                        scores[j] = scores[j - 1];
                    }
                    playerNames[i] = name;
                    scores[i] = score;
                    break;
                }
            }

            UpdateRankingTMP();

            changed = true;
        }


        private void UpdateRankingTMP()
        {
            for (int i = 0; i < nameTMPs.Length; i++)
            {
                nameTMPs[i].text = $"{i}.  {playerNames[i]}";
                scoreTMPs[i].text = scores[i].ToString();
            }
        }


        private void UpdateAchievements(int num, string name, out bool changed)
        {
            changed = false;
            if (achievementsValueTMP.Length < 5) { return; }

            ushort totalLines = udontets[num].inGameManager.GetLineStat();
            byte maxCombo = udontets[num].inGameManager.GetComboStat();
            ushort totalTSpins = udontets[num].inGameManager.GetTSpinStat();
            byte maxBTB = udontets[num].inGameManager.GetBTBStat();
            ushort totalPerfects = udontets[num].inGameManager.GetPerfectStat();

            if (totalLines > maxTotalLines)
            {
                maxTotalLines = totalLines;
                maxTotalLinesName = name;

                changed = true;
            }
            if (maxCombo > maxMaxCombo && maxCombo > 1)
            {
                maxMaxCombo = maxCombo;
                maxMaxComboName = name;

                changed = true;
            }
            if (totalTSpins > maxTotalTSpins)
            {
                maxTotalTSpins = totalTSpins;
                maxTotalTSpinsName = name;

                changed = true;
            }
            if (maxBTB > maxMaxBTB && maxBTB > 1)
            {
                maxMaxBTB = maxBTB;
                maxMaxBTBName = name;

                changed = true;
            }
            if (totalPerfects > maxTotalPerfects)
            {
                maxTotalPerfects = totalPerfects;
                maxTotalPerfectsName = name;

                changed = true;
            }

            if (changed) { UpdateAchievementsTMP(); }
        }


        private void UpdateAchievementsTMP()
        {
            if (achievementsValueTMP == null || achievementsValueTMP.Length < 5) { return; }
            achievementsValueTMP[0].text = $"{maxTotalLines}";
            achievementsValueTMP[1].text = $"{maxMaxCombo}";
            achievementsValueTMP[2].text = $"{maxTotalTSpins}";
            achievementsValueTMP[3].text = $"{maxMaxBTB}";
            achievementsValueTMP[4].text = $"{maxTotalPerfects}";

            achievementsNameTMP[0].text = $"{maxTotalLinesName}";
            achievementsNameTMP[1].text = $"{maxMaxComboName}";
            achievementsNameTMP[2].text = $"{maxTotalTSpinsName}";
            achievementsNameTMP[3].text = $"{maxMaxBTBName}";
            achievementsNameTMP[4].text = $"{maxTotalPerfectsName}";
        }




        //
        // 同期させる処理
        //




        public override void OnDeserialization()
        {
            UpdateRankingTMP();
            UpdateAchievementsTMP();
        }
    }
}