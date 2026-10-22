using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChuChuGimmicks.UDONTET.Chutil
{
    public static class EnsureUniquePass
    {
        public static bool OnBuildStarted(Scene scene)
        {
            ChutilEnsureUnique.Rebuild();
            bool canBuild = ChutilEnsureUnique.IsAllKeyUnique();

            if (!canBuild)
            {
                EnsureUniqueWindow.ShowWindow();
            }

            return canBuild;
        }


        public static void OnProcessScene(Scene scene)
        {
        }
    }


    public class EnsureUniqueWindow : EditorWindow
    {
        private Dictionary<string, HashSet<ChutilEnsureUnique>> _allInstances;




        public static void ShowWindow()
        {
            const float w = 400f;
            const float h = 450f;

            Rect main = EditorGUIUtility.GetMainWindowPosition();

            Rect rect = new Rect(
                main.center.x - w * 0.5f,
                main.center.y - h * 0.5f,
                w,
                h
            );

            var window = GetWindow<EnsureUniqueWindow>("Chutil Ensure Unique", true);
            window.position = rect;
            window.minSize = new Vector2(w, h);

            window.Initialize();
        }


        private void Initialize()
        {
            _allInstances = ChutilEnsureUnique.GetAllInstances();
        }


        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "Duplicate Key detected.\n" +
                "Only one object with the same Key can exist in the scene.\n" +
                "This placement may not be intended by the asset author.\n" +
                "Resolve the duplicate before building.\n" +
                "\n" +
                "Keyが重複しています。\n" +
                "同じKeyを持つオブジェクトは、シーン内に1つだけしか配置できません。\n" +
                "アセット制作者が意図しない配置の可能性があります。\n" +
                "重複を解消してからビルドしてください。",
                MessageType.Error
            );

            int displayCount = 0;
            foreach (var instances in _allInstances.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                if (instances.Value.Count(x => x != null) <= 1) { continue; }
                EditorGUILayout.LabelField($"{instances.Key}");
                EditorGUI.indentLevel++;
                foreach (var instance in instances.Value)
                {
                    if (instance == null) { continue; }
                    EditorGUILayout.ObjectField(instance.gameObject, typeof(GameObject), true);
                    if (displayCount < int.MaxValue)
                    {
                        displayCount++;
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            if (displayCount == 0)
            {
                Close();
                GUIUtility.ExitGUI();
            }
        }
    }
}
