
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
//using UCS;

namespace ChuChuGimmicks.UDONTET
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class GameContext : UdonSharpBehaviour
    {
        [Header("ON: Reflect synced data only in the collider.\nOFF: Reflect synced data anywhere.")]
        [SerializeField] public bool reflectOnlyInCollider = true;

        //[Space]

        //[Header("Score × rate = UdonChips")]
        //[SerializeField] public UdonChips udonChips;
        //[SerializeField] public float rate = 0.1f;

        [Space(80)]
        [Header("---- Don't Touch ----")]
        [SerializeField] public InGame inGameManager;




        //public void SetUdonChips(int income)
        //{
        //    udonChips.money += income * rate;
        //}
    }
}