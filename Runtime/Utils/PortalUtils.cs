using UnityEngine;

namespace Utils
{
    public static class PortalUtils 
    {
        
        public static Vector3 GetRelativeWorldPos(Vector3 currentPosition, Transform portalIn, Transform portalOut)
        {
            var objectToPortal = portalIn.InverseTransformDirection(currentPosition - portalIn.position);
            var portalOutPosition = portalOut.position;
            return new Vector3(portalOutPosition.x -objectToPortal.x,portalOutPosition.y + objectToPortal.y, portalOutPosition.z -objectToPortal.z);
        }
        public static Vector3 GetRelativeWorldDirection(Vector3 currentDirection, Transform portalIn, Transform portalOut)
        {
            var anglesDifferencePerAxis  = Quaternion.FromToRotation(portalIn.forward, portalOut.forward);
            return anglesDifferencePerAxis  * Quaternion.AngleAxis( 180,  portalOut.up) * currentDirection;
        }
        public enum CloneMode
        {
            AUTOMATIC,CUSTOM
        }
    }
}
