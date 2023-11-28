using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace Core.Portal.Scripts
{
    public class PortalRecursion : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]private Camera _camera;
        [HideInInspector]
        [SerializeField] private PortalCameraController portalCameraController;
        [SerializeField] private int recursiveIterations;
    
        [HideInInspector]
        [SerializeField] private PortalManager _portalManager;

        void Start()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != _portalManager.GetMainCamera())
                return;

            var seenPortals = PortalManager.allPortals
                .Where(portal => portal.GetLinkedOutPortal() != null && camera.IsLooking(portal.GetRenderer()));

            foreach (var portal in seenPortals)
            {
                RenderCamera(portal, 0, context, camera);
            }
        }

        private void RenderCamera(Portal inPortal, int iterationID, ScriptableRenderContext context, Camera cameraBeingReplicated)
        {
            if (iterationID == recursiveIterations)
            {
                _camera.targetTexture = inPortal.GetRenderTexture();
                UniversalRenderPipeline.RenderSingleCamera(context, _camera);
                return;
            }

            portalCameraController.SetCameraBeingReplicated(cameraBeingReplicated);

            if (inPortal.GetLinkedOutPortal() != null)
            {
                portalCameraController.SetPortalIn(inPortal.transform);
                portalCameraController.SetPortalOut(inPortal.GetLinkedOutPortal().transform);
                portalCameraController.SetPositionAndAngle();
                portalCameraController.SetNearClippingPlane();
                cameraBeingReplicated = _camera;

                RenderCamera(inPortal.GetLinkedOutPortal(), iterationID + 1, context, cameraBeingReplicated);
            }
        }



        void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }
    
    
   
    
    
    

    }
}
