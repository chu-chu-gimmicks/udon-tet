
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class InGame
    {
        private readonly Vector3 _GR_defaultScale = Vector3.one;
        private readonly Vector3 _GR_ghostScale = new Vector3(0.5f, 0.5f, 0.5f);

        private Vector2Int[] _GR_ghostMino = new Vector2Int[4];

        private Vector2Int[] _GR_minoBuffer = new Vector2Int[4];




        private void GR_Reset()
        {
            for (byte i = 0; i < gridRenderers.Length; i++)
            {
                GR_HideBlock(i);
            }

            for (int i = 0; i < _GR_ghostMino.Length; i++)
            {
                _GR_ghostMino[i] = new Vector2Int(0, 0);
            }
        }


        private void GR_ShowMino(Vector2Int[] minoPos, MinoType minoType)
        {
            _GR_ShowGhostMino(minoPos, minoType);

            for (int i = 0; i < minoPos.Length; i++)
            {
                byte idx = GetIndex(minoPos[i].x, minoPos[i].y);
                GR_ShowBlock(idx, minoType);
            }
        }


        private void _GR_ShowGhostMino(Vector2Int[] minoPos, MinoType minoType)
        {
            CopyMino(minoPos, _GR_minoBuffer);

            while (G_CanMoveDown(_GR_minoBuffer))
            {
                for (int i = 0; i < _GR_minoBuffer.Length; i++)
                {
                    _GR_minoBuffer[i].y--;
                }
            }

            for (int i = 0; i < _GR_minoBuffer.Length; i++)
            {
                byte idx = GetIndex(_GR_minoBuffer[i].x, _GR_minoBuffer[i].y);
                GR_ShowBlock(idx, minoType, isGhost: true);
            }

            CopyMino(_GR_minoBuffer, _GR_ghostMino);
        }


        private void GR_HideMino(Vector2Int[] minoPos)
        {
            for (int i = 0; i < minoPos.Length; i++)
            {
                byte idx = GetIndex(minoPos[i].x, minoPos[i].y);
                GR_HideBlock(idx);
            }

            _GR_HideGhostMino();
        }


        private void _GR_HideGhostMino()
        {
            for (int i = 0; i < _GR_ghostMino.Length; i++)
            {
                byte idx = GetIndex(_GR_ghostMino[i].x, _GR_ghostMino[i].y);
                GR_HideBlock(idx);
            }
        }


        private void GR_ShowBlock(byte idx, MinoType minoType, bool isGhost = false)
        {
            if (!_GR_IsIndexSafe(idx)) { return; }
            if (!_GR_IsMinoTypeSafe(minoType)) { return; }

            gridRenderers[idx].enabled = true;
            gridRenderers[idx].sharedMaterial = minoMaterials[(int)minoType];

            if (isGhost) { gridTransforms[idx].localScale = _GR_ghostScale; }
            else { gridTransforms[idx].localScale = _GR_defaultScale; }
        }


        private void GR_HideBlock(byte idx)
        {
            if (!_GR_IsIndexSafe(idx)) { return; }

            gridRenderers[idx].enabled = false;
            gridTransforms[idx].localScale = _GR_defaultScale;
        }


        private bool _GR_IsIndexSafe(byte idx)
        {
            return idx >= 0 && idx < gridRenderers.Length;
        }


        private bool _GR_IsMinoTypeSafe(MinoType minoType)
        {
            return (int)minoType >= 0 && (int)minoType < minoMaterials.Length;
        }
    }
}