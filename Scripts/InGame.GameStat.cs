
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const byte ST_MAX_LEVEL = 26;
        private const byte ST_THRESHOLD_LEVEL = 20;

        private const byte ST_LINES_PER_LEVEL_BASE = 10;
        private const byte ST_LINES_PER_LEVEL_HIGH = 20;

        private byte ST_Level { get; set; } = 0;
        private int  ST_Score { get; set; } = 0;

        private ushort ST_Line    { get; set; } = 0;
        private byte  ST_Combo   { get; set; } = 0;
        private ushort ST_TSpin   { get; set; } = 0;
        private byte  ST_BTB     { get; set; } = 0;
        private byte  ST_Perfect { get; set; } = 0;




        private void ST_Reset()
        {
            ST_Level = 0;
            ST_Score = 0;

            ST_Line    = 0;
            ST_Combo   = 0;
            ST_TSpin   = 0;
            ST_BTB     = 0;
            ST_Perfect = 0;
        }


        private void ST_UpdateLevel(out bool hasLeveledUp)
        {
            hasLeveledUp = false;
            if (ST_Level == ST_MAX_LEVEL) { return; }

            int required = 0;

            // from 0 to 19
            if (ST_Level < ST_THRESHOLD_LEVEL)
            {
                int interval = ST_LINES_PER_LEVEL_BASE;
                required = (ST_Level * interval) + interval;
            }
            // more than 20
            else
            {
                int baseLines = ST_THRESHOLD_LEVEL * ST_LINES_PER_LEVEL_BASE;
                int extraLevels = ST_Level - ST_THRESHOLD_LEVEL;
                required = baseLines + ((extraLevels + 1) * ST_LINES_PER_LEVEL_HIGH);
            }

            if (ST_Line >= required)
            {
                hasLeveledUp = true;
                ST_Level++;
            }
        }
    }
}