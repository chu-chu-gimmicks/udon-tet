
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const string _UI_MAX_LEVEL_STR = "UDON";

        private const string _UI_LINE_STR = " LINES!", _UI_TET_STR = "TET!";
        private const string _UI_COMBO_STR = " COMBO!";
        private readonly string[] _UI_TSPIN_STR = { "T-SPIN MINI!", "T-SPIN SINGLE!", "T-SPIN DOUBLE!", "T-SPIN TRIPLE!" };
        private const string _UI_BTB_STR = " BTB!";
        private const string _UI_PERFECT_STR = "PERFECT!";

        private readonly Vector3[] _UI_RESET_BUTTON_POS = { new Vector3(0, 0, 0), new Vector3(0, 0, -50.2f) };
        private readonly string[] _UI_RESET_BUTTON_STR  = { "PLAY", "BACK TO TITLE" };

        private const float _UI_SHOW_RESET_BUTTON_DELAY = 3.0f;





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

            _UI_SetUpGuide();

            gameOverUI.SetActive(false);

            logoImage.SetActive(true);
            statUIParent.SetActive(false);
            techniqueUIParent.SetActive(false);

            resetButton.SetActive(true);
            resetButton.transform.localPosition = _UI_RESET_BUTTON_POS[0];
            resetButtonTMP.text = _UI_RESET_BUTTON_STR[0];

            titleMinos.SetActive(true);
        }


        private void UI_Start()
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


        private void UI_Update()
        {
            UI_UpdateTechniques();
            UI_UpdateStat();
        }


        private void UI_UpdateTechniques()
        {
            tLineTMP.text    = string.Empty;
            tComboTMP.text   = string.Empty;
            tTSpinTMP.text   = string.Empty;
            tBTBTMP.text     = string.Empty;
            tPerfectTMP.text = string.Empty;

            // Line
            if (DR_Line > 1 && DR_TSpin == TSpinState.None)
            {
                if (DR_Line == 4)
                {
                    tLineTMP.text = _UI_TET_STR;
                }
                else
                {
                    tLineTMP.text = DR_Line + _UI_LINE_STR;
                }
            }

            // Combo
            if (DR_Combo > 1)
            {
                tComboTMP.text = DR_Combo - 1 + _UI_COMBO_STR;
            }

            // T-Spin
            if (DR_TSpin != TSpinState.None)
            {
                if (DR_TSpin == TSpinState.Mini)
                {
                    tTSpinTMP.text = _UI_TSPIN_STR[0];
                }
                else if (DR_TSpin == TSpinState.Normal)
                {
                    tTSpinTMP.text = _UI_TSPIN_STR[DR_Line];
                }
            }

            // Back To Back
            if (DR_BTB > 1)
            {
                if (DR_Line == 4 || DR_TSpin > 0)
                {
                    tBTBTMP.text = DR_BTB - 1 + _UI_BTB_STR;
                }
                else
                {
                    tBTBTMP.text = string.Empty;
                }
            }

            // Perfect
            if (DR_Block == 0)
            {
                tPerfectTMP.text = _UI_PERFECT_STR;
            }
        }


        private void UI_UpdateStat()
        {
            levelTMP.text      = ST_Level == ST_MAX_LEVEL ? $"{_UI_MAX_LEVEL_STR}" : $"{ST_Level}";
            scoreTMP.text      = $"{ST_Score}";
            scoreDeltaTMP.text = DR_ScoreDelta > 0 ? $"+{DR_ScoreDelta}" : string.Empty;

            sLineTMP.text    = $"{ST_Line}";
            sComboTMP.text   = ST_Combo > 1 ? $"{ST_Combo - 1}" : "0";
            sTSpinTMP.text   = $"{ST_TSpin}";
            sBTBTMP.text     = ST_BTB > 1 ? $"{ST_BTB - 1}" : "0";
            sPerfectTMP.text = $"{ST_Perfect}";
        }


        private void UI_OnGameOver()
        {
            nameTMP.text = Networking.GetOwner(this.gameObject).displayName;

            levelTMP.text = ST_Level == ST_MAX_LEVEL ? $"{_UI_MAX_LEVEL_STR}" : $"{ST_Level}";
            scoreTMP.text = $"{ST_Score}";
            scoreDeltaTMP.text = DR_ScoreDelta > 0 ? $"+{DR_ScoreDelta}" : string.Empty;

            tLineTMP.text    = string.Empty;
            tComboTMP.text   = string.Empty;
            tTSpinTMP.text   = string.Empty;
            tBTBTMP.text     = string.Empty;
            tPerfectTMP.text = string.Empty;

            gameOverUI.SetActive(true);

            logoImage.SetActive(false);
            statUIParent.SetActive(true);
            techniqueUIParent.SetActive(false);

            SendCustomEventDelayedSeconds(nameof(_UI_ShowResetButton), _UI_SHOW_RESET_BUTTON_DELAY);

            titleMinos.SetActive(false);
        }


        public void _UI_ShowResetButton()
        {
            resetButton.SetActive(true);
            resetButton.transform.localPosition = _UI_RESET_BUTTON_POS[1];
            resetButtonTMP.text = _UI_RESET_BUTTON_STR[1];
        }


        private void _UI_SetUpGuide()
        {
            guide.material = guideMaterial[Networking.LocalPlayer.IsUserInVR() ? 0 : 1];
        }


        private void UI_HideFake()
        {
            fake.SetActive(false);
        }
    }
}
