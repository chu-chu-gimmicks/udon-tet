using UnityEngine;

namespace ChuChuGimmicks.UDONTET.Chutil
{
    internal static class DebugRat
    {
        public static void Log(string message)
        {
            Debug.Log($"[<color=#ffbf00>ChuChuGimmicks</color>] {message}");
        }
        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[<color=#ffbf00>ChuChuGimmicks</color>] {message}");
        }
        public static void LogError(string message)
        {
            Debug.LogError($"[<color=#ffbf00>ChuChuGimmicks</color>] {message}");
        }
    }
}
