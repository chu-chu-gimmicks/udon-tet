
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        private ushort DRS_ScoreDelta { get; set; } = 0;

        private byte  _DRS_line  = 0;
        private byte  _DRS_comb  = 0;
        private TSpinState _DRS_tspn = TSpinState.None;
        private byte  _DRS_btob  = 0;
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

        private byte DRS_Comb
        {
            get { return _DRS_comb; }
            set
            {
                _DRS_comb = value;
                if (_DRS_comb > STT_Comb) { STT_Comb = _DRS_comb; }
            }
        }

        private TSpinState DRS_Tspn
        {
            get { return _DRS_tspn; }
            set
            {
                _DRS_tspn = value;
                if (_DRS_tspn != TSpinState.None) { STT_Tspn++; }
            }
        }

        private byte DRS_Btob
        {
            get { return _DRS_btob; }
            set
            {
                _DRS_btob = value;
                if (_DRS_btob > STT_Btob) { STT_Btob = _DRS_btob; }
            }
        }

        private byte DRS_Block
        {
            get { return _DRS_block; }
            set
            {
                _DRS_block = value;
                if (_DRS_block == 0) { STT_Perf++; }
            }
        }




        private void DRS_Reset()
        {
            DRS_ScoreDelta = 0;

            _DRS_line  = 0;
            _DRS_comb  = 0;
            _DRS_tspn  = TSpinState.None;
            _DRS_btob  = 0;
            _DRS_block = 0;
        }


        private void DRS_CalculateScoreDelta()
        {
            DRS_ScoreDelta = 0;

            if (DRS_Line == 0) { return; }

            int score = 0;

            if (DRS_Tspn == TSpinState.Normal)
            {
                switch (DRS_Line)
                {
                    case 1: score = 40; break;
                    case 2: score = 80; break;
                    case 3: score = 120; break;
                }
            }
            else if (DRS_Tspn == TSpinState.Mini)
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

            if (DRS_Line == 4 || DRS_Tspn != TSpinState.None)
            {
                if (DRS_Btob == 2) { score += DRS_Line * 10; }
                else if (DRS_Btob > 2) { score += DRS_Line * 20; }
            }

            score += (DRS_Comb - 1) * 20;

            if (DRS_Block == 0) { score += 500; }

            // レベルに応じてボーナス（最高7倍）
            int multiplier = 10 + STT_Level * 2;
            score = (int)(score * multiplier * 0.1f);

            DRS_ScoreDelta = DTU_ClampToMax((ushort)score);
        }


        private void DRS_AddScoreDelta()
        {
            STT_Score += DRS_ScoreDelta;
            DRS_ScoreDelta = 0;
        }


        private void DRS_ApplySyncedData(byte[] recordDropResults)
        {
            _DRS_line  = SYD_recordDropResults[0];
            _DRS_comb  = SYD_recordDropResults[1];
            _DRS_tspn  = (TSpinState)SYD_recordDropResults[2];
            _DRS_btob  = SYD_recordDropResults[3];
            _DRS_block = SYD_recordDropResults[4];
        }
    }
}