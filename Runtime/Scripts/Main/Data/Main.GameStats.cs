
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        private const byte STT_MAX_LEVEL = 30;
        private const byte _STT_THRESHOLD_LEVEL = 20; // 20レベルまでは10行ごとにレベルアップ、それ以降は20行ごとにレベルアップ

        private const byte STT_LINES_PER_LEVEL_BASE = 10;
        private const byte STT_LINES_PER_LEVEL_HIGH = 20;

        private uint   _STT_score = 0;
        private ushort _STT_level = 0;

        private ushort _STT_line = 0;
        private ushort _STT_comb = 0;
        private ushort _STT_tspn = 0;
        private ushort _STT_btob = 0;
        private ushort _STT_perf = 0;

        public uint   STT_Score { get { return _STT_score; } set { _STT_score = DTU_ClampToMax(value); } }
        public ushort STT_Level { get { return _STT_level; } set { _STT_level = DTU_ClampToMax(value); } }

        public ushort STT_Line { get { return _STT_line; } set { _STT_line = DTU_ClampToMax(value); } }
        public ushort STT_Comb { get { return _STT_comb; } set { _STT_comb = DTU_ClampToMax(value); } }
        public ushort STT_Tspn { get { return _STT_tspn; } set { _STT_tspn = DTU_ClampToMax(value); } }
        public ushort STT_Btob { get { return _STT_btob; } set { _STT_btob = DTU_ClampToMax(value); } }
        public ushort STT_Perf { get { return _STT_perf; } set { _STT_perf = DTU_ClampToMax(value); } }




        private void STT_Reset()
        {
            STT_Level = 0;
            STT_Score = 0;

            STT_Line = 0;
            STT_Comb = 0;
            STT_Tspn = 0;
            STT_Btob = 0;
            STT_Perf = 0;
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
            //Log($"GameStats", $"Level Up");
            STT_Level++;
        }


        private void STT_ApplySyncedData(ushort[] recordGameStats)
        {
            _STT_line = recordGameStats[0];
            _STT_comb = recordGameStats[1];
            _STT_tspn = recordGameStats[2];
            _STT_btob = recordGameStats[3];
            _STT_perf = recordGameStats[4];
        }
    }
}