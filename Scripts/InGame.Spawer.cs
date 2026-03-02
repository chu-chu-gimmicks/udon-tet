
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private readonly Vector2Int _S_INITIAL_POS = new Vector2Int(4, 18);

        private MinoType[] S_minoQueue = new MinoType[5];

        private byte[] _S_randomBox = new byte[7];
        private int _S_remaining = 0;

        private Vector2Int[] _S_minoBuffer = new Vector2Int[4];




        private void S_Reset()
        {
            _S_InitRandomBox();
            _S_InitMinoQueue();
        }


        private MinoType S_SpawnMino(Vector2Int[] minoPos)
        {
            MinoType nextMinoType = S_minoQueue[0];
            if (nextMinoType == MinoType.None) { return nextMinoType; }

            S_SetMino(minoPos, nextMinoType);

            S_UpdateMinoQueue();
            PR_ShowQueue(S_minoQueue);

            return nextMinoType;
        }


        private void S_SetMino(Vector2Int[] minoPos, MinoType minoType)
        {
            MS_GetMino(minoPos, minoType);
            for (int i = 0; i < minoPos.Length; i++)
            {
                minoPos[i] += _S_INITIAL_POS;
            }

            //初期位置に既にブロックがあったら、初期位置を上にずらす
            while (G_IsMinoOverlapped(minoPos))
            {
                for (int i = 0; i < minoPos.Length; i++)
                {
                    minoPos[i] += Vector2Int.up;
                }
            }
        }


        private void _S_InitRandomBox()
        {
            for (byte i = 0; i < _S_randomBox.Length; i++)
            {
                _S_randomBox[i] = i;
            }
            _S_remaining = _S_randomBox.Length;
        }


        private void _S_InitMinoQueue()
        {
            for (int i = 0; i < S_minoQueue.Length; i++)
            {
                S_minoQueue[i] = _S_GetMinoType();
            }
        }


        private void S_UpdateMinoQueue()
        {
            // nextMinos���l�߂�
            for (int i = 0; i < S_minoQueue.Length - 1; i++)
            {
                S_minoQueue[i] = S_minoQueue[i + 1];
            }
            // �Ō�̗v�f�͐V���ɎZ�o
            S_minoQueue[S_minoQueue.Length - 1] = _S_GetMinoType();
        }


        private MinoType _S_GetMinoType()
        {
            int randomIndex = 0;
            byte randomNum = 0;

            // ���I�p�̔z�������������
            if (_S_remaining == 0)
            {
                _S_InitRandomBox();
            }

            // ���I����
            randomIndex = Random.Range(0, _S_remaining);
            randomNum = _S_randomBox[randomIndex];

            // �܂��I�΂�ĂȂ��v�f��O�Ɏ����Ă���
            _S_randomBox[randomIndex] = _S_randomBox[_S_remaining - 1];
            _S_remaining--;

            return (MinoType)randomNum;
        }
    }
}
