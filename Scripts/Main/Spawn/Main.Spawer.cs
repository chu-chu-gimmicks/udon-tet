
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private readonly Vector2Int _SPN_INITIAL_POS = new Vector2Int(4, 18);

        private MinoType[] SPN_minoQueue = new MinoType[5];

        private byte[] _SPN_randomBox = new byte[7];
        private int _SPN_remaining = 0;

        private Vector2Int[] _SPN_minoBuffer = new Vector2Int[4];




        private void SPN_Reset()
        {
            _SPN_InitRandomBox();
            _SPN_InitMinoQueue();
        }


        private MinoType SPN_SpawnMino(Vector2Int[] minoPos)
        {
            MinoType nextMinoType = SPN_minoQueue[0];

            SPN_SetMino(minoPos, nextMinoType);

            _SPN_UpdateMinoQueue();
            PVR_ShowQueue(SPN_minoQueue);

            return nextMinoType;
        }


        private void SPN_SetMino(Vector2Int[] minoPos, MinoType minoType)
        {
            MSH_GetMino(minoPos, minoType);
            for (int i = 0; i < minoPos.Length; i++)
            {
                minoPos[i] += _SPN_INITIAL_POS;
            }

            //初期位置に既にブロックがあったら、初期位置を上にずらす
            while (GRD_IsMinoOverlapped(minoPos))
            {
                for (int i = 0; i < minoPos.Length; i++)
                {
                    minoPos[i] += Vector2Int.up;
                }
            }
        }


        private void _SPN_InitRandomBox()
        {
            for (byte i = 0; i < _SPN_randomBox.Length; i++)
            {
                _SPN_randomBox[i] = (byte)(i + 1);
            }
            _SPN_remaining = _SPN_randomBox.Length;
        }


        private void _SPN_InitMinoQueue()
        {
            for (int i = 0; i < SPN_minoQueue.Length; i++)
            {
                SPN_minoQueue[i] = _SPN_GetMinoType();
            }
        }


        private void _SPN_UpdateMinoQueue()
        {
            for (int i = 0; i < SPN_minoQueue.Length - 1; i++)
            {
                SPN_minoQueue[i] = SPN_minoQueue[i + 1];
            }
            SPN_minoQueue[SPN_minoQueue.Length - 1] = _SPN_GetMinoType();
        }


        private MinoType _SPN_GetMinoType()
        {
            int randomIndex = 0;
            byte randomNum = 0;

            if (_SPN_remaining == 0)
            {
                _SPN_InitRandomBox();
            }

            randomIndex = Random.Range(0, _SPN_remaining);
            randomNum = _SPN_randomBox[randomIndex];

            _SPN_randomBox[randomIndex] = _SPN_randomBox[_SPN_remaining - 1];
            _SPN_remaining--;

            return (MinoType)randomNum;
        }
    }
}
