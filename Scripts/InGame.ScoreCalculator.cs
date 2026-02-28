
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private void SC_Reset()
        {
        }


        private void SC_CalculateScoreDelta()
        {
            if (DR_line == 0)
            {
                DR_ScoreDelta = 0;
                return;
            }

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

            // レベルに応じてボーナス
            int multiplier = 10 + ST_Level * 2;
            if (ST_Level == ST_MAX_LEVEL) { multiplier += 4 * 2; }
            score = score * multiplier / 10;

            DR_ScoreDelta = (ushort)score;
        }


        private void SC_AddScoreDelta()
        {
            ST_Score += DR_ScoreDelta;
        }
    }
}
