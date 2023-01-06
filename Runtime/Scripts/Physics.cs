using System.ComponentModel;
using UnityEngine;
using static Utils.PortalUtils;
namespace Scripts
{
    public static class Physics 
    {
        /// <summary>
        ///   <para>Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the Scene. Can pass through portals</para>
        /// </summary>
        /// <param name="origin">The starting point of the ray in world coordinates.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="maxDistance">The max distance the ray should check for collisions.</param>
        /// <param name="layerMask">A that is used to selectively ignore Colliders when casting a ray.</param>
        /// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
        /// <returns>
        ///   <para>Returns true if the ray intersects with a Collider, otherwise false.</para>
        /// </returns>
        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            [DefaultValue("Mathf.Infinity")] float maxDistance,
            [DefaultValue("DefaultRaycastLayers")] int layerMask,
            [DefaultValue("QueryTriggerInteraction.UseGlobal")]
            QueryTriggerInteraction queryTriggerInteraction
        )
        {
            
            if (!UnityEngine.Physics.Raycast(origin, direction, out var hit, maxDistance, layerMask,
                    queryTriggerInteraction))
                return false;
            var portal = hit.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            
            if (!origin.IsInFrontOf(portal.transform) || portal.GetOutPortal() == null)
            {
                //nuevo raycast en el mismo mundo desde el portal
                return PortalRaycast(hit.point, direction, maxDistance - hit.distance, layerMask, queryTriggerInteraction);
            }
            // hit portal, we have to cast a new raycast from outPortal
            var newOrigin = GetRelativeWorldPos(hit.point, portal.transform, portal.GetOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetOutPortal().transform);
            return PortalRaycast(newOrigin, newDirection, maxDistance - hit.distance, layerMask, queryTriggerInteraction);
            
        }

        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            out RaycastHit hitInfo,
            [DefaultValue("Mathf.Infinity")] float maxDistance,
            [DefaultValue("DefaultRaycastLayers")] int layerMask,
            [DefaultValue("QueryTriggerInteraction.UseGlobal")]
            QueryTriggerInteraction queryTriggerInteraction)
        {
            var didHit = UnityEngine.Physics.Raycast(origin, direction, out  hitInfo, maxDistance, layerMask,
                queryTriggerInteraction);
            if (!didHit)
            {
                return false;
            }
                
            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            
            if (!origin.IsInFrontOf(portal.transform) || portal.GetLinkedOutPortal() == null)
            {
                //nuevo raycast en el mismo mundo desde el portal
                return PortalRaycast(hitInfo.point, direction, out hitInfo, maxDistance - hitInfo.distance, layerMask, queryTriggerInteraction);
            }
            // hit portal, we have to cast a new raycast from outPortal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);
            return PortalRaycast(newOrigin, newDirection, out hitInfo,maxDistance - hitInfo.distance, layerMask, queryTriggerInteraction);

        }
        
        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            out RaycastHit hitInfo,
            [DefaultValue("Mathf.Infinity")] float maxDistance,
            [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            var didHit = UnityEngine.Physics.Raycast(origin, direction, out  hitInfo, maxDistance, layerMask);
            if (!didHit)
            {
                return false;
            }
                
            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            
            if (!origin.IsInFrontOf(portal.transform) || portal.GetLinkedOutPortal() == null)
            {
                //nuevo raycast en el mismo mundo desde el portal
                return PortalRaycast(hitInfo.point, direction, out hitInfo, maxDistance - hitInfo.distance, layerMask);
            }
            // hit portal, we have to cast a new raycast from outPortal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);
            return PortalRaycast(newOrigin, newDirection, out hitInfo,maxDistance - hitInfo.distance, layerMask);

        }
        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            out RaycastHit hitInfo,
            [DefaultValue("Mathf.Infinity")] float maxDistance)
        {
            var didHit = UnityEngine.Physics.Raycast(origin, direction, out  hitInfo, maxDistance);
            if (!didHit)
            {
                return false;
            }
                
            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            
            if (!origin.IsInFrontOf(portal.transform) || portal.GetLinkedOutPortal() == null)
            {
                //nuevo raycast en el mismo mundo desde el portal
                return PortalRaycast(hitInfo.point, direction, out hitInfo, maxDistance - hitInfo.distance);
            }
            // hit portal, we have to cast a new raycast from outPortal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);
            return PortalRaycast(newOrigin, newDirection, out hitInfo,maxDistance - hitInfo.distance);

        }
        
        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            out RaycastHit hitInfo)
        {
            
            var didHit = UnityEngine.Physics.Raycast(origin, direction, out  hitInfo);
            if (!didHit)
            {
                return false;
            }
            Debug.DrawRay(origin, direction *hitInfo.distance, Color.blue);  
            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            
            if (!origin.IsInFrontOf(portal.transform) || portal.GetLinkedOutPortal() == null)
            {
                //nuevo raycast en el mismo mundo desde el portal
                return PortalRaycast(hitInfo.point, direction, out hitInfo);
            }
            // hit portal, we have to cast a new raycast from outPortal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);
            
            // Debug.DrawRay(newOrigin, newDirection *100, Color.blue);
            // return true;
            return PortalRaycast(newOrigin, newDirection, out hitInfo);

        }
        
        
        
        
    }
}
