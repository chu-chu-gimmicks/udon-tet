using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        [UdonSynced] private byte SYD_gameState = 0;
        [UdonSynced] private uint SYD_compressedMinos = 0;
        [UdonSynced] private ulong[] SYD_compressedGrid = new ulong[10];
        [UdonSynced] private ushort SYD_compressedMinoQueue = ushort.MaxValue;

        [UdonSynced] private int SYD_playId = 0;

        [UdonSynced] private byte SYD_level = 0;
        [UdonSynced] private int  SYD_score = 0;
        [UdonSynced] private ushort SYD_scoreDelta = 0;

        [UdonSynced] private byte SYD_line  = 0;
        [UdonSynced] private byte SYD_combo = 0;
        [UdonSynced] private byte SYD_tSpin = 0;
        [UdonSynced] private byte SYD_bTB   = 0;
        [UdonSynced] private byte SYD_block = 0;

        [UdonSynced] private ushort SYD_lineStat    = 0;
        [UdonSynced] private byte   SYD_comboStat   = 0;
        [UdonSynced] private ushort SYD_tSpinStat   = 0;
        [UdonSynced] private byte   SYD_bTBStat     = 0;
        [UdonSynced] private byte   SYD_perfectStat = 0;




        private void SYD_Reset()
        {
            SYD_compressedMinos = 0;
            for (int i = 0; i < SYD_compressedGrid.Length; i++)
            {
                SYD_compressedGrid[i] = 0;
            }
            SYD_compressedMinoQueue = ushort.MaxValue;

            SYD_level = 0;
            SYD_score = 0;
            SYD_scoreDelta = 0;

            SYD_line  = 0;
            SYD_combo = 0;
            SYD_tSpin = 0;
            SYD_bTB   = 0;
            SYD_block = 0;

            SYD_lineStat    = 0;
            SYD_comboStat   = 0;
            SYD_tSpinStat   = 0;
            SYD_bTBStat     = 0;
            SYD_perfectStat = 0;
        }
    }
}
