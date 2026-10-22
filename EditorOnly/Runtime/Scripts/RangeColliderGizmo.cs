using UnityEngine;

#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
#endif

namespace ChuChuGimmicks.UDONTET.EditorOnly
{
    public class RangeColliderGizmo : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        private void OnDrawGizmos()
        {
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Color previousColor = Gizmos.color;

            // 一時的に変更
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.color = new Color32(0, 255, 0, 63);

            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            // 元に戻す
            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }
    }




#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(RangeColliderGizmo))]
    public class RangeColliderGizmoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                $"Synced data is applied to players within this Cube.\n" +
                $"\n" +
                $"この Cube 内にいるプレイヤーには、同期データが反映されます。",
                MessageType.Info
            );
        }
    }
#endif
}
