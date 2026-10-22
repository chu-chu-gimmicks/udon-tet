using System.Collections.Generic;
using UnityEngine;

namespace ChuChuGimmicks.UDONTET.Chutil
{
#if UNITY_EDITOR && !COMPILER_UDONSHARP
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class ChutilEnsureUnique : ChutilBaseComponent
    {
        private static readonly Dictionary<string, HashSet<ChutilEnsureUnique>> _instancesByKey = new Dictionary<string, HashSet<ChutilEnsureUnique>>();

        public const string KeyFieldName = nameof(_key);
        [SerializeField] private string _key = "Key";




        public static void Rebuild()
        {
            _instancesByKey.Clear();

            foreach (var instance in FindObjectsByType<ChutilEnsureUnique>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None))
            {
                RegisterInstance(instance);
            }
        }


        private static void RegisterInstance(ChutilEnsureUnique instance)
        {
            if (instance == null) { return; }
            if (_instancesByKey.TryGetValue(instance._key, out var _instances))
            {
                _instances.Add(instance);
            }
            else
            {
                _instances = new HashSet<ChutilEnsureUnique>() { instance };
                _instancesByKey[instance._key] = _instances;
            }
        }


        public static bool IsUnique(string key)
        {
            if (_instancesByKey.TryGetValue(key, out var instances))
            {
                if (instances.Count > 1)
                {
                    return false;
                }
            }
            return true;
        }


        public static bool IsAllKeyUnique()
        {
            foreach (var kvp in _instancesByKey)
            {
                if (kvp.Value.Count > 1)
                {
                    return false;
                }
            }
            return true;
        }


        public static HashSet<ChutilEnsureUnique> GetInstances(string key)
        {
            if (_instancesByKey.TryGetValue(key, out var instances))
            {
                return instances;
            }
            return null;
        }


        public static Dictionary<string, HashSet<ChutilEnsureUnique>> GetAllInstances()
        {
            return _instancesByKey;
        }
    }




#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(ChutilEnsureUnique))]
    [CanEditMultipleObjects]
    internal class ChutilEnsureUniqueEditor : Editor
    {
        private bool _canEditKey = false;
        private SerializedProperty _keyProp;

        HashSet<ChutilEnsureUnique> _instances;




        private void OnEnable()
        {
            ChutilEnsureUnique.Rebuild();
            _keyProp = serializedObject.FindProperty(ChutilEnsureUnique.KeyFieldName);

            _instances = ChutilEnsureUnique.GetInstances(_keyProp.stringValue);
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(
                "Only one object with the same Key can be placed in the scene.\n" +
                "If a duplicate exists, the placement may not match the asset author's intent.\n" +
                "\n" +
                "同じ Key を持つオブジェクトは、シーン内に1つだけしか配置できません。\n" +
                "重複が発生している場合、アセット制作者の意図しない配置になっている可能性があります。",
                MessageType.Info
            );

            _canEditKey = EditorGUILayout.Toggle("Edit Key", _canEditKey);
            EditorGUI.BeginChangeCheck();
            using (new EditorGUI.DisabledScope(!_canEditKey))
            {
                EditorGUILayout.DelayedTextField(_keyProp, new GUIContent("Key"));
            }
            bool hasKeyChanged = EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();

            if (hasKeyChanged)
            {
                ChutilEnsureUnique.Rebuild();
                _canEditKey = false;
            }

            bool isUnique = ChutilEnsureUnique.IsUnique(_keyProp.stringValue);
            if (!isUnique)
            {
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                EditorGUILayout.HelpBox(
                    "Multiple objects with the same Key were found.\n" +
                    "Resolve the duplicate before building.\n" +
                    "\n" +
                    "同じ Key を持つオブジェクトが複数存在します。\n" +
                    "重複を解消してからビルドしてください。",
                    MessageType.Error
                );

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.LabelField("Conflicting Objects", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                foreach (var instance in _instances)
                {
                    if (instance == null) { continue; }
                    EditorGUILayout.ObjectField(instance.gameObject, typeof(GameObject), true);
                }
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            }
        }
    }
#endif
}
