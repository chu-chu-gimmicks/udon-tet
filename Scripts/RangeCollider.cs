
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
        private const float EPSILON = 0.01f;

        [SerializeField] private InGame inGameManager;

        [SerializeField] private GameObject targetCollider;

        private float timer = float.MinValue;




        private void OnEnable()
        {
            if (Utilities.IsValid(targetCollider))
            {
                if (targetCollider.activeInHierarchy)
                {
                    targetCollider.SetActive(false);
                }

                SendCustomEventDelayedSeconds(nameof(JudgeByCollider), INTERVAL);
                timer = Time.time - INTERVAL / 2.0f;
            }
        }


        private void OnDisable()
        {
            timer = float.MaxValue;
        }


        public void JudgeByCollider()
        {
            if (Time.time + EPSILON < timer + INTERVAL) { return; }
            SendCustomEventDelayedSeconds(nameof(JudgeByCollider), INTERVAL);
            timer = Time.time;

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