
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const string _UIM_MAX_LEVEL_STR = "UDON";

        private const string _UIM_LINE_STR = " LINES!", _UIM_TET_STR = "TET!";
        private const string _UIM_COMBO_STR = " COMBO!";
        private readonly string[] _UIM_TSPIN_STR = { "T-SPIN MINI!", "T-SPIN SINGLE!", "T-SPIN DOUBLE!", "T-SPIN TRIPLE!" };
        private const string _UIM_BTB_STR = " BTB!";
        private const string _UIM_PERFECT_STR = "PERFECT!";

        private readonly Vector3[] _UIM_RESET_BUTTON_POS = { new Vector3(0, 0, 0), new Vector3(0, 0, -50.2f) };
        private readonly string[] _UIM_RESET_BUTTON_STR  = { "PLAY", "BACK TO GAME", "BACK TO TITLE" };

        private const float _UIM_SHOW_RESET_BUTTON_DELAY = 3.0f;





        private void UIR_Reset()
        {
            logoImage.SetActive(true);

            playerNameLabel.text = string.Empty;

            yourHighScoreUIParent.SetActive(true);

            levelLabel.text      = string.Empty;
            scoreLabel.text      = string.Empty;
            scoreDeltaLabel.text = string.Empty;

            sLineLabel.text  = string.Empty;
            sComboLabel.text = string.Empty;
            sTSpinLabel.text = string.Empty;
            sBTBLabel.text   = string.Empty;
            statsUIParent.SetActive(false);

            pLineLabel.text    = string.Empty;
            pComboLabel.text   = string.Empty;
            pTSpinLabel.text   = string.Empty;
            pBTBLabel.text     = string.Empty;
            pPerfectLabel.text = string.Empty;
            pupupUIParent.SetActive(false);

            gameOverUIParent.SetActive(false);

            resetButton.SetActive(true);
            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[0];
            resetButtonLabel.text = _UIM_RESET_BUTTON_STR[0];

            _UIM_SetUpGuide();
            titleMinos.SetActive(true);
        }


        public void UIM_ShowYourHighScore()
        {
            yourHighScoreLabel.text = $"{userDataAccessor.GetYourHighScore()}";
        }


        private void UIM_Start()
        {
            logoImage.SetActive(false);

            playerNameLabel.text = PlayerName;

            yourHighScoreUIParent.SetActive(false);

            levelLabel.text      = "0";
            scoreLabel.text      = "0";
            scoreDeltaLabel.text = string.Empty;

            sLineLabel.text    = "0";
            sComboLabel.text   = "0";
            sTSpinLabel.text   = "0";
            sBTBLabel.text     = "0";
            sPerfectLabel.text = "0";
            statsUIParent.SetActive(true);

            pLineLabel.text    = string.Empty;
            pComboLabel.text   = string.Empty;
            pTSpinLabel.text   = string.Empty;
            pBTBLabel.text     = string.Empty;
            pPerfectLabel.text = string.Empty;
            pupupUIParent.SetActive(true);

            gameOverUIParent.SetActive(false);

            resetButton.SetActive(false);

            titleMinos.SetActive(false);
        }


        private void UIM_Update()
        {
            UIM_UpdateStats();
            UIM_UpdatePupup();
        }


        private void UIM_UpdateStats()
        {
            levelLabel.text      = STT_Level == STT_MAX_LEVEL ? $"{_UIM_MAX_LEVEL_STR}" : $"{STT_Level}";
            scoreLabel.text      = $"{STT_Score}";
            scoreDeltaLabel.text = DRS_ScoreDelta > 0 ? $"+{DRS_ScoreDelta}" : string.Empty;

            sLineLabel.text    = $"{STT_Line}";
            sComboLabel.text   = STT_Combo > 1 ? $"{STT_Combo - 1}" : "0";
            sTSpinLabel.text   = $"{STT_TSpin}";
            sBTBLabel.text     = STT_BTB > 1 ? $"{STT_BTB - 1}" : "0";
            sPerfectLabel.text = $"{STT_Perfect}";
        }


        private void UIM_UpdatePupup()
        {
            pLineLabel.text    = string.Empty;
            pComboLabel.text   = string.Empty;
            pTSpinLabel.text   = string.Empty;
            pBTBLabel.text     = string.Empty;
            pPerfectLabel.text = string.Empty;

            // Line
            if (DRS_Line > 1 && DRS_TSpin == TSpinState.None)
            {
                if (DRS_Line == 4)
                {
                    pLineLabel.text = _UIM_TET_STR;
                }
                else
                {
                    pLineLabel.text = DRS_Line + _UIM_LINE_STR;
                }
            }

            // Combo
            if (DRS_Combo > 1)
            {
                pComboLabel.text = DRS_Combo - 1 + _UIM_COMBO_STR;
            }

            // T-Spin
            if (DRS_TSpin != TSpinState.None)
            {
                if (DRS_TSpin == TSpinState.Mini)
                {
                    pTSpinLabel.text = _UIM_TSPIN_STR[0];
                }
                else if (DRS_TSpin == TSpinState.Normal)
                {
                    pTSpinLabel.text = _UIM_TSPIN_STR[DRS_Line];
                }
            }

            // Back To Back
            if (DRS_BTB > 1)
            {
                if (DRS_Line == 4 || DRS_TSpin > 0)
                {
                    pBTBLabel.text = DRS_BTB - 1 + _UIM_BTB_STR;
                }
                else
                {
                    pBTBLabel.text = string.Empty;
                }
            }

            // Perfect
            if (DRS_Block == 0)
            {
                pPerfectLabel.text = _UIM_PERFECT_STR;
            }
        }


        private void UIM_Pause()
        {
            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[1];
            resetButtonLabel.text = _UIM_RESET_BUTTON_STR[1];
            resetButton.SetActive(true);
        }


        private void UIM_Resume()
        {
            resetButton.SetActive(false);
        }


        private void UIM_OnGameOver()
        {
            logoImage.SetActive(false);

            playerNameLabel.text = Networking.GetOwner(this.gameObject).displayName;

            yourHighScoreUIParent.SetActive(false);

            UIM_UpdateStats();
            statsUIParent.SetActive(true);

            pLineLabel.text    = string.Empty;
            pComboLabel.text   = string.Empty;
            pTSpinLabel.text   = string.Empty;
            pBTBLabel.text     = string.Empty;
            pPerfectLabel.text = string.Empty;
            pupupUIParent.SetActive(false);

            gameOverUIParent.SetActive(true);

            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[1];
            resetButtonLabel.text = _UIM_RESET_BUTTON_STR[2];
            SendCustomEventDelayedSeconds(nameof(_UIM_ShowResetButton), _UIM_SHOW_RESET_BUTTON_DELAY);

            titleMinos.SetActive(false);
        }


        public void _UIM_ShowResetButton()
        {
            resetButton.SetActive(true);
        }


        private void _UIM_SetUpGuide()
        {
            guide.material = guideMaterial[Networking.LocalPlayer.IsUserInVR() ? 0 : 1];
        }


        private void UIM_HideFake()
        {
            fake.SetActive(false);
        }
    }
}
