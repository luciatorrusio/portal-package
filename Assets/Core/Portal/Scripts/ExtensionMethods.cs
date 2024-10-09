
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
        public static bool IsLookingThroughDoor(this Camera camera, Renderer objectRenderer, Renderer doorRenderer)
        {
            // Get the bounds of the door and the object
            Bounds doorBounds = doorRenderer.bounds;
            Bounds objectBounds = objectRenderer.bounds;

            // Get the camera position
            Vector3 cameraPosition = camera.transform.position;

            // Calculate the corners of the door's bounding box
            Vector3 doorTopLeft = new Vector3(doorBounds.min.x, doorBounds.max.y, doorBounds.min.z);
            Vector3 doorTopRight = new Vector3(doorBounds.max.x, doorBounds.max.y, doorBounds.min.z);
            Vector3 doorBottomLeft = new Vector3(doorBounds.min.x, doorBounds.min.y, doorBounds.min.z);
            Vector3 doorBottomRight = new Vector3(doorBounds.max.x, doorBounds.min.y, doorBounds.min.z);

            // Create frustum planes from the camera position to the door edges
            Plane customLeftPlane = new Plane(cameraPosition, doorBottomLeft, doorTopLeft);     // Left edge plane
            Plane customRightPlane = new Plane(cameraPosition, doorTopRight, doorBottomRight);  // Right edge plane
            Plane customTopPlane = new Plane(cameraPosition, doorTopLeft, doorTopRight);        // Top edge plane
            Plane customBottomPlane = new Plane(cameraPosition, doorBottomRight, doorBottomLeft); // Bottom edge plane
            // Get the camera's default frustum planes
            Plane[] cameraFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);

            // Replace the custom planes if they are more permissive than the camera's frustum planes
            customTopPlane = CompareAndReplacePlane(customTopPlane, cameraFrustumPlanes[(int)FrustumPlane.Top], camera);
            customBottomPlane = CompareAndReplacePlane(customBottomPlane, cameraFrustumPlanes[(int)FrustumPlane.Bottom], camera);
            customLeftPlane = CompareAndReplacePlane(customLeftPlane, cameraFrustumPlanes[(int)FrustumPlane.Left], camera);
            customRightPlane = CompareAndReplacePlane(customRightPlane, cameraFrustumPlanes[(int)FrustumPlane.Right], camera);

            // Create the custom frustum planes array
            Plane[] customFrustumPlanes = new Plane[6];

            // Assign the custom frustum planes for left, right, top, bottom
            customFrustumPlanes[(int)FrustumPlane.Left] = customLeftPlane;
            customFrustumPlanes[(int)FrustumPlane.Right] = customRightPlane;
            customFrustumPlanes[(int)FrustumPlane.Top] = customTopPlane;
            customFrustumPlanes[(int)FrustumPlane.Bottom] = customBottomPlane;

            // For near and far planes, use the camera's original frustum planes
            customFrustumPlanes[(int)FrustumPlane.Near] = cameraFrustumPlanes[(int)FrustumPlane.Near];
            customFrustumPlanes[(int)FrustumPlane.Far] = cameraFrustumPlanes[(int)FrustumPlane.Far];

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
            if (angleWithCameraPlane < angleWithCustomPlane)
            {
                // Replace with the camera's plane
                return cameraPlane;
            }
            // Otherwise, keep the custom plane
            return customPlane;
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