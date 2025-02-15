using UnityEditor;
using UnityEngine;
using AlsetRGames.Portal.Support;

namespace Core.Portal.Editor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ShowOnlyIfAttribute))]
    public class ShowOnlyIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowOnlyIfAttribute showOnlyIfAttribute = attribute as ShowOnlyIfAttribute;
            SerializedProperty conditionBool = property.serializedObject.FindProperty(showOnlyIfAttribute.conditionBoolName);

            if (conditionBool != null && conditionBool.propertyType == SerializedPropertyType.Enum)
            {
                if (conditionBool.enumValueIndex == (int)showOnlyIfAttribute.conditionValue)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
            {
                EditorGUI.HelpBox(position, "Invalid condition", MessageType.Warning);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowOnlyIfAttribute showOnlyIfAttribute = attribute as ShowOnlyIfAttribute;
            SerializedProperty conditionBool = property.serializedObject.FindProperty(showOnlyIfAttribute.conditionBoolName);

            if (conditionBool != null && conditionBool.propertyType == SerializedPropertyType.Enum)
            {
                if (conditionBool.enumValueIndex == (int)showOnlyIfAttribute.conditionValue)
                {
                    return EditorGUI.GetPropertyHeight(property, label);
                }
            }

            return 0f;
        }
    }
#endif
}

