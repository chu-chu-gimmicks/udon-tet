
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        // Stats
        private const string _UIM_MAX_LEVEL_TEXT = "UDON";

        // Popup
        private const string      _UIM_LINE_TEXT    = "LINES", _UIM_TET_STR = "TET";
        private const string      _UIM_COMBO_TEXT   = "COMBO";
        private readonly string[] _UIM_TSPIN_TEXT   = { "T-SPIN MINI", "T-SPIN SINGLE", "T-SPIN DOUBLE", "T-SPIN TRIPLE" };
        private const string      _UIM_BTB_TEXT     = "B2B";
        private const string      _UIM_PERFECT_TEXT = "PERFECT CLEAR";

        // Game Over
        private const float _UIM_GAME_OVER_DELAY = 2.5f;

        // Reset Button
        private readonly Vector3[] _UIM_RESET_BUTTON_POS = { new Vector3(0, 0, 0), new Vector3(0, 0, -32.2f) };
        private readonly string[]  _UIM_RESET_BUTTON_TEXT  = { "PLAY", "CONTINUE", "TO TITLE" };





        private void UIM_Reset()
        {
            _playerNameText.text = string.Empty;

            _titleUI.SetActive(true);

            _statsValueText.text = string.Empty;
            _popupText.text = string.Empty;
            _ingameUI.SetActive(false);

            _gameOverUI.SetActive(false);

            _button.transform.localPosition = _UIM_RESET_BUTTON_POS[0];
            _buttonText.text = _UIM_RESET_BUTTON_TEXT[0];
            _button.SetActive(true);

            _UIM_SetUpGuide();
            _guide.SetActive(false);

            _titleMino.SetActive(true);
        }


        private void UIM_Start()
        {
            _playerNameText.text = PlayerName;

            _titleUI.SetActive(false);

            _statsValueText.text =
                "0\n" +
                "\n" + 
                "0\n" +
                "\n" + 
                "0\n" +
                "0\n" +
                "0\n" +
                "0\n" +
                "0"
                ;
            _popupText.text = string.Empty;
            _ingameUI.SetActive(true);

            _gameOverUI.SetActive(false);

            _button.SetActive(false);

            if (Networking.IsOwner(this.gameObject))
            {
                _guide.SetActive(true);
            }

            _titleMino.SetActive(false);
        }


        private void UIM_Update()
        {
            UIM_UpdateStats();
            UIM_UpdatePupup();
        }


        private void UIM_UpdateStats()
        {
            string value = string.Empty;
            value += $"{STT_Score}\n";
            value += DRS_ScoreDelta > 0 ? $"+{DRS_ScoreDelta}\n" : "\n";
            value += STT_Level == STT_MAX_LEVEL ? $"{_UIM_MAX_LEVEL_TEXT}\n" : $"{STT_Level}\n";
            value += "\n";
            value += $"{STT_Line} \n";
            value += STT_Comb > 1 ? $"{STT_Comb - 1}\n" : "0\n";
            value += $"{STT_Tspn} \n";
            value += STT_Btob > 1 ? $"{STT_Btob - 1} \n" : "0\n";
            value += $"{STT_Perf}";
            _statsValueText.text = value;
        }


        private void UIM_UpdatePupup()
        {
            string value = string.Empty;
            _popupText.text = string.Empty;

            if (DRS_Line == 0) { return; }

            // Line
            if (DRS_Line > 1 && DRS_Tspn == TSpinState.None)
            {
                if (DRS_Line == 4)
                {
                    value += $"{_UIM_TET_STR}\n";
                }
                else
                {
                    value += $"{DRS_Line} {_UIM_LINE_TEXT}\n";
                }
            }

            // Combo
            if (DRS_Comb > 1)
            {
                value += $"{DRS_Comb - 1} {_UIM_COMBO_TEXT}\n";
            }

            // T-Spin
            if (DRS_Tspn != TSpinState.None)
            {
                if (DRS_Tspn == TSpinState.Mini)
                {
                    value += $"{_UIM_TSPIN_TEXT[0]}\n";
                }
                else if (DRS_Tspn == TSpinState.Normal)
                {
                    value += $"{_UIM_TSPIN_TEXT[DRS_Line]}\n";
                }
            }

            // Back To Back
            if (DRS_Btob > 1)
            {
                if (DRS_Line == 4 || DRS_Tspn > 0)
                {
                    value += $"{_UIM_BTB_TEXT} {DRS_Btob - 1}\n";
                }
            }

            // Perfect
            if (DRS_Block == 0)
            {
                value += $"{_UIM_PERFECT_TEXT}";
            }

            _popupText.text = value;
        }


        private void UIM_Pause()
        {
            _button.transform.localPosition = _UIM_RESET_BUTTON_POS[1];
            _buttonText.text = _UIM_RESET_BUTTON_TEXT[1];
            _button.SetActive(true);
        }


        private void UIM_Resume()
        {
            _button.SetActive(false);
        }


        private void UIM_OnGameOver()
        {
            _playerNameText.text = PlayerName;

            _titleUI.SetActive(false);

            UIM_UpdateStats();
            _popupText.text = string.Empty;
            _ingameUI.SetActive(true);

            _gameOverUI.SetActive(true);

            _button.SetActive(false);

            _guide.SetActive(false);

            _titleMino.SetActive(false);
        }


        private void UIM_OnResettable()
        {
            _playerNameText.text = PlayerName;

            _titleUI.SetActive(false);

            UIM_UpdateStats();
            _popupText.text = string.Empty;
            _ingameUI.SetActive(true);

            _gameOverUI.SetActive(true);

            _button.transform.localPosition = _UIM_RESET_BUTTON_POS[1];
            _buttonText.text = _UIM_RESET_BUTTON_TEXT[2];
            _button.SetActive(true);

            _guide.SetActive(false);

            _titleMino.SetActive(false);
        }


        private void _UIM_SetUpGuide()
        {
            bool isVR = Networking.LocalPlayer.IsUserInVR();
            _guideBindings[0].gameObject.SetActive(isVR);
            _guideBindings[1].gameObject.SetActive(!isVR);
        }
    }
}
