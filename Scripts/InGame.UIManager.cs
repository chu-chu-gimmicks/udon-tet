
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

            playerNameText.text = string.Empty;

            yourHighScoreUIParent.SetActive(true);

            levelValueText.text      = string.Empty;
            scoreValueText.text      = string.Empty;
            scoreDeltaValueText.text = string.Empty;

            sLineValueText.text  = string.Empty;
            sComboValueText.text = string.Empty;
            sTSpinValueText.text = string.Empty;
            sBTBValueText.text   = string.Empty;
            statsUIParent.SetActive(false);

            pLineValueText.text    = string.Empty;
            pComboValueText.text   = string.Empty;
            pTSpinValueText.text   = string.Empty;
            pBTBValueText.text     = string.Empty;
            pPerfectValueText.text = string.Empty;
            pupupUIParent.SetActive(false);

            gameOverUIParent.SetActive(false);

            resetButton.SetActive(true);
            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[0];
            resetButtonText.text = _UIM_RESET_BUTTON_STR[0];

            _UIM_SetUpGuide();
            UIM_HideGuide();
            guideImage.gameObject.SetActive(false);

            titleMinos.SetActive(true);
        }


        public void UIM_ShowYourHighScore()
        {
            yourHighScoreValueText.text = $"{userDataAccessor.GetYourHighScore()}";
        }


        private void UIM_Start()
        {
            logoImage.SetActive(false);

            playerNameText.text = PlayerName;

            yourHighScoreUIParent.SetActive(false);

            levelValueText.text      = "0";
            scoreValueText.text      = "0";
            scoreDeltaValueText.text = string.Empty;

            sLineValueText.text    = "0";
            sComboValueText.text   = "0";
            sTSpinValueText.text   = "0";
            sBTBValueText.text     = "0";
            sPerfectValueText.text = "0";
            statsUIParent.SetActive(true);

            pLineValueText.text    = string.Empty;
            pComboValueText.text   = string.Empty;
            pTSpinValueText.text   = string.Empty;
            pBTBValueText.text     = string.Empty;
            pPerfectValueText.text = string.Empty;
            pupupUIParent.SetActive(true);

            gameOverUIParent.SetActive(false);

            resetButton.SetActive(false);

            UIM_HideGuide();
            guideImage.gameObject.SetActive(true);

            titleMinos.SetActive(false);
        }


        private void UIM_Update()
        {
            UIM_UpdateStats();
            UIM_UpdatePupup();
        }


        private void UIM_UpdateStats()
        {
            levelValueText.text      = STT_Level == STT_MAX_LEVEL ? $"{_UIM_MAX_LEVEL_STR}" : $"{STT_Level}";
            scoreValueText.text      = $"{STT_Score}";
            scoreDeltaValueText.text = DRS_ScoreDelta > 0 ? $"+{DRS_ScoreDelta}" : string.Empty;

            sLineValueText.text    = $"{STT_Line}";
            sComboValueText.text   = STT_Combo > 1 ? $"{STT_Combo - 1}" : "0";
            sTSpinValueText.text   = $"{STT_TSpin}";
            sBTBValueText.text     = STT_BTB > 1 ? $"{STT_BTB - 1}" : "0";
            sPerfectValueText.text = $"{STT_Perfect}";
        }


        private void UIM_UpdatePupup()
        {
            pLineValueText.text    = string.Empty;
            pComboValueText.text   = string.Empty;
            pTSpinValueText.text   = string.Empty;
            pBTBValueText.text     = string.Empty;
            pPerfectValueText.text = string.Empty;

            // Line
            if (DRS_Line > 1 && DRS_TSpin == TSpinState.None)
            {
                if (DRS_Line == 4)
                {
                    pLineValueText.text = _UIM_TET_STR;
                }
                else
                {
                    pLineValueText.text = DRS_Line + _UIM_LINE_STR;
                }
            }

            // Combo
            if (DRS_Combo > 1)
            {
                pComboValueText.text = DRS_Combo - 1 + _UIM_COMBO_STR;
            }

            // T-Spin
            if (DRS_TSpin != TSpinState.None)
            {
                if (DRS_TSpin == TSpinState.Mini)
                {
                    pTSpinValueText.text = _UIM_TSPIN_STR[0];
                }
                else if (DRS_TSpin == TSpinState.Normal)
                {
                    pTSpinValueText.text = _UIM_TSPIN_STR[DRS_Line];
                }
            }

            // Back To Back
            if (DRS_BTB > 1)
            {
                if (DRS_Line == 4 || DRS_TSpin > 0)
                {
                    pBTBValueText.text = DRS_BTB - 1 + _UIM_BTB_STR;
                }
                else
                {
                    pBTBValueText.text = string.Empty;
                }
            }

            // Perfect
            if (DRS_Block == 0)
            {
                pPerfectValueText.text = _UIM_PERFECT_STR;
            }
        }


        private void UIM_Pause()
        {
            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[1];
            resetButtonText.text = _UIM_RESET_BUTTON_STR[1];
            resetButton.SetActive(true);
        }


        private void UIM_Resume()
        {
            resetButton.SetActive(false);
        }


        private void UIM_OnGameOver()
        {
            logoImage.SetActive(false);

            playerNameText.text = Networking.GetOwner(this.gameObject).displayName;

            yourHighScoreUIParent.SetActive(false);

            UIM_UpdateStats();
            statsUIParent.SetActive(true);

            pLineValueText.text    = string.Empty;
            pComboValueText.text   = string.Empty;
            pTSpinValueText.text   = string.Empty;
            pBTBValueText.text     = string.Empty;
            pPerfectValueText.text = string.Empty;
            pupupUIParent.SetActive(false);

            gameOverUIParent.SetActive(true);

            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[1];
            resetButtonText.text = _UIM_RESET_BUTTON_STR[2];
            SendCustomEventDelayedSeconds(nameof(_UIM_ShowResetButton), _UIM_SHOW_RESET_BUTTON_DELAY);

            UIM_HideGuide();
            guideImage.gameObject.SetActive(false);

            titleMinos.SetActive(false);
        }


        public void _UIM_ShowResetButton()
        {
            resetButton.SetActive(true);
        }


        private void _UIM_SetUpGuide()
        {
            guideImage.sprite = guideSprite[Networking.LocalPlayer.IsUserInVR() ? 0 : 1];
        }


        private void UIM_ShowGuide()
        {
            guideImage.enabled = true;
        }


        private void UIM_HideGuide()
        {
            guideImage.enabled = false;
        }


        private void UIM_HideFake()
        {
            fake.SetActive(false);
        }
    }
}
