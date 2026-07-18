
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    public partial class Main
    {
        private const float _RAC_HEIGHT_OFFSET = 0.1f;

        Vector3   _RAC_colliderPosition = Vector3.zero;
        float     _RAC_colliderRadiusSq = 0.0f;
        Matrix4x4 _RAC_colliderMatrix = Matrix4x4.identity;




        private void RAC_Reset()
        {
            if (Utilities.IsValid(rangeCollider))
            {
                if (rangeCollider.gameObject.activeSelf)
                {
                    rangeCollider.gameObject.SetActive(false);
                }

                _RAC_colliderPosition = rangeCollider.position;
                Vector3 s = rangeCollider.localScale;
                _RAC_colliderRadiusSq = (s.x * s.x + s.y * s.y + s.z * s.z) * 0.25f;
                _RAC_colliderMatrix = rangeCollider.worldToLocalMatrix;
            }
        }


        private bool RAC_IsInCollider()
        {
            Vector3 playerPos = Networking.LocalPlayer.GetPosition();
            playerPos.y += _RAC_HEIGHT_OFFSET;

            Vector3 posDelta = playerPos - _RAC_colliderPosition;
            if (posDelta.sqrMagnitude > _RAC_colliderRadiusSq) { return false; }

            // ワールド空間のプレイヤー座標をコライダーのローカル空間に変換
            Vector3 playerPosInLocal = _RAC_colliderMatrix.MultiplyPoint3x4(playerPos);

            return
                Mathf.Abs(playerPosInLocal.x) <= 0.5f &&
                Mathf.Abs(playerPosInLocal.y) <= 0.5f &&
                Mathf.Abs(playerPosInLocal.z) <= 0.5f;
        }
    }
}
