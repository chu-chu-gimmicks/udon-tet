
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private ushort DR_ScoreDelta { get; set; } = 0;

        private byte  DR_line  = 0;
        private byte  DR_combo = 0;
        private TSpinState DR_tSpin = 0;
        private byte  DR_bTB   = 0;
        private byte  DR_block = 0;

        private byte DR_Line
        {
            get { return DR_line; }
            set
            {
                DR_line = value;
                ST_Line += DR_line;
            }
        }

        private byte DR_Combo
        {
            get { return DR_combo; }
            set
            {
                DR_combo = value;
                if (DR_combo > ST_Combo) { ST_Combo = DR_combo; }
            }
        }

        private TSpinState DR_TSpin
        {
            get { return DR_tSpin; }
            set
            {
                DR_tSpin = value;
                if (DR_tSpin != TSpinState.None) { ST_TSpin++; }
            }
        }

        private byte DR_BTB
        {
            get { return DR_bTB; }
            set
            {
                DR_bTB = value;
                if (DR_bTB > ST_BTB) { ST_BTB = DR_bTB; }
            }
        }

        private byte DR_Block
        {
            get { return DR_block; }
            set
            {
                DR_block = value;
                if (DR_block == 0) { ST_Perfect++; }
            }
        }




        private void DR_Reset()
        {
            DR_ScoreDelta = 0;

            DR_line  = 0;
            DR_combo = 0;
            DR_tSpin = TSpinState.None;
            DR_bTB   = 0;
            DR_block = 0;
        }


        private void DR_CalculateScore()
        {
            if (DR_line == 0) { return; }

            int score = 0;

            if (DR_TSpin == TSpinState.Normal)
            {
                switch (DR_line)
                {
                    case 1: score = 40; break;
                    case 2: score = 80; break;
                    case 3: score = 120; break;
                }
            }
            else if (DR_TSpin == TSpinState.Mini)
            {
                score = 20;
            }
            else
            {
                switch (DR_line)
                {
                    case 1: score = 10; break;
                    case 2: score = 30; break;
                    case 3: score = 50; break;
                    case 4: score = 80; break;
                }
            }

            if (DR_line == 4 || DR_TSpin != TSpinState.None)
            {
                if (DR_bTB == 2) { score += DR_line * 10; }
                else if (DR_bTB > 2) { score += DR_line * 20; }
            }

            score += (DR_combo - 1) * 20;

            if (DR_block == 0) { score += 500; }

            // �Ō�Ƀ��x���{�[�i�X
            int multiplier = 10 + ST_Level * 2;
            if (ST_Level == ST_MAX_LEVEL) { multiplier += 4 * 2; }
            score = score * multiplier / 10;

            DR_ScoreDelta = (ushort)score;
        }
    }
}