
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        private const float RCL_HEIGHT_OFFSET = 0.1f;

        Vector3   _RCL_colliderPosition = Vector3.zero;
        float     _RCL_colliderRadiusSq = 0.0f;
        Matrix4x4 _RCL_colliderMatrix = Matrix4x4.identity;




        private void RCL_Reset()
        {
            if (Utilities.IsValid(_rangeCollider))
            {
                if (_rangeCollider.gameObject.activeSelf)
                {
                    _rangeCollider.gameObject.SetActive(false);
                }

                _RCL_colliderPosition = _rangeCollider.position;
                Vector3 s = _rangeCollider.localScale;
                _RCL_colliderRadiusSq = (s.x * s.x + s.y * s.y + s.z * s.z) * 0.25f;
                _RCL_colliderMatrix = _rangeCollider.worldToLocalMatrix;
            }
        }


        private bool RCL_IsInCollider()
        {
            Vector3 playerPos = Networking.LocalPlayer.GetPosition();
            playerPos.y += RCL_HEIGHT_OFFSET;

            Vector3 posDelta = playerPos - _RCL_colliderPosition;
            if (posDelta.sqrMagnitude > _RCL_colliderRadiusSq) { return false; }

            // ワールド空間のプレイヤー座標をコライダーのローカル空間に変換
            Vector3 playerPosInLocal = _RCL_colliderMatrix.MultiplyPoint3x4(playerPos);

            return
                Mathf.Abs(playerPosInLocal.x) <= 0.5f &&
                Mathf.Abs(playerPosInLocal.y) <= 0.5f &&
                Mathf.Abs(playerPosInLocal.z) <= 0.5f;
        }
    }
}
