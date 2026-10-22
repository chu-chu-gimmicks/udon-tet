using ChuChuGimmicks.UDONTET.Runtime;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    [CustomEditor(typeof(UserDataAccessor))]
    public class UserDataAccessorEditor : Editor
    {
        SerializedProperty _referenceProp;

        private bool _isOpen = false;




        private void OnEnable()
        {
            _referenceProp = serializedObject.FindProperty(UserDataAccessor.FieldNames.Reference);
        }


        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) { return; }

            EditorGUILayout.HelpBox(
                "This object is required for Persistence.\n" +
                "Do not duplicate or delete it.\n" +
                "Enable or disable Persistence from the UDON TET - Manager object.\n" +
                "\n" +
                "Persistence 機能を使うためのオブジェクトです。\n" +
                "複製や削除はしないでください。\n" +
                "Persistence 機能の採否は UDON TET - Manager オブジェクトから変更してください。",
                MessageType.Info
            );

            EditorGUI.BeginDisabledGroup(true);
            _isOpen = EditorGUILayout.Foldout(_isOpen, "References", true);
            if (_isOpen)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_referenceProp, new GUIContent("Reference"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
