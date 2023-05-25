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
            if (!(camera == _portalManager.GetMainCamera())) 
                return;
            foreach (var portal in PortalManager.allPortals.Where(portal => portal.GetLinkedOutPortal() != null && camera.IsLooking(portal.GetRenderer()) ))
            {
                for (var i = 0; i <= recursiveIterations; i++)
                {
                    RenderCamera(portal, i, context, camera);
                }
            }
        }

        private void RenderCamera(Portal inPortal, int iterationID, ScriptableRenderContext context, Camera cameraBeingReplicated)
        {
            while (true)
            {
                portalCameraController.SetCameraBeingReplicated(cameraBeingReplicated);
                if (iterationID == recursiveIterations)
                {
                    _camera.targetTexture = inPortal.GetRenderTexture();
                    UniversalRenderPipeline.RenderSingleCamera(context, _camera);
                    return;
                }

                iterationID++;

                if (inPortal.GetLinkedOutPortal() != null)
                {
                    // todo
                    portalCameraController.SetPortalIn(inPortal.transform);
                    portalCameraController.SetPortalOut(inPortal.GetLinkedOutPortal().transform);
                    portalCameraController.SetPositionAndAngle();
                    portalCameraController.SetNearClippingPlane();
                    cameraBeingReplicated = _camera;
                    continue;
                }


                break;
            }
        }


        void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }
    
    
   
    
    
    

    }
}
