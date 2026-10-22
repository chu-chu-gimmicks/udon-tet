using ChuChuGimmicks.UDONTET.Runtime;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    [CustomEditor(typeof(StatsBoard))]
    public class StatsBoardEditor : Editor
    {
        SerializedProperty _pValueTextProp;
        SerializedProperty _lNameTextsProp;
        SerializedProperty _lValueTextProp;
        SerializedProperty _rNameTextsProp;
        SerializedProperty _rValueTextProp;

        private bool _isOpen = false;




        private void OnEnable()
        {
            _pValueTextProp = serializedObject.FindProperty(StatsBoard.FieldNames.PValueText);
            _lNameTextsProp = serializedObject.FindProperty(StatsBoard.FieldNames.LNameTexts);
            _lValueTextProp = serializedObject.FindProperty(StatsBoard.FieldNames.LValueText);
            _rNameTextsProp = serializedObject.FindProperty(StatsBoard.FieldNames.RNameTexts);
            _rValueTextProp = serializedObject.FindProperty(StatsBoard.FieldNames.RValueText);
        }


        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) { return; }

            EditorGUILayout.HelpBox(
                "Move this object to your desired position.\n" +
                "You may change its parent-child hierarchy.\n" +
                "To add or remove these objects, use the UDON TET - Manager object.\n" +
                "\n" +
                "このオブジェクトを好きな位置に配置してください。\n" +
                "親子関係は変更して構いません。\n" +
                "追加・削除は UDON TET - Manager オブジェクトから行ってください。",
                MessageType.Info
            );

            EditorGUI.BeginDisabledGroup(true);
            _isOpen = EditorGUILayout.Foldout(_isOpen, "References", true);
            if (_isOpen)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_pValueTextProp, new GUIContent("Personal Data Value Text"));
                EditorGUILayout.PropertyField(_lNameTextsProp, new GUIContent("Leaderboard Name Texts"));
                EditorGUILayout.PropertyField(_lValueTextProp, new GUIContent("Leaderboard Value Text"));
                EditorGUILayout.PropertyField(_rNameTextsProp, new GUIContent("Records Name Texts"));
                EditorGUILayout.PropertyField(_rValueTextProp, new GUIContent("Records Value Text"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
