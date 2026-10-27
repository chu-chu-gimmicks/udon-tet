using ChuChuGimmicks.UDONTET.Runtime;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    [CustomEditor(typeof(Main))]
    public class MainEditor : Editor
    {
        SerializedProperty _udonChipsProp;
        SerializedProperty _udonChipsRateProp;

        SerializedProperty _updateHandlerProp;

        SerializedProperty _minoMaterialsProp;
        SerializedProperty _gridTransformsProp;
        SerializedProperty _gridRenderersProp;
        SerializedProperty _holdRenderersProp;
        SerializedProperty _nextRenderersProp;

        SerializedProperty _playerNameTextProp;
        SerializedProperty _titleUIProp;
        SerializedProperty _ingameUIProp;
        SerializedProperty _statsValueTextProp;
        SerializedProperty _popupTextProp;
        SerializedProperty _gameOverUIProp;
        SerializedProperty _buttonProp;
        SerializedProperty _buttonTextProp;
        SerializedProperty _guideProp;
        SerializedProperty _guideBindingsProp;
        SerializedProperty _titleMinoProp;

        SerializedProperty _stationProp;
        SerializedProperty _maxHeightProp;
        SerializedProperty _minHeightProp;

        SerializedProperty _rangeColliderProp;

        private bool _isOpen = false;




        private void OnEnable()
        {
            _udonChipsProp     = serializedObject.FindProperty(Main.FieldNames.UdonChips);
            _udonChipsRateProp = serializedObject.FindProperty(Main.FieldNames.UdonChipsRate);

            _updateHandlerProp = serializedObject.FindProperty(Main.FieldNames.UpdateHandler);

            _minoMaterialsProp  = serializedObject.FindProperty(Main.FieldNames.MinoMaterials);
            _gridTransformsProp = serializedObject.FindProperty(Main.FieldNames.GridTransforms);
            _gridRenderersProp  = serializedObject.FindProperty(Main.FieldNames.GridRenderers);
            _holdRenderersProp  = serializedObject.FindProperty(Main.FieldNames.HoldRenderers);
            _nextRenderersProp  = serializedObject.FindProperty(Main.FieldNames.NextRenderers);


            _playerNameTextProp = serializedObject.FindProperty(Main.FieldNames.PlayerNameText);
            _titleUIProp        = serializedObject.FindProperty(Main.FieldNames.TitleUI);
            _ingameUIProp       = serializedObject.FindProperty(Main.FieldNames.IngameUI);
            _statsValueTextProp = serializedObject.FindProperty(Main.FieldNames.StatsValueText);
            _popupTextProp      = serializedObject.FindProperty(Main.FieldNames.PopupText);
            _gameOverUIProp     = serializedObject.FindProperty(Main.FieldNames.GameOverUI);
            _buttonProp         = serializedObject.FindProperty(Main.FieldNames.Button);
            _buttonTextProp     = serializedObject.FindProperty(Main.FieldNames.ButtonText);
            _guideProp          = serializedObject.FindProperty(Main.FieldNames.Guide);
            _guideBindingsProp  = serializedObject.FindProperty(Main.FieldNames.GuideBindings);
            _titleMinoProp      = serializedObject.FindProperty(Main.FieldNames.TitleMino);

            _stationProp   = serializedObject.FindProperty(Main.FieldNames.Station);
            _maxHeightProp = serializedObject.FindProperty(Main.FieldNames.MaxHeight);
            _minHeightProp = serializedObject.FindProperty(Main.FieldNames.MinHeight);

            _rangeColliderProp = serializedObject.FindProperty(Main.FieldNames.RangeCollider);


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
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginDisabledGroup(true);
            _isOpen = EditorGUILayout.Foldout(_isOpen, "References", true);
            if (_isOpen)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_udonChipsProp,     new GUIContent("UdonChips"));
                EditorGUILayout.PropertyField(_udonChipsRateProp, new GUIContent("UdonChips Rate"));
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                EditorGUILayout.PropertyField(_updateHandlerProp, new GUIContent("Update Handler"));
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                EditorGUILayout.PropertyField(_minoMaterialsProp,  new GUIContent("Mino Materials"));
                EditorGUILayout.PropertyField(_gridTransformsProp, new GUIContent("Grid Transforms"));
                EditorGUILayout.PropertyField(_gridRenderersProp,  new GUIContent("Grid Renderers"));
                EditorGUILayout.PropertyField(_holdRenderersProp,  new GUIContent("Hold Renderers"));
                EditorGUILayout.PropertyField(_nextRenderersProp,  new GUIContent("Next Renderers"));
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                EditorGUILayout.PropertyField(_playerNameTextProp, new GUIContent("Player Name Text"));
                EditorGUILayout.PropertyField(_titleUIProp,        new GUIContent("Title UI"));
                EditorGUILayout.PropertyField(_ingameUIProp,       new GUIContent("Ingame UI"));
                EditorGUILayout.PropertyField(_statsValueTextProp, new GUIContent("Stats Value Text"));
                EditorGUILayout.PropertyField(_popupTextProp,      new GUIContent("Popup Text"));
                EditorGUILayout.PropertyField(_gameOverUIProp,     new GUIContent("Game Over UI"));
                EditorGUILayout.PropertyField(_buttonProp,         new GUIContent("Button"));
                EditorGUILayout.PropertyField(_buttonTextProp,     new GUIContent("Button Text"));
                EditorGUILayout.PropertyField(_guideProp,          new GUIContent("Guide Renderer"));
                EditorGUILayout.PropertyField(_guideBindingsProp,  new GUIContent("Guide Materials"));
                EditorGUILayout.PropertyField(_titleMinoProp,      new GUIContent("TitleMino"));
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                EditorGUILayout.PropertyField(_stationProp,   new GUIContent("Station"));
                EditorGUILayout.PropertyField(_maxHeightProp, new GUIContent("Max Height"));
                EditorGUILayout.PropertyField(_minHeightProp, new GUIContent("Min Height"));
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                EditorGUILayout.PropertyField(_rangeColliderProp, new GUIContent("Range Collider"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
