
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UserDataAccessor : UdonSharpBehaviour
    {
#if UNITY_EDITOR && !COMPILER_UDONSHARP
        public static class FieldNames
        {
            public const string Reference = nameof(_reference);
        }
#endif




        [SerializeField] private ChuChuGimmicks.UDONTET.Runtime.UserData _reference;

        private ChuChuGimmicks.UDONTET.Runtime.UserData _userData;
        public bool _isRestored = false;

        private Manager _manager;




        public void Subscribe(Manager manager)
        {
            _manager = manager;
        }


        public override void OnPlayerRestored(VRCPlayerApi player)
        {
            if (!player.isLocal) { return; }

            _isRestored = FindUserData();

            if (_isRestored && Utilities.IsValid(_manager))
            {
                _manager.OnUserDataRestored();
            }
        }


        private bool FindUserData()
        {
            _userData = (ChuChuGimmicks.UDONTET.Runtime.UserData)Networking.FindComponentInPlayerObjects(Networking.LocalPlayer, _reference);

            if (Utilities.IsValid(_userData))
            {
                Debug.Log($"[<color=#ffbf00>UDON TET/UserDataAccessor</color>] UserData restored");
                return true;
            }
            else
            {
                Debug.LogWarning($"[<color=#ffbf00>UDON TET/UserDataAccessor</color>] UserData not found");
                return false;
            }
        }


        public void GetData(out uint score, out uint level, out uint count)
        {
            if (_isRestored)
            {
                _userData.GetData(out score, out level,out count);
            }
            else
            {
                score = 0;
                level = 0;
                count = 0;
            }
        }


        public void SetData(uint score, uint level, uint count)
        {
            if (_isRestored)
            {
                _userData.SetData(score, level, count);
            }
        }
    }
}
