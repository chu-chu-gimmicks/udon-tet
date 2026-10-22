using System;
using UnityEngine.SceneManagement;

namespace ChuChuGimmicks.UDONTET.Chutil
{
    internal static class EnsureActivePass
    {
        public static bool OnBuildStarted(Scene scene)
        {
            return true;
        }


        public static void OnProcessScene(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var components = root.GetComponentsInChildren<ChutilEnsureActive>(true);
                Array.Sort(
                    components,
                    (a, b) => a.GetDepth().CompareTo(b.GetDepth())
                );

                for (int i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    if (component == null) { continue; }
                    var gameObject = component.gameObject;
                    if (gameObject == null) { continue; }
                    var transform = component.transform;
                    if (transform == null) { continue; }

                    transform.SetParent(null, true);
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
