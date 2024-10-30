using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static Core.Portal.Utils.PortalUtils;
namespace Core.Portal.Scripts
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
            
            RaycastHit hitInfo;
            // Perform the raycast
            if (!UnityEngine.Physics.Raycast(
                    origin: origin,
                    direction: direction,
                    hitInfo: out hitInfo, 
                    maxDistance: maxDistance, 
                    layerMask: layerMask, 
                    queryTriggerInteraction: queryTriggerInteraction))
            {
#if UNITY_EDITOR
                Debug.DrawRay(origin, direction * 1, Color.yellow); 
#endif
                return false;
            }


#if UNITY_EDITOR
            Debug.DrawRay(origin, direction * hitInfo.distance, Color.blue);
#endif

            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            if (!origin.IsInFrontOf(portal.transform))
            {
                // cast ray from point that it hit the portal in the same direction
                return PortalRaycast(
                    origin: hitInfo.point + direction, 
                    direction: direction, 
                    maxDistance: maxDistance - hitInfo.distance, 
                    layerMask: layerMask, 
                    queryTriggerInteraction: queryTriggerInteraction);

            }
            if(portal.GetLinkedOutPortal() == null)
                return true;
            
            // Calculate the new origin and direction for the raycast from the linked out portal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);

            // Recursive call to continue the raycast from the linked portal
            return PortalRaycast(
                origin: newOrigin+ newDirection, 
                direction: newDirection, 
                maxDistance: maxDistance - hitInfo.distance,
                layerMask: layerMask, queryTriggerInteraction: queryTriggerInteraction
                );

        }
        
        /// <summary>
        ///   <para>Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the Scene. Can pass through portals</para>
        /// </summary>
        /// <param name="origin">The starting point of the ray in world coordinates.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hitsInfo">List of RaycastHit info, each representing how the ray goes from portal to portal till it reaches an object</param>
        /// <param name="maxDistance">The max distance the ray should check for collisions.</param>
        /// <param name="layerMask">A that is used to selectively ignore Colliders when casting a ray.</param>
        /// <param name="queryTriggerInteraction">Specifies whether this query should hit Triggers.</param>
        /// <returns>
        ///   <para>Returns true if the ray intersects with a Collider, otherwise false.</para>
        /// </returns>
        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            out List<RaycastHit> hitsInfo,
            [DefaultValue("Mathf.Infinity")] float maxDistance,
            [DefaultValue("DefaultRaycastLayers")] int layerMask,
            [DefaultValue("QueryTriggerInteraction.UseGlobal")]
            QueryTriggerInteraction queryTriggerInteraction)
        {
            // Initialize the hitInfos list at the beginning
            hitsInfo = new List<RaycastHit>();
            RaycastHit hitInfo;
            // Perform the raycast
            if (!UnityEngine.Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction))
            {
#if UNITY_EDITOR
                Debug.DrawRay(origin, direction * 1, Color.yellow); 
#endif
                return false;
            }

            // Add the hitInfo to the list
            hitsInfo.Add(hitInfo);

#if UNITY_EDITOR
            Debug.DrawRay(origin, direction * hitInfo.distance, Color.blue);
#endif

            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            if (!origin.IsInFrontOf(portal.transform))
            {
                // cast ray from point that it hit the portal in the same direction
                if (PortalRaycast(hitInfo.point + direction, direction, out List<RaycastHit> subsequentHits,maxDistance - hitInfo.distance, layerMask, queryTriggerInteraction))
                {
                    hitsInfo.AddRange(subsequentHits);
                    return true;
                }
                return false;
            }
            if(portal.GetLinkedOutPortal() == null)
                return true;
            
            // Calculate the new origin and direction for the raycast from the linked out portal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);

            // Recursive call to continue the raycast from the linked portal
            if (PortalRaycast(newOrigin+ newDirection, newDirection, out List<RaycastHit> subsequentHits2,maxDistance - hitInfo.distance, layerMask, queryTriggerInteraction))
            {
                hitsInfo.AddRange(subsequentHits2);
                return true;
            }

            return false;
        }
        
        /// <summary>
        ///   <para>Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the Scene. Can pass through portals</para>
        /// </summary>
        /// <param name="origin">The starting point of the ray in world coordinates.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hitsInfo">List of RaycastHit info, each representing how the ray goes from portal to portal till it reaches an object</param>
        /// <param name="maxDistance">The max distance the ray should check for collisions.</param>
        /// <param name="layerMask">A that is used to selectively ignore Colliders when casting a ray.</param>
        /// <returns>
        ///   <para>Returns true if the ray intersects with a Collider, otherwise false.</para>
        /// </returns>
        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            out List<RaycastHit> hitsInfo,
            [DefaultValue("Mathf.Infinity")] float maxDistance,
            [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            // Initialize the hitInfos list at the beginning
            hitsInfo = new List<RaycastHit>();
            RaycastHit hitInfo;
            // Perform the raycast
            if (!UnityEngine.Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask))
            {
#if UNITY_EDITOR
                Debug.DrawRay(origin, direction * 1, Color.yellow); 
#endif
                return false;
            }

            // Add the hitInfo to the list
            hitsInfo.Add(hitInfo);

#if UNITY_EDITOR
            Debug.DrawRay(origin, direction * hitInfo.distance, Color.blue);
