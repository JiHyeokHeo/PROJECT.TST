using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TST
{
    public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // 읽기 전용
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true; // 다시 활성화
        }
    }
#endif
}