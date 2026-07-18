
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private ushort DRS_ScoreDelta { get; set; } = 0;

        private byte  _DRS_line  = 0;
        private byte  _DRS_combo = 0;
        private TSpinState _DRS_tSpin = TSpinState.None;
        private byte  _DRS_bTB   = 0;
        private byte  _DRS_block = 0;

        private byte DRS_Line
        {
            get { return _DRS_line; }
            set
            {
                _DRS_line = value;
                STT_Line += _DRS_line;
            }
        }

        private byte DRS_Combo
        {
            get { return _DRS_combo; }
            set
            {
                _DRS_combo = value;
                if (_DRS_combo > STT_Combo) { STT_Combo = _DRS_combo; }
            }
        }

        private TSpinState DRS_TSpin
        {
            get { return _DRS_tSpin; }
            set
            {
                _DRS_tSpin = value;
                if (_DRS_tSpin != TSpinState.None) { STT_TSpin++; }
            }
        }

        private byte DRS_BTB
        {
            get { return _DRS_bTB; }
            set
            {
                _DRS_bTB = value;
                if (_DRS_bTB > STT_BTB) { STT_BTB = _DRS_bTB; }
            }
        }

        private byte DRS_Block
        {
            get { return _DRS_block; }
            set
            {
                _DRS_block = value;
                if (_DRS_block == 0) { STT_Perfect++; }
            }
        }




        private void DRS_Reset()
        {
            DRS_ScoreDelta = 0;

            _DRS_line  = 0;
            _DRS_combo = 0;
            _DRS_tSpin = TSpinState.None;
            _DRS_bTB   = 0;
            _DRS_block = 0;
        }


        private void DRS_CalculateScoreDelta()
        {
            DRS_ScoreDelta = 0;

            if (DRS_Line == 0) { return; }

            int score = 0;

            if (DRS_TSpin == TSpinState.Normal)
            {
                switch (DRS_Line)
                {
                    case 1: score = 40; break;
                    case 2: score = 80; break;
                    case 3: score = 120; break;
                }
            }
            else if (DRS_TSpin == TSpinState.Mini)
            {
                score = 20;
            }
            else
            {
                switch (DRS_Line)
                {
                    case 1: score = 10; break;
                    case 2: score = 30; break;
                    case 3: score = 50; break;
                    case 4: score = 80; break;
                }
            }

            if (DRS_Line == 4 || DRS_TSpin != TSpinState.None)
            {
                if (DRS_BTB == 2) { score += DRS_Line * 10; }
                else if (DRS_BTB > 2) { score += DRS_Line * 20; }
            }

            score += (DRS_Combo - 1) * 20;

            if (DRS_Block == 0) { score += 500; }

            // レベルに応じてボーナス（最高7倍）
            int multiplier = 10 + STT_Level * 2;
            score = (int)(score * multiplier * 0.1f);

            DRS_ScoreDelta = (ushort)score;
        }


        private void DRS_AddScoreDelta()
        {
            STT_Score += DRS_ScoreDelta;
            DRS_ScoreDelta = 0;
        }
    }
}