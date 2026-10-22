using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace ChuChuGimmicks.UDONTET.Chutil
{
#if UNITY_EDITOR && !COMPILER_UDONSHARP
    using UnityEditor;
#endif

    [DisallowMultipleComponent]
    public class ChutilEnsureActive : MonoBehaviour, IEditorOnly
    {
        public int GetDepth()
        {
            var depth = 0;
            Transform current = this.transform;
            while (current.parent != null)
            {
                depth++;
                current = current.parent;
            }
            return depth;
        }
    }




#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(ChutilEnsureActive))]
    public class ChutilEnsureActiveEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "This component moves this GameObject to the scene root and sets it to Active at build time.\n" +
                "Any parent or inactive state set in the Editor will be overridden.\n" +
                "\n" +
                "このコンポーネントは、ビルド時にこの GameObject をルートへ移動し、Active にします。\n" +
                "エディタ上で設定された親子関係や非アクティブ状態は上書きされます。",
                MessageType.Info
            );
        }
    }
#endif
}
