using UnityEngine;

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    public static class Util
    {
        public static void Log(string message)
        {
            Debug.Log($"[<color=#ffbf00>UDON TET</color>] {message}");
        }


        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[<color=#ffbf00>UDON TET</color>] {message}");
        }


        public static void LogError(string message)
        {
            Debug.LogError($"[<color=#ffbf00>UDON TET</color>] {message}");
        }
    }
}
