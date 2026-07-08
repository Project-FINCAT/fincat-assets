using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TextSwitcherBehaviour))]
public class TextSwitcherDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 색상(1줄) + 폰트크기(1줄) + 텍스트 영역(3줄 정도) = 총 5줄 높이
        return 5 * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 1. 변수명을 textContent로 수정 (Behaviour.cs와 동일하게)
        SerializedProperty colorProp = property.FindPropertyRelative("color");
        SerializedProperty fontSizeProp = property.FindPropertyRelative("fontSize");
        SerializedProperty textProp = property.FindPropertyRelative("textContent"); 

        if (colorProp == null || fontSizeProp == null || textProp == null) return;

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        // 색상 필드
        EditorGUI.PropertyField(singleFieldRect, colorProp);

        // 폰트 크기 필드
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, fontSizeProp);

        // 텍스트 영역 (남은 높이를 모두 사용)
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        singleFieldRect.height = EditorGUIUtility.singleLineHeight * 3; // 3줄 높이로 설정
        
        EditorGUI.BeginProperty(position, label, textProp);
        textProp.stringValue = EditorGUI.TextArea(singleFieldRect, textProp.stringValue);
        EditorGUI.EndProperty();
    }
}