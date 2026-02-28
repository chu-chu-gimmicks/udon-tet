using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        [UdonSynced] private uint Sync_compressedMinos = 0;
        [UdonSynced] private ulong[] Sync_compressedGrid = new ulong[10];
        [UdonSynced] private ushort Sync_compressedMinoQueue = ushort.MaxValue;

        [UdonSynced] private int Sync_playId = 0;

        [UdonSynced] private byte Sync_level = 0;
        [UdonSynced] private int  Sync_score = 0;
        [UdonSynced] private ushort Sync_scoreDelta = 0;

        [UdonSynced] private byte Sync_line  = 0;
        [UdonSynced] private byte Sync_combo = 0;
        [UdonSynced] private byte Sync_tSpin = 0;
        [UdonSynced] private byte Sync_bTB   = 0;
        [UdonSynced] private byte Sync_block = 0;

        [UdonSynced] private ushort Sync_lineStat    = 0;
        [UdonSynced] private byte   Sync_comboStat   = 0;
        [UdonSynced] private ushort Sync_tSpinStat   = 0;
        [UdonSynced] private byte   Sync_bTBStat     = 0;
        [UdonSynced] private byte   Sync_perfectStat = 0;




        private void SYD_Reset()
        {
            Sync_compressedMinos = 0;
            for (int i = 0; i < Sync_compressedGrid.Length; i++)
            {
                Sync_compressedGrid[i] = 0;
            }
            Sync_compressedMinoQueue = ushort.MaxValue;

            Sync_level = 0;
            Sync_score = 0;
            Sync_scoreDelta = 0;

            Sync_line  = 0;
            Sync_combo = 0;
            Sync_tSpin = 0;
            Sync_bTB   = 0;
            Sync_block = 0;

            Sync_lineStat    = 0;
            Sync_comboStat   = 0;
            Sync_tSpinStat   = 0;
            Sync_bTBStat     = 0;
            Sync_perfectStat = 0;
        }
    }
}
