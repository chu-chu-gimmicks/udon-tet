
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UpdateHandler : UdonSharpBehaviour
    {
        [SerializeField] private InGame gameManager;




        private void Update()
        {
            gameManager.GL_Update();
        }


        private void LateUpdate()
        {
            gameManager.GL_LateUpdate();
        }
    }
}
