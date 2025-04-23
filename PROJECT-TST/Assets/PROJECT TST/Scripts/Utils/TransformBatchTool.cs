using UnityEngine;
using UnityEditor;

namespace TST
{
    public class TransformBatchTool : EditorWindow
    {
        private Vector3 startPosition = Vector3.zero;
        private Vector3 positionSpacing = Vector3.right;
        private Vector3 startRotation = Vector3.zero;
        private Vector3 rotationStep = Vector3.zero;

        private bool applyPosition = true;
        private bool applyRotation = false;

        [MenuItem("Tools/Transform Batch Tool")]
        public static void ShowWindow()
        {
            GetWindow<TransformBatchTool>("Transform Batch Tool");
        }

        void OnGUI()
        {
            GUILayout.Label("🔧 배치 시작 위치", EditorStyles.boldLabel);
            startPosition = EditorGUILayout.Vector3Field("Start Position", startPosition);
            positionSpacing = EditorGUILayout.Vector3Field("Spacing (Offset)", positionSpacing);

            GUILayout.Space(10);
            GUILayout.Label("🎯 회전 설정", EditorStyles.boldLabel);
            startRotation = EditorGUILayout.Vector3Field("Start Rotation", startRotation);
            rotationStep = EditorGUILayout.Vector3Field("Rotation Step", rotationStep);

            GUILayout.Space(10);
            applyPosition = EditorGUILayout.Toggle("Apply Position", applyPosition);
            applyRotation = EditorGUILayout.Toggle("Apply Rotation", applyRotation);

            GUILayout.Space(10);
            if (GUILayout.Button("Apply To Selected Objects"))
            {
                ApplyTransformToSelection();
            }
        }

        void ApplyTransformToSelection()
        {
            GameObject[] selection = Selection.gameObjects;
            if (selection.Length == 0)
            {
                Debug.LogWarning("No objects selected.");
                return;
            }

            System.Array.Sort(selection, (a, b) => a.name.CompareTo(b.name));

            for (int i = 0; i < selection.Length; i++)
            {
                Undo.RecordObject(selection[i].transform, "Batch Transform");

                if (applyPosition)
                    selection[i].transform.position = startPosition + positionSpacing * i;

                if (applyRotation)
                    selection[i].transform.rotation = Quaternion.Euler(startRotation + rotationStep * i);
            }
        }
    }
}