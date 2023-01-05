using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    public static class ExtensionMethods
    {
        public static bool IsInFrontOf(this Transform one, Transform other)
        {
            
            var toOther = one.position - other.position;
            var dot = Vector3.Dot(other.forward, toOther);
            return dot > 0;
        }
        
        public static bool IsInFrontOf(this Vector3 one, Transform other)
        {
            
            var toOther = one - other.position;
            var dot = Vector3.Dot(other.forward, toOther);
            return dot > 0;
        }
        public static bool IsInFrontOfWithError(this Transform one, Transform other, float error)
        {
            var toOther = one.position - other.position;
            return Vector3.Dot(other.forward, toOther) > error ;
        }
        public static GameObject GetMainCamera(this Transform o)
        {
            foreach (var child in o.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("MainCamera")) 
                    return child.gameObject;
            }
            return null;
        
        }
    }
}