using UnityEngine;

namespace Core.Portal.Editor
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
    
#endif
}