
using Codice.Client.BaseCommands;
using UnityEngine;

namespace Core.Portal.Scripts
{
    public static class ExtensionMethods
    {
        public static bool IsInFrontOf(this Transform one, Transform other)
        {
            
            var toOther = one.position - other.position;
            var dot = Vector3.Dot(other.forward, toOther.normalized);
            return dot >= 0;
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
        public static bool IsInFrontOfWithError2(this Transform one, Transform other, float error)
        {
            var toOther = one.position - other.position;
            return Vector3.Dot(other.forward, toOther) > -error ;
        }
        public static bool IsInFrontOfWithError(this Vector3 one, Transform other, float error)
        {
            var toOther = one - other.position;
            return Vector3.Dot(other.forward, toOther) > -error ;
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
        public static bool IsLooking(this Camera camera, Renderer r)
        {
            var bounds = r.bounds;
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);

            return GeometryUtility.TestPlanesAABB(planes, bounds);

        }
        
        // TODO: it still "sees" portals that he shouldnt, pretty sure the custom planes arent being generated corrreclty
        public static bool IsLookingThroughDoor(this Plane[] customFrustumPlanes, Renderer objectRenderer)
        {
            // Get the bounds of the door and the object
            Bounds objectBounds = objectRenderer.bounds;
            
            // Now test if the object is within the custom frustum planes
            return GeometryUtility.TestPlanesAABB(customFrustumPlanes, objectBounds);
        }

        // Function to compare and replace the custom plane with the camera's frustum plane if needed
        private static Plane CompareAndReplacePlane(Plane customPlane, Plane cameraPlane, Camera camera)
        {
            // Get the camera's forward direction
            Vector3 cameraForward = camera.transform.forward;

            // Calculate angles between the camera's forward direction and the planes
            float angleWithCameraPlane = Vector3.Angle(cameraForward, cameraPlane.normal);
            float angleWithCustomPlane = Vector3.Angle(cameraForward, customPlane.normal);

            // Check if the angle with the camera plane is smaller than with the custom plane
            if (angleWithCameraPlane > angleWithCustomPlane)
            {
                // Replace with the camera's plane
                return cameraPlane;
            }
            // Otherwise, keep the custom plane
            return customPlane;
        }

        public static Plane[] GenerateCustomFrustumPlanes(this Camera camera, Portal outPortal)
        {
            var doorBounds = outPortal.GetRenderer().localBounds.size;
            var topLeft = outPortal.transform.TransformPoint(  Vector3.left * doorBounds.x/2 + Vector3.up * doorBounds.z/2 ); 
            var topRight = outPortal.transform.TransformPoint(  Vector3.right * doorBounds.x/2 + Vector3.up * doorBounds.z/2); 
            var bottomLeft = outPortal.transform.TransformPoint(  Vector3.left * doorBounds.x/2 +Vector3.down * doorBounds.z/2 ); 
            var bottomRight = outPortal.transform.TransformPoint(  Vector3.right * doorBounds.x/2 + Vector3.down * doorBounds.z/2 ); 
            // Now you have the world-space positions of the corners from the camera's perspective

            var cameraPosition = camera.transform.position;
            Plane[] customFrustumPlanes = new Plane[6];
            Plane customLeftPlane = new Plane(cameraPosition,topLeft, bottomLeft);     // Left edge plane
            Plane customRightPlane = new Plane(cameraPosition, bottomRight, topRight);  // Right edge plane
            Plane customTopPlane = new Plane(cameraPosition, topRight, topLeft);        // Top edge plane
            Plane customBottomPlane = new Plane(cameraPosition, bottomLeft,bottomRight); // Bottom edge plane
            customFrustumPlanes[(int)FrustumPlane.Left] = customLeftPlane;
            customFrustumPlanes[(int)FrustumPlane.Right] = customRightPlane;
            customFrustumPlanes[(int)FrustumPlane.Top] = customTopPlane;
            customFrustumPlanes[(int)FrustumPlane.Bottom] = customBottomPlane;
            
            Plane[] cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);

            // // Replace the custom planes if they are more permissive than the camera's frustum planes
            customFrustumPlanes[(int)FrustumPlane.Top] = CompareAndReplacePlane(customFrustumPlanes[(int)FrustumPlane.Top], cameraFrustumPlanes[(int)FrustumPlane.Top], camera);
            customFrustumPlanes[(int)FrustumPlane.Bottom] = CompareAndReplacePlane(customFrustumPlanes[(int)FrustumPlane.Bottom], cameraFrustumPlanes[(int)FrustumPlane.Bottom], camera);
            customFrustumPlanes[(int)FrustumPlane.Left] = CompareAndReplacePlane(customFrustumPlanes[(int)FrustumPlane.Left], cameraFrustumPlanes[(int)FrustumPlane.Left], camera);
            customFrustumPlanes[(int)FrustumPlane.Right] = CompareAndReplacePlane(customFrustumPlanes[(int)FrustumPlane.Right], cameraFrustumPlanes[(int)FrustumPlane.Right], camera);
            customFrustumPlanes[(int)FrustumPlane.Near] = cameraFrustumPlanes[(int)FrustumPlane.Near];
            customFrustumPlanes[(int)FrustumPlane.Far] = cameraFrustumPlanes[(int)FrustumPlane.Far];
            
            return customFrustumPlanes;
        }
        

        // Enum to simplify referencing the camera frustum planes
        private enum FrustumPlane
        {
            Left = 0,
            Right = 1,
            Bottom = 2,
            Top = 3,
            Near = 4,
            Far = 5
        }
        
    }
}