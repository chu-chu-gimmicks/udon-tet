
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UpdateHandler : UdonSharpBehaviour
    {
        [SerializeField] private Main gameManager;




        private void Update()
        {
            gameManager.GLP_Update();
        }
    }
}
