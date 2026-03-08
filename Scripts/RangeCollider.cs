
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RangeCollider : UdonSharpBehaviour
    {
        private const float INTERVAL = 2.0f;
        private const float HEIGHT_OFFSET = 0.1f;

        [SerializeField] private InGame inGameManager;

        [SerializeField] private GameObject targetCollider;

        private bool isPending = false;




        private void OnEnable()
        {
            if (Utilities.IsValid(targetCollider))
            {
                if (targetCollider.activeInHierarchy)
                {
                    targetCollider.SetActive(false);
                }

                if (isPending) { return; }
                SendCustomEventDelayedSeconds(nameof(JudgeByCollider), INTERVAL);
                isPending = true;
            }
        }


        public void JudgeByCollider()
        {
            if (this.enabled)
            {
                SendCustomEventDelayedSeconds(nameof(JudgeByCollider), INTERVAL);
            }
            else
            {
                isPending = false;
                return;
            }

            bool isinCollider = false;

            Vector3 colliderPos    = targetCollider.transform.position;
            Quaternion colliderRot = targetCollider.transform.rotation;
            Vector3 colliderSize   = targetCollider.transform.localScale;

            Vector3 playerPos = Networking.LocalPlayer.GetPosition();

            // ワールド空間のプレイヤー座標をコライダーのローカル空間に変換
            Vector3 playerPosInLocal = Quaternion.Inverse(colliderRot) * (playerPos - colliderPos);

            isinCollider =
                Mathf.Abs(playerPosInLocal.x) <= colliderSize.x * 0.5f &&
                Mathf.Abs(playerPosInLocal.y + HEIGHT_OFFSET) <= colliderSize.y * 0.5f &&
                Mathf.Abs(playerPosInLocal.z) <= colliderSize.z * 0.5f;

            inGameManager.IsInCollider = isinCollider;
        }
    }
}