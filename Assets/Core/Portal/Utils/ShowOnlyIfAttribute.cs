using UnityEditor;
using UnityEngine;

namespace Core.Portal.Utils
{
#if UNITY_EDITOR
    public class ShowOnlyIfAttribute : PropertyAttribute
    {
        public string conditionBoolName;
        public object conditionValue;

        public ShowOnlyIfAttribute(string boolName, object value)
        {
            conditionBoolName = boolName;
            conditionValue = value;
        }
    }

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