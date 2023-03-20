using System.Collections.Generic;
using Scripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;


public class PortalManager : MonoBehaviour
{
    private static  List<Portal> allPortals = new List<Portal>();
    private static Camera _camera;
    [SerializeField] private CameraOutMovement cameraOutMovement;
    [SerializeField] private int recursiveIterations;
    private int _currentIterations = 0;

    void Start()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        _camera = gameObject.GetComponent<Camera>();
    }

    public static void AddPortal(Portal newPortal)
    {
        allPortals.Add(newPortal);
    }

    public static Camera GetPortalCamera()
    {
        return _camera;
    }

    public static void RemovePortal(Portal portal)
    {
        allPortals.Remove(portal);
    }

    // OPCION 1
    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        
        // if(camera ==  _camera)
        //     print(camera.name + " rendering in pos: "+_camera.transform.position+" rot: "+_camera.transform.rotation );
        if(camera.CompareTag("MainCamera"))
        {
            foreach (var portal in allPortals)
            {

                if (camera.IsLooking(portal.gameObject) && portal.GetLinkedOutPortal() != null)
                {
                    _camera.targetTexture = portal.GetRenderTexture();

                    for (int i = 0; i <= recursiveIterations; i++)
                    {
                        RenderCamera(portal, i, context, camera);
                    }

                }
            }
        }
    
    }
    
    private void RenderCamera(Portal inPortal, int iterationID, ScriptableRenderContext context, Camera cameraBeingReplicated)
    {
        cameraOutMovement.SetCameraBeingReplicated(cameraBeingReplicated);
        if (iterationID == recursiveIterations)
        {
            _camera.targetTexture =  inPortal.GetRenderTexture();
            UniversalRenderPipeline.RenderSingleCamera(context, _camera);
            return;
        }
        
        iterationID++;
        
        if (  inPortal.GetLinkedOutPortal()!= null)
        {
            cameraOutMovement.SetPortalIn(inPortal.transform);
            cameraOutMovement.SetPortalOut( inPortal.GetLinkedOutPortal().transform);
            cameraOutMovement.SetPositionAndAngle();
            cameraOutMovement.SetNearClippingPlane();
            RenderCamera(inPortal.GetLinkedOutPortal(), iterationID, context, _camera);
        }
        
        
    }
    

    void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
    
    
   
    
    
    

}
