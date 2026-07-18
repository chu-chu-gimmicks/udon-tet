
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private Vector2Int[] currMinoPos = new Vector2Int[4];
        private MinoType CurrMinoType { get; set; } = MinoType.None;
        private int _currMinoAngle = 0;
        private int CurrMinoAngle
        {
            get { return _currMinoAngle; }
            set { _currMinoAngle = (value / 360 + 360) / 360; }
        }




        private void MINO_Reset()
        {
            for (int i = 0; i < currMinoPos.Length; i++)
            {
                currMinoPos[i] = Vector2Int.zero;
            }
            CurrMinoType = MinoType.None;
            CurrMinoAngle = 0;
        }
    }
}