
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private readonly Vector3 _GRR_defaultScale = Vector3.one;
        private readonly Vector3 _GRR_ghostScale = new Vector3(0.5f, 0.5f, 0.5f);

        private Vector2Int[] _GRR_ghostMino = new Vector2Int[4];

        private Vector2Int[] _GRR_minoBuffer = new Vector2Int[4];




        private void GRR_Reset()
        {
            for (byte i = 0; i < gridRenderers.Length; i++)
            {
                gridRenderers[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _GRR_ghostMino.Length; i++)
            {
                _GRR_ghostMino[i] = new Vector2Int(0, 0);
            }
        }


        private void GRR_Start()
        {
            for (byte i = 0; i < gridRenderers.Length; i++)
            {
                gridRenderers[i].gameObject.SetActive(true);
                GRR_HideBlock(i);
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
            gridRenderers[idx].sharedMaterial = minoMaterials[(int)minoType];
            gridTransforms[idx].localScale = isGhost ? _GRR_ghostScale : _GRR_defaultScale;
        }


        private void GRR_HideBlock(byte idx)
        {
            gridRenderers[idx].sharedMaterial = minoMaterials[(int)MinoType.None];
            gridTransforms[idx].localScale = _GRR_defaultScale;
        }
    }
}