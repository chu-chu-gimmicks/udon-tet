using ChuChuGimmicks.UDONTET.EditorOnly;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UdonSharp;

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    [CustomEditor(typeof(Setup))]
    public class SetupEditor : Editor
    {
        private SerializedProperty _numberOfMainProp;
        private SerializedProperty _numberOfStatsBoardProp;
        private SerializedProperty _usePersistenceProp;
        private SerializedProperty _udonChipsProp;
        private SerializedProperty _udonChipsRateProp;

        private SerializedProperty _mainsProp;
        private SerializedProperty _statsBoardsProp;
        private SerializedProperty _userDataAccessorProp;

        private SerializedProperty _mainPrefabProp;
        private SerializedProperty _statsBoardPrefabProp;
        private SerializedProperty _userDataAccessorPrefabProp;

        private SerializedProperty __numberOfMainProp;
        private SerializedProperty __numberOfStatsBoardProp;
        private SerializedProperty __usePersistenceProp;
        private SerializedProperty __udonChipsProp;
        private SerializedProperty __udonChipsRateProp;

        private bool _hasChanged = true;
        private bool _isOpen = false;




        private void OnEnable()
        {
            _numberOfMainProp       = serializedObject.FindProperty(Setup.FieldNames.NumberOfMain);
            _numberOfStatsBoardProp = serializedObject.FindProperty(Setup.FieldNames.NumberOfStatsBoard);
            _usePersistenceProp     = serializedObject.FindProperty(Setup.FieldNames.UsePersistence);
            _udonChipsProp          = serializedObject.FindProperty(Setup.FieldNames.UdonChips);
            _udonChipsRateProp      = serializedObject.FindProperty(Setup.FieldNames.UdonChipsRate);

            _mainsProp            = serializedObject.FindProperty(Setup.FieldNames.Mains);
            _statsBoardsProp      = serializedObject.FindProperty(Setup.FieldNames.StatsBoards);
            _userDataAccessorProp = serializedObject.FindProperty(Setup.FieldNames.UserDataAccessor);

            _mainPrefabProp             = serializedObject.FindProperty(Setup.FieldNames.MainPrefab);
            _statsBoardPrefabProp       = serializedObject.FindProperty(Setup.FieldNames.StatsBoardPrefab);
            _userDataAccessorPrefabProp = serializedObject.FindProperty(Setup.FieldNames.UserDataAccessorPrefab);

            __numberOfMainProp       = serializedObject.FindProperty(Setup.FieldNames._NumberOfMain);
            __numberOfStatsBoardProp = serializedObject.FindProperty(Setup.FieldNames._NumberOfStatsBoard);
            __usePersistenceProp     = serializedObject.FindProperty(Setup.FieldNames._UsePersistence);
            __udonChipsProp          = serializedObject.FindProperty(Setup.FieldNames._UdonChips);
            __udonChipsRateProp      = serializedObject.FindProperty(Setup.FieldNames._UdonChipsRate);
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(
                "Please make any additions, removals, or configuration changes related to UDON TET through this component.\n" +
                "Required references will be set automatically.\n" +
                "\n" +
                "UDON TET に関する追加・削除および設定変更は、このコンポーネントから行ってください。\n" +
                "必要な参照関係が自動的に設定されます。",
                MessageType.Info
            );

            EditorGUILayout.HelpBox(
                "- Number of UDON TET instances\n" +
                "- Number of Stats Board instances\n" +
                "    - Whether to use Persistence\n" +
                "- Assign a reference if using UdonChips\n" +
                "    - UdonChips = Score * UdonChips Rate\n" +
                "\n" +
                "- UDON TET の配置数\n" +
                "- Stats Board の配置数\n" +
                "    - Persistence 機能の使用\n" +
                "- UdonChips を使用する場合は参照を設定\n" +
                "    - UdonChips = Score * UdonChips Rate",
                MessageType.Info
            );

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.IntSlider(_numberOfMainProp, 1, 8, new GUIContent("Number of UDON TET"));
            EditorGUILayout.IntSlider(_numberOfStatsBoardProp, 0, 8, new GUIContent("Number of Stats Board"));
            if (_numberOfStatsBoardProp.intValue > 0)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_usePersistenceProp, new GUIContent("Use Persistence"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(_udonChipsProp, new GUIContent("UdonChips"));
            if (_udonChipsProp.objectReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_udonChipsRateProp, new GUIContent("UdonChips Rate"));
                EditorGUI.indentLevel--;
            }
            _hasChanged = EditorGUI.EndChangeCheck() ? true : _hasChanged;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUILayout.HelpBox(
                "Revert: Discard your edits.\n" +
                "Apply: Apply your edits.\n" +
                "\n" +
                "Revert: 編集内容を破棄します。\n" +
                "Apply: 編集内容を適用します。",
                MessageType.Info
            );

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!_hasChanged);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Revert"))
            {
                Revert();
                _hasChanged = false;
            }
            else if (GUILayout.Button("Apply"))
            {
                Apply();
                _hasChanged = false;
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginDisabledGroup(true);
            _isOpen = EditorGUILayout.Foldout(_isOpen, "References", true);
            if (_isOpen)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_mainsProp,            new GUIContent("UDON TET"));
                EditorGUILayout.PropertyField(_statsBoardsProp,      new GUIContent("Stats Board"));
                EditorGUILayout.PropertyField(_userDataAccessorProp, new GUIContent("User Data Accessor"));
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
                EditorGUILayout.PropertyField(_mainPrefabProp,             new GUIContent("UDON TET Prefab"));
                EditorGUILayout.PropertyField(_statsBoardPrefabProp,       new GUIContent("Stats Board Prefab"));
                EditorGUILayout.PropertyField(_userDataAccessorPrefabProp, new GUIContent("User Data Accessor Prefab"));
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
                EditorGUILayout.PropertyField(__numberOfMainProp,       new GUIContent("Number of UDON TET (Backup)"));
                EditorGUILayout.PropertyField(__numberOfStatsBoardProp, new GUIContent("Number of Stats Board (Backup)"));
                EditorGUILayout.PropertyField(__usePersistenceProp,     new GUIContent("Use Persistence (Backup)"));
                EditorGUILayout.PropertyField(__udonChipsProp,          new GUIContent("UdonChips (Backup)"));
                EditorGUILayout.PropertyField(__udonChipsRateProp,      new GUIContent("UdonChips Rate (Backup)"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }


        private void Revert()
        {
            Util.Log("Revert");

            _numberOfMainProp.intValue          = __numberOfMainProp.intValue;
            _numberOfStatsBoardProp.intValue    = __numberOfStatsBoardProp.intValue;
            _usePersistenceProp.boolValue       = __usePersistenceProp.boolValue;
            _udonChipsProp.objectReferenceValue = __udonChipsProp.objectReferenceValue;
            _udonChipsRateProp.floatValue       = __udonChipsRateProp.floatValue;
        }


        private void Apply()
        {
            Util.Log("Apply");

            Setup setup = target as Setup;

            SetupHandler.CreateMains(
                setup.Mains,
                setup.NumberOfMain,
                setup.MainPrefab,
                setup.transform,
                setup.UdonChips,
                setup.UdonChipsRate
            );
            _mainsProp.ClearArray();
            _mainsProp.arraySize = setup.Mains.Count;
            for (int i = 0; i < setup.Mains.Count; i++)
            {
                SerializedProperty elementProp = _mainsProp.GetArrayElementAtIndex(i);
                elementProp.objectReferenceValue = setup.Mains[i];
            }

            SetupHandler.CreateStatsBoards(
                setup.StatsBoards,
                setup.NumberOfStatsBoard,
                setup.StatsBoardPrefab,
                setup.transform
            );
            _statsBoardsProp.ClearArray();
            _statsBoardsProp.arraySize = setup.StatsBoards.Count;
            for(int i = 0;i < setup.StatsBoards.Count; i++)
            {
                SerializedProperty elementProp = _statsBoardsProp.GetArrayElementAtIndex(i);
                elementProp.objectReferenceValue = setup.StatsBoards[i];
            }

            bool usePersistence = setup.UsePersistence && setup.StatsBoards != null && setup.StatsBoards.Count > 0;
            _userDataAccessorProp.objectReferenceValue = SetupHandler.CreateUserDataAccessor(
                setup.UserDataAccessor,
                usePersistence,
                setup.UserDataAccessorPrefab,
                setup.transform
            );

            serializedObject.ApplyModifiedProperties();
            SetupHandler.RegisterToManager(setup.gameObject, setup.Mains, setup.StatsBoards, setup.UserDataAccessor);

            __numberOfMainProp.intValue          = setup.NumberOfMain;
            __numberOfStatsBoardProp.intValue    = setup.NumberOfStatsBoard;
            __usePersistenceProp.boolValue       = setup.UsePersistence;
            __udonChipsProp.objectReferenceValue = setup.UdonChips;
            __udonChipsRateProp.floatValue       = setup.UdonChipsRate;
        }
    }
}
