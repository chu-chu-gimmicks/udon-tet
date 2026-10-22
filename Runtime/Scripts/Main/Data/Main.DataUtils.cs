
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        private const ushort DTU_MAX_4DIGIT_VALUE = 9_999;
        private const uint DTU_MAX_8DIGIT_VALUE = 99_999_999;




        private uint DTU_ClampToMax(uint value)
        {
            value = value > DTU_MAX_8DIGIT_VALUE ? DTU_MAX_8DIGIT_VALUE : value;
            return value;
        }


        private ushort DTU_ClampToMax(ushort value)
        {
            value = value > DTU_MAX_4DIGIT_VALUE ? DTU_MAX_4DIGIT_VALUE : value;
            return value;
        }
    }
}
