
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UDONTETAuthoring : UdonSharpBehaviour
    {
        [SerializeField] public UdonSharpBehaviour udonChips;
        [SerializeField] public float udonChipsRate = 0.1f;

        [SerializeField] public Main main;
    }
}