#endif

            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            if (!origin.IsInFrontOf(portal.transform))
            {
                // cast ray from point that it hit the portal in the same direction
                if (PortalRaycast(hitInfo.point + direction, direction, out List<RaycastHit> subsequentHits,maxDistance - hitInfo.distance, layerMask))
                {
                    hitsInfo.AddRange(subsequentHits);
                    return true;
                }
                return false;
            }
            if(portal.GetLinkedOutPortal() == null)
                return true;
            
            // Calculate the new origin and direction for the raycast from the linked out portal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);

            // Recursive call to continue the raycast from the linked portal
            if (PortalRaycast(newOrigin+ newDirection, newDirection, out List<RaycastHit> subsequentHits2,maxDistance - hitInfo.distance, layerMask))
            {
                hitsInfo.AddRange(subsequentHits2);
                return true;
            }

            return false;
        }
        
        /// <summary>
        ///   <para>Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the Scene. Can pass through portals</para>
        /// </summary>
        /// <param name="origin">The starting point of the ray in world coordinates.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hitsInfo">List of RaycastHit info, each representing how the ray goes from portal to portal till it reaches an object</param>
        /// <param name="maxDistance">The max distance the ray should check for collisions.</param>
        /// <returns>
        ///   <para>Returns true if the ray intersects with a Collider, otherwise false.</para>
        /// </returns>
        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            out List<RaycastHit> hitsInfo,
            [DefaultValue("Mathf.Infinity")] float maxDistance)
        {   
            // Initialize the hitInfos list at the beginning
            hitsInfo = new List<RaycastHit>();
            RaycastHit hitInfo;
            // Perform the raycast
            if (!UnityEngine.Physics.Raycast(origin, direction, out hitInfo, maxDistance))
            {
#if UNITY_EDITOR
                Debug.DrawRay(origin, direction * 1, Color.yellow); 
#endif
                return false;
            }

            // Add the hitInfo to the list
            hitsInfo.Add(hitInfo);

#if UNITY_EDITOR
            Debug.DrawRay(origin, direction * hitInfo.distance, Color.blue);
#endif

            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            if (!origin.IsInFrontOf(portal.transform))
            {
                // cast ray from point that it hit the portal in the same direction
                if (PortalRaycast(hitInfo.point + direction, direction, out List<RaycastHit> subsequentHits, maxDistance - hitInfo.distance))
                {
                    hitsInfo.AddRange(subsequentHits);
                    return true;
                }
                return false;
            }
            if(portal.GetLinkedOutPortal() == null)
                return true;
            
            // Calculate the new origin and direction for the raycast from the linked out portal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);

            // Recursive call to continue the raycast from the linked portal
            if (PortalRaycast(newOrigin+ newDirection, newDirection, out List<RaycastHit> subsequentHits2, maxDistance - hitInfo.distance))
            {
                hitsInfo.AddRange(subsequentHits2);
                return true;
            }

            return false;
        }
        
        /// <summary>
        ///   <para>Casts a ray, from point origin, in direction direction, of length maxDistance, against all colliders in the Scene. Can pass through portals</para>
        /// </summary>
        /// <param name="origin">The starting point of the ray in world coordinates.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hitsInfo">List of RaycastHit info, each representing how the ray goes from portal to portal till it reaches an object</param>
        /// <returns>
        ///   <para>Returns true if the ray intersects with a Collider, otherwise false.</para>
        /// </returns>
        public static bool PortalRaycast(
            Vector3 origin,
            Vector3 direction,
            out List<RaycastHit> hitsInfo)
        {
            // Initialize the hitInfos list at the beginning
            hitsInfo = new List<RaycastHit>();
            RaycastHit hitInfo;
            // Perform the raycast
            if (!UnityEngine.Physics.Raycast(origin, direction, out hitInfo))
            {
#if UNITY_EDITOR
                Debug.DrawRay(origin, direction * 1, Color.yellow); 
#endif
                return false;
            }

            // Add the hitInfo to the list
            hitsInfo.Add(hitInfo);

#if UNITY_EDITOR
            Debug.DrawRay(origin, direction * hitInfo.distance, Color.blue);
#endif

            var portal = hitInfo.collider.gameObject.GetComponent<Portal>();
            if (portal == null)
                return true;
            if (!origin.IsInFrontOf(portal.transform))
            {
              // cast ray from point that it hit the portal in the same direction
              if (PortalRaycast(hitInfo.point + direction, direction, out List<RaycastHit> subsequentHits))
              {
                  hitsInfo.AddRange(subsequentHits);
                  return true;
              }
              return false;
            }
            if(portal.GetLinkedOutPortal() == null)
                return true;
            
            // Calculate the new origin and direction for the raycast from the linked out portal
            var newOrigin = GetRelativeWorldPos(hitInfo.point, portal.transform, portal.GetLinkedOutPortal().transform);
            var newDirection = GetRelativeWorldDirection(direction, portal.transform, portal.GetLinkedOutPortal().transform);

            // Recursive call to continue the raycast from the linked portal
            if (PortalRaycast(newOrigin+ newDirection, newDirection, out List<RaycastHit> subsequentHits2))
            {
                hitsInfo.AddRange(subsequentHits2);
                return true;
            }

            return false;
            
            

            
        }
        
        
        
        
    }
}
