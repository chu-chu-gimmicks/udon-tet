using UnityEngine;

#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
#endif

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    public class ViewpointGizmo : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        private void OnDrawGizmos()
        {
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Color previousColor = Gizmos.color;

            // 一時的に変更
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.color = new Color32(0, 255, 0, 63);

            Gizmos.DrawSphere(Vector3.zero, 0.03125f);

            // 元に戻す
            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }
    }




#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(ViewpointGizmo))]
    public class ViewpointGizmoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                $"This object represents the player's initial eye height.\n" +
                $"\n" +
                $"このオブジェクトはプレイヤーの初期視点の高さを表します。",
                MessageType.Info
            );
        }
    }
#endif
}
