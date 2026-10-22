using ChuChuGimmicks.UDONTET.Runtime;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    [CustomEditor(typeof(Manager))]
    public class ManagerEditor : Editor
    {
        SerializedProperty _mainsProp;
        SerializedProperty _statsBoardsProp;
        SerializedProperty _userDataAccessorProp;

        private bool _isOpen = false;




        private void OnEnable()
        {
            _mainsProp = serializedObject.FindProperty(Manager.FieldNames.Mains);
            _statsBoardsProp = serializedObject.FindProperty(Manager.FieldNames.StatsBoards);
            _userDataAccessorProp = serializedObject.FindProperty(Manager.FieldNames.UserDataAccessor);
        }


        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) { return; }

            EditorGUI.BeginDisabledGroup(true);
            _isOpen = EditorGUILayout.Foldout(_isOpen, "References", true);
            if (_isOpen)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_mainsProp, new GUIContent("UDON TET"));
                EditorGUILayout.PropertyField(_statsBoardsProp, new GUIContent("Stats Board"));
                EditorGUILayout.PropertyField(_userDataAccessorProp, new GUIContent("User Data Accessor"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
