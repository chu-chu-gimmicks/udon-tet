
using TMPro;
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
        private readonly string[] _UIM_RESET_BUTTON_STR  = { "PLAY", "BACK TO TITLE" };

        private const float _UIM_SHOW_RESET_BUTTON_DELAY = 3.0f;





        private void UIR_Reset()
        {
            nameTMP.text       = string.Empty;
            levelTMP.text      = string.Empty;
            scoreTMP.text      = string.Empty;
            scoreDeltaTMP.text = string.Empty;

            sLineTMP.text  = string.Empty;
            sComboTMP.text = string.Empty;
            sTSpinTMP.text = string.Empty;
            sBTBTMP.text   = string.Empty;

            tLineTMP.text    = string.Empty;
            tComboTMP.text   = string.Empty;
            tTSpinTMP.text   = string.Empty;
            tBTBTMP.text     = string.Empty;
            tPerfectTMP.text = string.Empty;

            _UIM_SetUpGuide();

            gameOverUI.SetActive(false);

            logoImage.SetActive(true);
            statUIParent.SetActive(false);
            techniqueUIParent.SetActive(false);

            resetButton.SetActive(true);
            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[0];
            resetButtonTMP.text = _UIM_RESET_BUTTON_STR[0];

            titleMinos.SetActive(true);
        }


        private void UIM_Start()
        {
            nameTMP.text = PlayerName;

            levelTMP.text      = "0";
            scoreTMP.text      = "0";
            scoreDeltaTMP.text = string.Empty;

            sLineTMP.text    = "0";
            sComboTMP.text   = "0";
            sTSpinTMP.text   = "0";
            sBTBTMP.text     = "0";
            sPerfectTMP.text = "0";

            tLineTMP.text    = string.Empty;
            tComboTMP.text   = string.Empty;
            tTSpinTMP.text   = string.Empty;
            tBTBTMP.text     = string.Empty;
            tPerfectTMP.text = string.Empty;

            gameOverUI.SetActive(false);

            logoImage.SetActive(false);
            statUIParent.SetActive(true);
            techniqueUIParent.SetActive(true);

            resetButton.SetActive(false);

            titleMinos.SetActive(false);
        }


        private void UIM_Update()
        {
            UIM_UpdateTechniques();
            UIM_UpdateStat();
        }


        private void UIM_UpdateTechniques()
        {
            tLineTMP.text    = string.Empty;
            tComboTMP.text   = string.Empty;
            tTSpinTMP.text   = string.Empty;
            tBTBTMP.text     = string.Empty;
            tPerfectTMP.text = string.Empty;

            // Line
            if (DRS_Line > 1 && DRS_TSpin == TSpinState.None)
            {
                if (DRS_Line == 4)
                {
                    tLineTMP.text = _UIM_TET_STR;
                }
                else
                {
                    tLineTMP.text = DRS_Line + _UIM_LINE_STR;
                }
            }

            // Combo
            if (DRS_Combo > 1)
            {
                tComboTMP.text = DRS_Combo - 1 + _UIM_COMBO_STR;
            }

            // T-Spin
            if (DRS_TSpin != TSpinState.None)
            {
                if (DRS_TSpin == TSpinState.Mini)
                {
                    tTSpinTMP.text = _UIM_TSPIN_STR[0];
                }
                else if (DRS_TSpin == TSpinState.Normal)
                {
                    tTSpinTMP.text = _UIM_TSPIN_STR[DRS_Line];
                }
            }

            // Back To Back
            if (DRS_BTB > 1)
            {
                if (DRS_Line == 4 || DRS_TSpin > 0)
                {
                    tBTBTMP.text = DRS_BTB - 1 + _UIM_BTB_STR;
                }
                else
                {
                    tBTBTMP.text = string.Empty;
                }
            }

            // Perfect
            if (DRS_Block == 0)
            {
                tPerfectTMP.text = _UIM_PERFECT_STR;
            }
        }


        private void UIM_UpdateStat()
        {
            levelTMP.text      = STT_Level == STT_MAX_LEVEL ? $"{_UIM_MAX_LEVEL_STR}" : $"{STT_Level}";
            scoreTMP.text      = $"{STT_Score}";
            scoreDeltaTMP.text = DRS_ScoreDelta > 0 ? $"+{DRS_ScoreDelta}" : string.Empty;

            sLineTMP.text    = $"{STT_Line}";
            sComboTMP.text   = STT_Combo > 1 ? $"{STT_Combo - 1}" : "0";
            sTSpinTMP.text   = $"{STT_TSpin}";
            sBTBTMP.text     = STT_BTB > 1 ? $"{STT_BTB - 1}" : "0";
            sPerfectTMP.text = $"{STT_Perfect}";
        }


        private void UIM_Pause()
        {
            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[1];
            resetButtonTMP.text = _UIM_RESET_BUTTON_STR[1];
            resetButton.SetActive(true);
        }


        private void UIM_Continue()
        {
            resetButton.SetActive(false);
        }


        private void UIM_OnGameOver()
        {
            nameTMP.text = Networking.GetOwner(this.gameObject).displayName;

            levelTMP.text = STT_Level == STT_MAX_LEVEL ? $"{_UIM_MAX_LEVEL_STR}" : $"{STT_Level}";
            scoreTMP.text = $"{STT_Score}";
            scoreDeltaTMP.text = DRS_ScoreDelta > 0 ? $"+{DRS_ScoreDelta}" : string.Empty;

            tLineTMP.text    = string.Empty;
            tComboTMP.text   = string.Empty;
            tTSpinTMP.text   = string.Empty;
            tBTBTMP.text     = string.Empty;
            tPerfectTMP.text = string.Empty;

            gameOverUI.SetActive(true);

            logoImage.SetActive(false);
            statUIParent.SetActive(true);
            techniqueUIParent.SetActive(false);

            resetButton.transform.localPosition = _UIM_RESET_BUTTON_POS[1];
            resetButtonTMP.text = _UIM_RESET_BUTTON_STR[1];
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
