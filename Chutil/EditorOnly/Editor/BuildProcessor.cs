using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
using VRC.SDKBase.Editor.BuildPipeline;

namespace ChuChuGimmicks.UDONTET.Chutil
{
    internal class BuildProcessor : IVRCSDKBuildRequestedCallback, IProcessSceneWithReport
    {
        public int callbackOrder => 0;




        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            DebugRat.Log($"OnBuildRequested: begin (buildType={requestedBuildType})");

            if (requestedBuildType != VRCSDKRequestedBuildType.Scene) { return false; }

            bool canBuild = true;
            var scene = SceneManager.GetActiveScene();

            canBuild &= EnsureActivePass.OnBuildStarted(scene);
            canBuild &= EnsureUniquePass.OnBuildStarted(scene);

            DebugRat.Log($"OnBuildRequested: end (scene={scene.name}, buildType={requestedBuildType}, canBuild={canBuild}");
            return canBuild;
        }


        public void OnProcessScene(Scene scene, BuildReport report)
        {
            DebugRat.Log($"OnProcessScene: begin (scene={scene.name})");
            
            // OnBuildRequested は Play Mode では発火しないので補完
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                DebugRat.Log($"In play mode");
                bool canBuild = true;
                canBuild &= EnsureActivePass.OnBuildStarted(scene);
                canBuild &= EnsureUniquePass.OnBuildStarted(scene);
                if (!canBuild) { return; }
            }

            EnsureActivePass.OnProcessScene(scene);
            EnsureUniquePass.OnProcessScene(scene);

            DebugRat.Log($"OnProcessScene: end (scene={scene.name})");
        }
    }
}
