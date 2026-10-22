
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ChuChuGimmicks.UDONTET.Runtime
{
    public partial class Main
    {
        private void CopyMino(Vector2Int[] source, Vector2Int[] target)
        {
            target[0] = source[0];
            target[1] = source[1];
            target[2] = source[2];
            target[3] = source[3];
        }


        private byte GetIndex(int x, int y)
        {
            return (byte)(x + y * WIDTH);
        }


        private void Log(string className, string message)
        {
            Debug.Log($"[<color=#ffbf00>UDON TET/{className}</color>] {message}");
        }
    }
}
