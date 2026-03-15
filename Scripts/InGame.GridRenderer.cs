
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private readonly Vector3 _GRR_defaultScale = Vector3.one;
        private readonly Vector3 _GRR_ghostScale = new Vector3(0.5f, 0.5f, 0.5f);

        private Vector2Int[] _GRR_ghostMino = new Vector2Int[4];

        private Vector2Int[] _GRR_minoBuffer = new Vector2Int[4];




        private void GRR_Reset()
        {
            for (byte i = 0; i < gridRenderers.Length; i++)
            {
                GRR_HideBlock(i);
            }

            for (int i = 0; i < _GRR_ghostMino.Length; i++)
            {
                _GRR_ghostMino[i] = new Vector2Int(0, 0);
            }
        }


        private void GRR_ShowMino(Vector2Int[] minoPos, MinoType minoType)
        {
            _GRR_ShowGhostMino(minoPos, minoType);

            for (int i = 0; i < minoPos.Length; i++)
            {
                byte idx = GetIndex(minoPos[i].x, minoPos[i].y);
                GRR_ShowBlock(idx, minoType);
            }
        }


        private void _GRR_ShowGhostMino(Vector2Int[] minoPos, MinoType minoType)
        {
            CopyMino(minoPos, _GRR_minoBuffer);

            while (GRD_CanMoveDown(_GRR_minoBuffer))
            {
                for (int i = 0; i < _GRR_minoBuffer.Length; i++)
                {
                    _GRR_minoBuffer[i].y--;
                }
            }

            for (int i = 0; i < _GRR_minoBuffer.Length; i++)
            {
                byte idx = GetIndex(_GRR_minoBuffer[i].x, _GRR_minoBuffer[i].y);
                GRR_ShowBlock(idx, minoType, isGhost: true);
            }

            CopyMino(_GRR_minoBuffer, _GRR_ghostMino);
        }


        private void GRR_HideMino(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                byte idx = GetIndex(minoPos[i].x, minoPos[i].y);
                GRR_HideBlock(idx);
            }

            _GRR_HideGhostMino();
        }


        private void _GRR_HideGhostMino()
        {
            for (int i = 0; i < _GRR_ghostMino.Length; i++)
            {
                byte idx = GetIndex(_GRR_ghostMino[i].x, _GRR_ghostMino[i].y);
                GRR_HideBlock(idx);
            }
        }


        private void GRR_ShowBlock(byte idx, MinoType minoType, bool isGhost = false)
        {
            if (!_GRR_IsIndexSafe(idx)) { return; }
            if (!_GRR_IsMinoTypeSafe(minoType)) { return; }

            if (!gridRenderers[idx].enabled)
            {
                gridRenderers[idx].enabled = true;
            }

            if (gridRenderers[idx].sharedMaterial != minoMaterials[(int)minoType])
            {
                gridRenderers[idx].sharedMaterial = minoMaterials[(int)minoType];
            }

            Vector3 targetScale = isGhost ? _GRR_ghostScale : _GRR_defaultScale;
            if (gridTransforms[idx].localScale != targetScale)
            {
                gridTransforms[idx].localScale = targetScale;
            }
        }


        private void GRR_HideBlock(byte idx)
        {
            if (!_GRR_IsIndexSafe(idx)) { return; }

            if (gridRenderers[idx].enabled)
            {
                gridRenderers[idx].enabled = false;
            }

            if (gridTransforms[idx].localScale != _GRR_defaultScale)
            {
                gridTransforms[idx].localScale = _GRR_defaultScale;
            }
        }

        private bool _GRR_IsIndexSafe(byte idx)
        {
            return idx >= 0 && idx < gridRenderers.Length;
        }


        private bool _GRR_IsMinoTypeSafe(MinoType minoType)
        {
            return (int)minoType >= 0 && (int)minoType < minoMaterials.Length;
        }
    }
}