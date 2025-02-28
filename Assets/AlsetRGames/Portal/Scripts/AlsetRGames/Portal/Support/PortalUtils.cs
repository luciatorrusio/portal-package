using UnityEngine;

namespace AlsetRGames.Portal.Support
{
    public static class PortalUtils 
    {
        
        public static Vector3 GetRelativeWorldPos(Vector3 currentPosition, Transform portalIn, Transform portalOut)
        {
            //
            var scale = portalIn.localScale;
            var objectToPortal = portalIn.InverseTransformDirection(currentPosition - portalIn.position) ;
            var localPos = new Vector3(-objectToPortal.x* (1/scale.x), objectToPortal.y* (1/scale.y), -objectToPortal.z* (1/scale.z));
            return portalOut.TransformPoint(localPos);
            
        }
        public static Vector3 GetRelativeWorldDirection(Vector3 currentDirection, Transform portalIn, Transform portalOut)
        {
            Quaternion rotation = Quaternion.LookRotation(-portalIn.forward, portalIn.up);
            Quaternion relativeRot = Quaternion.Inverse(rotation) ;
            var direction = portalOut.rotation * relativeRot * currentDirection;
            return direction;
        }
        public enum CloneMode
        {
            AUTOMATIC, // The clone is a duplicate of the original, (only the mesh and colliders is duplicated)
            CUSTOM // the clone GameObject is given by the user 
        }
        
        public enum PortalMode
        {
            FULL_FUNCTION, // it transports the objects and sets the texture
            NO_TRANSPORTATION, // only sets the texture
            NO_IMAGE
        }
    }
}
