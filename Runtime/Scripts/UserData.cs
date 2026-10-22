
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UserData : UdonSharpBehaviour
    {
        private const uint MAX_VALUE = 99_999_999;

        [UdonSynced] private uint _yourHighScore = 0;
        [UdonSynced] private uint _yourPeakLevel = 0;
        [UdonSynced] private uint _yourPlayCount = 0;




        public void GetData(out uint score, out uint level, out uint count)
        {
            score = _yourHighScore;
            level = _yourPeakLevel;
            count = _yourPlayCount;
        }


        public void SetData(uint score, uint level, uint count)
        {
            _yourHighScore = SetValue(_yourHighScore, score, out bool hasChanged0);
            _yourPeakLevel = SetValue(_yourPeakLevel, level, out bool hasChanged1);
            _yourPlayCount = SetValue(_yourPlayCount, count, out bool hasChanged2);

            if (hasChanged0 || hasChanged1 || hasChanged2)
            {
                RequestSerialization();
            }
        }


        private uint SetValue(uint oldValue, uint newValue, out bool hasChanged)
        {
            newValue = newValue > MAX_VALUE ? MAX_VALUE : newValue;
            if (newValue > oldValue)
            {
                hasChanged = true;
                return newValue;
            }
            hasChanged = false;
            return oldValue;
        }
    }
}
