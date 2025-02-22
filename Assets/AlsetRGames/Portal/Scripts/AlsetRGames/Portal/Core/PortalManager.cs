using System.Collections.Generic;
using UnityEngine;

namespace AlsetRGames.Portal.Core
{
    public class PortalManager : MonoBehaviour
    {
        
        public static HashSet<Portal> allPortals { get; set; } =  new HashSet<Portal>();
        [SerializeField] private Camera mainCamera;
        private static Camera portalCamera;

        private void Awake()
        {
            portalCamera = GetComponent<Camera>();
            mainCamera = mainCamera == null ? Camera.main: mainCamera;
        }


        #region API

        public Camera GetMainCamera()
        {
            return mainCamera;
        }

        public void SetMainCamera(Camera camera)
        {
            mainCamera = camera;
        }

        #endregion
    
    
        public static void AddPortal(Portal newPortal)
        {
            allPortals.Add(newPortal);
            if (allPortals.Count == 1 && portalCamera != null){
                portalCamera.enabled = true;
            }
                
        }

        public static void RemovePortal(Portal portal)
        {
            if(allPortals.Count == 0)
                return;
            if(allPortals.Remove(portal) && allPortals.Count == 0 && portalCamera!= null)
                portalCamera.enabled = false;
        }

        
    }
}
