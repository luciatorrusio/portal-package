using System;
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

        private Material _material;
        private int counter = 0;
        private Stack<TextureToRender> savedTextures = new Stack<TextureToRender>();
        void Start()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        // void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        // {
        //     if (camera != _portalManager.GetMainCamera())
        //         return;
        //
        //     var seenPortals = PortalManager.allPortals
        //         .Where(portal => portal.GetLinkedOutPortal() != null && camera.IsLooking(portal.GetRenderer()));
        //
        //     foreach (var portal in seenPortals)
        //     {
        //         RenderCamera(portal, 0, context, camera);
        //     }
        // }
        //
        // private void RenderCamera(Portal inPortal, int iterationID, ScriptableRenderContext context, Camera cameraBeingReplicated)
        // {
        //     if (iterationID == recursiveIterations)
        //     {
        //         _camera.targetTexture = inPortal.GetRenderTexture();
        //         UniversalRenderPipeline.RenderSingleCamera(context, _camera);
        //         return;
        //     }
        //
        //     portalCameraController.SetCameraBeingReplicated(cameraBeingReplicated);
        //
        //     if (inPortal.GetLinkedOutPortal() != null)
        //     {
        //         portalCameraController.SetPortalIn(inPortal.transform);
        //         portalCameraController.SetPortalOut(inPortal.GetLinkedOutPortal().transform);
        //         portalCameraController.SetPositionAndAngle();
        //         portalCameraController.SetNearClippingPlane();
        //         cameraBeingReplicated = _camera;
        //
        //         RenderCamera(inPortal.GetLinkedOutPortal(), iterationID + 1, context, cameraBeingReplicated);
        //     }
        // }

        void OnBeginCameraRendering1(ScriptableRenderContext context, Camera camera)
        {
            if (!(camera == _portalManager.GetMainCamera())) 
                return;
            foreach (var portal in PortalManager.allPortals.Where(portal => portal.GetLinkedOutPortal() != null && Vector3.Dot(camera.transform.forward, portal.transform.forward) < 0 && camera.IsLooking(portal.GetRenderer())  ))
            {
                print(portal.name);
                for (var i = 0; i <= recursiveIterations; i++)
                {
                    RenderCamera1(portal, i, context, camera);
                }
            }
        }
        
        void OnBeginCameraRendering2(ScriptableRenderContext context, Camera camera)
        {
            if (!(camera == _portalManager.GetMainCamera())) 
                return;
            foreach (
                var portal in PortalManager.allPortals.Where(
                    portal => portal.GetLinkedOutPortal() != null 
                              && Vector3.Dot(camera.transform.forward, portal.transform.forward) < 0 
                              && camera.IsLooking(portal.GetRenderer()) 
                              ))
            {
                RenderCamera2(portal, context, camera);
            }
        }

        void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (!(camera == _portalManager.GetMainCamera())) 
                return;
            
            foreach (
                var portal in PortalManager.allPortals.Where(
                    portal => portal.GetLinkedOutPortal() != null 
                              && Vector3.Dot((portal.transform.position -camera.transform.position).normalized, portal.transform.forward) < 0 
                              && camera.IsLooking(portal.GetRenderer()) 
                ))
            {
                // print($"{counter++}, main camera can see {portal.name}");
                RenderCamera(portal, 0, context, camera);
                var renderTexture = portal.GetRenderTexture();
                // push texture and  corresponded portal 
                var savedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

                // Read pixels from the RenderTexture into the Texture2D
                savedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                savedTexture.Apply();

                // Reset the RenderTexture
                RenderTexture.active = null;
                savedTextures.Push(new TextureToRender()
                {
                    portal = portal,
                    texture =  savedTexture
                });
                
                
                // print($"{counter++}, saved texture to put on {portal.name}");
            }
            for (int i = 0; i < savedTextures.Count; i++)
            {
                // pop textures and set them to corresponding portal
                var a = savedTextures.Pop();
                var m =a.portal.GetPortalMaterial();
                m.mainTexture = a.texture;

            }
            // print($"{counter++}, I have finished a frame");
        }
        private class TextureToRender
        {
            public Portal portal;
            public Texture2D texture;
        }
        

        private void RenderCamera1(Portal inPortal, int iterationID, ScriptableRenderContext context, Camera cameraBeingReplicated)
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

        private void RenderCamera2(Portal inPortal,  ScriptableRenderContext context, Camera cameraBeingReplicated)
        {
            // Stack to store transformations
            Stack<CameraTransform> savedTransforms = new Stack<CameraTransform>();
            Transform cameraTransform;
            var stop = false;
            while (true)
            {
                // print($"{OnBeginCameraRenderingCount}.{RenderCameraCount}");
                // Save the current camera transformation to avoid recalculating it later
                if (savedTransforms.Count <= recursiveIterations 
                    && !stop 
                    )
                {
                    // Calculate and store the transformation
                    portalCameraController.SetCameraBeingReplicated(cameraBeingReplicated);
                    portalCameraController.SetPortalIn(inPortal.transform);
                    portalCameraController.SetPortalOut(inPortal.GetLinkedOutPortal().transform);
                    portalCameraController.SetPositionAndAngle();
                    portalCameraController.SetNearClippingPlane();

                    // Save the current transformation and projection matrix
                    var transformData = new CameraTransform
                    {
                        InPortalTexture = inPortal.GetRenderTexture(),
                        Position = (cameraTransform = _camera.transform).position,
                        Rotation = cameraTransform.rotation,
                        ProjectionMatrix = _camera.projectionMatrix
                    };
                    savedTransforms.Push(transformData);
                    if (Vector3.Dot(_camera.transform.forward, inPortal.transform.forward) > 0  || !_camera.IsLooking(inPortal.GetRenderer()) )
                    {
                        stop = true;
                    }
                }
                else
                {
                    // Use the saved transformation
                    while (savedTransforms.Count > 0)
                    {
                        var transformData = savedTransforms.Pop();
                        portalCameraController.SetPosition(transformData.Position);
                        portalCameraController.SetRotation(transformData.Rotation);
                        portalCameraController.SetProjectionMatrix(transformData.ProjectionMatrix);
                        portalCameraController.SetTargetTexture(transformData.InPortalTexture);
                        UniversalRenderPipeline.RenderSingleCamera(context, _camera);
                    }
                    
                    return;
                }

                // If there's a linked portal, continue the rendering process
                if (inPortal.GetLinkedOutPortal() != null)
                {
                    // Use the previously saved camera for the next iteration
                    cameraBeingReplicated = _camera;
                    continue;
                }

                break;
            }
        }

        private void RenderCamera(Portal inPortal,  int depth, ScriptableRenderContext context, Camera cameraBeingReplicated)
        {
            if (depth >= recursiveIterations)
                return;
            depth++;
            portalCameraController.SetCameraBeingReplicated(cameraBeingReplicated);
            portalCameraController.SetPortalIn(inPortal.transform);
            portalCameraController.SetPortalOut(inPortal.GetLinkedOutPortal().transform);
            portalCameraController.SetPositionAndAngle();
            portalCameraController.SetNearClippingPlane();
            var transformData1 = new CameraTransform
            {
                InPortalTexture = inPortal.GetRenderTexture(),
                Position = _camera.transform.position,
                Rotation = _camera.transform.rotation,
                ProjectionMatrix = _camera.projectionMatrix
            };
            if (depth < recursiveIterations)
            {
                Stack<TextureToRender> texturesToRender = new Stack<TextureToRender>();
                foreach (
                    var portal in PortalManager.allPortals.Where(
                        portal => portal.GetLinkedOutPortal() != null 
                                  && portal != inPortal.GetLinkedOutPortal()
                                  && Vector3.Dot((portal.transform.position - transformData1.Position).normalized, portal.transform.forward) < 0 
                                  && _camera.IsLooking(portal.GetRenderer())
                                  )
                        )
                {
                    
                    // print($"{counter++}, Going to print on {portal.name}. {inPortal.name} is connected to {inPortal.GetLinkedOutPortal().name} and I see {portal.name}, depth: {depth}. " );
                    RenderCamera(portal, depth, context, _camera);

                    // push texture and  corresponded portal 
                    var renderTexture = portal.GetRenderTexture();
                    // push texture and  corresponded portal 
                    var savedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

                    // Read pixels from the RenderTexture into the Texture2D
                    savedTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                    savedTexture.Apply();

                    // Reset the RenderTexture
                    RenderTexture.active = null;
                    texturesToRender.Push(new TextureToRender()
                    {
                        portal = portal,
                        texture =  savedTexture
                    });
                    
                    
                    // print($"{counter++}, saved texture to put on {portal.name}");
                    portalCameraController.SetPosition(transformData1.Position);
                    portalCameraController.SetRotation(transformData1.Rotation);
                    portalCameraController.SetProjectionMatrix(transformData1.ProjectionMatrix);
                    portalCameraController.SetTargetTexture(transformData1.InPortalTexture);
                }

                for (int i = 0; i < texturesToRender.Count; i++)
                {
                    // pop textures and set them to corresponding portal
                    var a = texturesToRender.Pop();
                    var m =a.portal.GetPortalMaterial();
                    m.mainTexture = a.texture;
                }

            }
                
            portalCameraController.SetPosition(transformData1.Position);
            portalCameraController.SetRotation(transformData1.Rotation);
            portalCameraController.SetProjectionMatrix(transformData1.ProjectionMatrix);
            portalCameraController.SetTargetTexture(transformData1.InPortalTexture);
            UniversalRenderPipeline.RenderSingleCamera(context, _camera);
            // print($"{counter++}, I have rendered on {inPortal.name}");
        }

        void OnBeginCameraRendering4(ScriptableRenderContext context, Camera camera)
        {
            if (camera != _portalManager.GetMainCamera())
                return;
        
            var seenPortals = PortalManager.allPortals
                .Where(portal => portal.GetLinkedOutPortal() != null && camera.IsLooking(portal.GetRenderer()));
        
            foreach (var portal in seenPortals)
            {
                RenderCamera4(portal, 0, context, camera);
            }
        }
        
        private void RenderCamera4(Portal inPortal, int iterationID, ScriptableRenderContext context, Camera cameraBeingReplicated)
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
        
                RenderCamera4(inPortal.GetLinkedOutPortal(), iterationID + 1, context, cameraBeingReplicated);
            }
        }
        
        private class CameraTransform
        {
            public RenderTexture InPortalTexture;
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
            public Matrix4x4 ProjectionMatrix { get; set; }
            public override string ToString()
            {
                return $"Position: {Position}, Rotation: {Rotation}, ProjectionMatrix: {ProjectionMatrix}, InPortalTexture: {(InPortalTexture != null ? InPortalTexture.name : "null")}";
            }
        }
        void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }
    
    }
}
