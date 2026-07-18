
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private const byte STT_MAX_LEVEL = 30;
        private const byte _STT_THRESHOLD_LEVEL = 20; // 20レベルまでは10行ごとにレベルアップ、それ以降は20行ごとにレベルアップ

        private const byte STT_LINES_PER_LEVEL_BASE = 10;
        private const byte STT_LINES_PER_LEVEL_HIGH = 20;

        private byte STT_Level { get; set; } = 0;
        private int  STT_Score { get; set; } = 0;

        private ushort STT_Line    { get; set; } = 0;
        private byte   STT_Combo   { get; set; } = 0;
        private ushort STT_TSpin   { get; set; } = 0;
        private byte   STT_BTB     { get; set; } = 0;
        private byte   STT_Perfect { get; set; } = 0;




        private void STT_Reset()
        {
            STT_Level = 0;
            STT_Score = 0;

            STT_Line    = 0;
            STT_Combo   = 0;
            STT_TSpin   = 0;
            STT_BTB     = 0;
            STT_Perfect = 0;
        }


        private bool STT_NeedsLevelUp()
        {
            if (STT_Level == STT_MAX_LEVEL) { return false; }

            int required = 0;

            // from 0 to 19
            if (STT_Level < _STT_THRESHOLD_LEVEL)
            {
                int interval = STT_LINES_PER_LEVEL_BASE;
                required = (STT_Level * interval) + interval;
            }
            // more than 20
            else
            {
                int baseLines = _STT_THRESHOLD_LEVEL * STT_LINES_PER_LEVEL_BASE;
                int extraLevels = STT_Level - _STT_THRESHOLD_LEVEL;
                required = baseLines + ((extraLevels + 1) * STT_LINES_PER_LEVEL_HIGH);
            }

            if (STT_Line >= required)
            {
                return true;
            }
            return false;
        }


        private void STT_LevelUp()
        {
            STT_Level++;
        }
    }
}