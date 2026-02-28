
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private const int _PREVIEW_WIDTH = 4, _PREVIEW_HEIGHT = 2;
        private readonly Vector2Int _PR_INITIAL_HOLD_POS  = new Vector2Int(1, 0);
        private readonly Vector2Int _PR_INITIAL_QUEUE_POS = new Vector2Int(1, 0);

        private Vector2Int[] _PR_minoBuffer = new Vector2Int[4];




        private void PR_Reset()
        {
            _PR_HideAllHoldBlock();
            _PR_HideAllQueueBlock();
        }


        private void PR_ShowHoldMino(MinoType minoType)
        {
            _PR_HideAllHoldBlock();

            if (minoType == MinoType.None) { return; } // 必要？

            MS_GetMino(_PR_minoBuffer, minoType);
            for (int i = 0; i < _PR_minoBuffer.Length; i++)
            {
                _PR_ShowHoldBlock(_PR_minoBuffer[i], minoType);
            }
        }


        private void _PR_ShowHoldBlock(Vector2Int pos, MinoType minoType)
        {
            byte idx = GetPreviewIndex(pos.x + _PR_INITIAL_HOLD_POS.x, pos.y + _PR_INITIAL_HOLD_POS.y);
            if (!_PR_IsIndexSafe(idx, holdMinoRenderers)) { return; }
            if (!_PR_IsMinoTypeSafe(minoType)) { return; }

            holdMinoRenderers[idx].enabled = true;
            holdMinoRenderers[idx].sharedMaterial = minoMaterials[(int)minoType];
        }


        private void _PR_HideAllHoldBlock()
        {
            for (byte i = 0; i < holdMinoRenderers.Length; i++)
            {
                holdMinoRenderers[i].enabled = false;
            }
        }


        private void PR_ShowQueue(MinoType[] minoQueue)
        {
            _PR_HideAllQueueBlock();

            for (int i = 0; i < minoQueue.Length; i++)
            {
                if (minoQueue[i] == MinoType.None) { continue; } // 必要？

                MS_GetMino(_PR_minoBuffer, minoQueue[i]);
                for (int j = 0; j < _PR_minoBuffer.Length; j++)
                {
                    _PR_ShowQueueBlock(_PR_minoBuffer[j], minoQueue[i], i);
                }
            }
        }


        private void _PR_ShowQueueBlock(Vector2Int pos, MinoType minoType, int index)
        {
            byte idx = GetPreviewIndex(pos.x + _PR_INITIAL_QUEUE_POS.x, pos.y + _PR_INITIAL_QUEUE_POS.y + _PREVIEW_HEIGHT * index);
            if (!_PR_IsIndexSafe(idx, nextMinoRenderers)) { return; }
            if (!_PR_IsMinoTypeSafe(minoType)) { return; }

            nextMinoRenderers[idx].enabled = true;
            nextMinoRenderers[idx].sharedMaterial = minoMaterials[(int)minoType];
        }


        private void _PR_HideAllQueueBlock()
        {
            for (byte i = 0; i < nextMinoRenderers.Length; i++)
            {
                nextMinoRenderers[i].enabled = false;
            }
        }


        private byte GetPreviewIndex(int x, int y)
        {
            return (byte)(x + y * _PREVIEW_WIDTH);
        }


        private bool _PR_IsIndexSafe(byte idx, Renderer[] renderers)
        {
            return idx >= 0 && idx < renderers.Length;
        }


        private bool _PR_IsMinoTypeSafe(MinoType minoType)
        {
            return (int)minoType >= 0 && (int)minoType < minoMaterials.Length;
        }
    }
}