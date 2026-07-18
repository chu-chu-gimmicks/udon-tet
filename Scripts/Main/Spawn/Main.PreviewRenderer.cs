
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private const int _PREVIEW_WIDTH = 4, _PREVIEW_HEIGHT = 2;
        private readonly Vector2Int _PVR_INITIAL_HOLD_POS  = new Vector2Int(1, 0);
        private readonly Vector2Int _PVR_INITIAL_QUEUE_POS = new Vector2Int(1, 0);

        private Vector2Int[] _PVR_minoBuffer = new Vector2Int[4];




        private void PVR_Reset()
        {
            for (byte i = 0; i < holdMinoRenderers.Length; i++)
            {
                holdMinoRenderers[i].gameObject.SetActive(false);
            }

            for (byte i = 0; i < nextMinoRenderers.Length; i++)
            {
                nextMinoRenderers[i].gameObject.SetActive(false);
            }
        }


        private void PVR_Start()
        {
            for (byte i = 0; i < holdMinoRenderers.Length; i++)
            {
                holdMinoRenderers[i].gameObject.SetActive(true);
            }
            _PVR_HideAllHoldBlock();

            for (byte i = 0; i < nextMinoRenderers.Length; i++)
            {
                nextMinoRenderers[i].gameObject.SetActive(true);
            }
            _PVR_HideAllQueueBlock();
        }


        private void PVR_ShowHoldMino(MinoType minoType)
        {
            _PVR_HideAllHoldBlock();

            MSH_GetMino(_PVR_minoBuffer, minoType);
            for (int i = 0; i < _PVR_minoBuffer.Length; i++)
            {
                _PVR_ShowHoldBlock(_PVR_minoBuffer[i], minoType);
            }
        }


        private void _PVR_ShowHoldBlock(Vector2Int pos, MinoType minoType)
        {
            byte idx = GetPreviewIndex(pos.x + _PVR_INITIAL_HOLD_POS.x, pos.y + _PVR_INITIAL_HOLD_POS.y);
            holdMinoRenderers[idx].sharedMaterial = minoMaterials[(int)minoType];
        }


        private void _PVR_HideAllHoldBlock()
        {
            for (byte i = 0; i < holdMinoRenderers.Length; i++)
            {
                holdMinoRenderers[i].sharedMaterial = minoMaterials[(int)MinoType.None];
            }
        }


        private void PVR_ShowQueue(MinoType[] minoQueue)
        {
            _PVR_HideAllQueueBlock();

            for (int i = 0; i < minoQueue.Length; i++)
            {
                MinoType minoType = minoQueue[i];
                MSH_GetMino(_PVR_minoBuffer, minoType);
                for (int j = 0; j < _PVR_minoBuffer.Length; j++)
                {
                    _PVR_ShowQueueBlock(_PVR_minoBuffer[j], minoType, i);
                }
            }
        }


        private void _PVR_ShowQueueBlock(Vector2Int pos, MinoType minoType, int index)
        {
            byte idx = GetPreviewIndex(pos.x + _PVR_INITIAL_QUEUE_POS.x, pos.y + _PVR_INITIAL_QUEUE_POS.y + _PREVIEW_HEIGHT * index);
            nextMinoRenderers[idx].sharedMaterial = minoMaterials[(int)minoType];
        }


        private void _PVR_HideAllQueueBlock()
        {
            for (byte i = 0; i < nextMinoRenderers.Length; i++)
            {
                nextMinoRenderers[i].sharedMaterial = minoMaterials[(int)MinoType.None];
            }
        }


        private byte GetPreviewIndex(int x, int y)
        {
            return (byte)(x + y * _PREVIEW_WIDTH);
        }
    }
}