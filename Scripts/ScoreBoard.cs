
using ChuChuGimmicks.UDONBINGO;
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




        [Header("Max 16")]
        [SerializeField] private GameContext[] udontets;

        [Space(80)]
        [Header("---- Don't Touch ----")]
        [SerializeField] private UserDataAccessor userDataAccessor;

        [SerializeField] private TextMeshProUGUI yourHighScoreTMP;
        [SerializeField] private TextMeshProUGUI[] nameTMPs, scoreTMPs, achievementsValueTMP, achievementsNameTMP;

        private bool isPending = false;
        private const float INTERVAL = 2.0f;

        private int yourHighScore = 0;

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
            for (int i = 0; i < playerNames.Length; i++)
            {
                playerNames[i] = "--------";
            }

            if (isPending) { return; }
            SendCustomEventDelayedSeconds(nameof(CheckResult), INTERVAL);
        }




        public void CheckResult()
        {
            if (!Utilities.IsValid(udontets) || udontets.Length == 0) { return; }

            // 予約するか判断
            if (this.enabled)
            {
                SendCustomEventDelayedSeconds(nameof(CheckResult), INTERVAL);
            }
            else
            {
                isPending = false;
                return;
            }

            UpdateScoreBoard();
        }


        private void UpdateScoreBoard()
        {
            UpdateYHS();

            if (!Networking.IsOwner(this.gameObject)) { return; }

            bool isChanged = false;

            for (int i = 0; i < udontets.Length; i++)
            {
                if (!Utilities.IsValid(udontets[i])) { continue; }
                if (udontets[i].inGameManager.CurrentGameState == GameState.Playing) { continue; }

                int playId        = udontets[i].inGameManager.GetPlayID();
                string playerName = udontets[i].inGameManager.GetPlayerName();
                int score         = udontets[i].inGameManager.GetScore();

                if (playIds[i] > playId)
                {
                    playIds[i] = (short)playId;

                    if (UpdateRanking(playerName, score) || UpdateAchievements(i, playerName))
                    {
                        isChanged = true;
                    }
                }
            }

            if (isChanged)
            {
                RequestSerialization();
            }
        }


        private void UpdateYHS()
        {
            int value = userDataAccessor.GetYourHighScore();
            if (value <= yourHighScore) { return; }
            yourHighScore = value;
            yourHighScoreTMP.text = $"{yourHighScore}";
        }


        private bool UpdateRanking(string name, int score)
        {
            if (score <= scores[scores.Length - 1]) { return false; }

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
                    return false;
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

            return true;
        }


        private void UpdateRankingTMP()
        {
            for (int i = 0; i < nameTMPs.Length; i++)
            {
                nameTMPs[i].text = $"{i}.  {playerNames[i]}";
                scoreTMPs[i].text = scores[i].ToString();
            }
        }


        private bool UpdateAchievements(int num, string name)
        {
            bool isChanged = false;
            if (achievementsValueTMP.Length < 5) { return isChanged; }

            ushort totalLines = udontets[num].inGameManager.GetLineStat();
            byte maxCombo = udontets[num].inGameManager.GetComboStat();
            ushort totalTSpins = udontets[num].inGameManager.GetTSpinStat();
            byte maxBTB = udontets[num].inGameManager.GetBTBStat();
            ushort totalPerfects = udontets[num].inGameManager.GetPerfectStat();

            if (totalLines > maxTotalLines)
            {
                maxTotalLines = totalLines;
                maxTotalLinesName = name;

                isChanged = true;
            }
            if (maxCombo > maxMaxCombo && maxCombo > 1)
            {
                maxMaxCombo = maxCombo;
                maxMaxComboName = name;

                isChanged = true;
            }
            if (totalTSpins > maxTotalTSpins)
            {
                maxTotalTSpins = totalTSpins;
                maxTotalTSpinsName = name;

                isChanged = true;
            }
            if (maxBTB > maxMaxBTB && maxBTB > 1)
            {
                maxMaxBTB = maxBTB;
                maxMaxBTBName = name;

                isChanged = true;
            }
            if (totalPerfects > maxTotalPerfects)
            {
                maxTotalPerfects = totalPerfects;
                maxTotalPerfectsName = name;

                isChanged = true;
            }

            if (isChanged) { UpdateAchievementsTMP(); }

            return isChanged;
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