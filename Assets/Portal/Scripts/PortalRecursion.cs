using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;


public class PortalRecursion : MonoBehaviour
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
        if (!camera.CompareTag("MainCamera")) 
            return;
        foreach (var portal in allPortals.Where(portal => camera.IsLooking(portal.GetRenderPlane().gameObject) && portal.GetLinkedOutPortal() != null))
        {
            print(portal.name +"is seen by "+ camera.name);
            for (var i = 0; i <= recursiveIterations; i++)
            {
                RenderCamera(portal, i, context, camera);
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
        
        var lookingAtEachOther = true;
        var portalIn = inPortal;
        for (var i = iterationID; i < recursiveIterations && lookingAtEachOther; i++)
        {
            if (portalIn.GetLinkedOutPortal() == null) 
                return;
            cameraOutMovement.SetPortalIn(portalIn.transform);
            cameraOutMovement.SetPortalOut( portalIn.GetLinkedOutPortal().transform);
            cameraOutMovement.SetPositionAndAngle();
            cameraOutMovement.SetNearClippingPlane();
            
            if (_camera.IsLooking(portalIn.GetRenderPlane().gameObject) && _camera.transform.IsInFrontOf(portalIn.transform)&&
                portalIn.GetLinkedOutPortal() != null)
                cameraOutMovement.SetCameraBeingReplicated(_camera);
            else 
                lookingAtEachOther = false;
        }

        if (!lookingAtEachOther) 
            return;
        _camera.targetTexture =  portalIn.GetRenderTexture();
        UniversalRenderPipeline.RenderSingleCamera(context, _camera);



    }
    

    void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
    
    
   
    
    
    

}
