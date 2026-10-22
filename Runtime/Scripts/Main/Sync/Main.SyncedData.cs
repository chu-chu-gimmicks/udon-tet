
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        [UdonSynced] private byte SYD_gameState = 0;
        [UdonSynced] private uint SYD_compressedMinos = 0;
        [UdonSynced] private ulong[] SYD_compressedGrid = new ulong[10];
        [UdonSynced] private ushort SYD_compressedMinoQueue = 0;

        [UdonSynced] private uint SYD_playId = 0;

        [UdonSynced] private ushort SYD_level = 0;
        [UdonSynced] private uint   SYD_score = 0;
        [UdonSynced] private ushort SYD_scoreDelta = 0;

        [UdonSynced] private ushort[] SYD_recordGameStats = new ushort[5];

        [UdonSynced] private byte[] SYD_recordDropResults = new byte[5];




        private void SYD_Reset()
        {
            SYD_compressedMinos = 0;
            for (int i = 0; i < SYD_compressedGrid.Length; i++)
            {
                SYD_compressedGrid[i] = 0;
            }
            SYD_compressedMinoQueue = 0;

            SYD_level = 0;
            SYD_score = 0;
            SYD_scoreDelta = 0;

            for (int i = 0; i < SYD_recordGameStats.Length; i++)
            {
                SYD_recordGameStats[i] = 0;
            }

            for (int i = 0;i < SYD_recordDropResults.Length; i++)
            {
                SYD_recordDropResults[i] = 0;
            }
        }
    }
}
