using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AlsetRGames.Portal.Support;


namespace AlsetRGames.Portal.Core
{
    public class PortalDepth : MonoBehaviour
    {
        
        private Camera _camera;
        
        private PortalCameraController portalCameraController;
        [SerializeField] private int portalDepth;
        
        private PortalManager _portalManager;

        private Material _material;
        
        private Stack<TextureToRender> savedTextures = new Stack<TextureToRender>();
        private Stack<TextureToRender> savedOtherTextures = new Stack<TextureToRender>();
        
        private Stack<Texture2D> texturePool = new Stack<Texture2D>(); 


        void Start()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            _camera = gameObject.GetComponent<Camera>();
            portalCameraController = gameObject.GetComponent<PortalCameraController>();
            _portalManager = gameObject.GetComponent<PortalManager>();
        }
        void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != _portalManager.GetMainCamera())
                return;

            foreach (var portal in PortalManager.allPortals)
            {
                if (portal.GetLinkedOutPortal() == null ||
                    Vector3.Dot((portal.transform.position - camera.transform.position).normalized, portal.transform.forward) >= 0 ||
                    !camera.IsLooking(portal.GetRenderer()))
                {
                    continue; // Skip unnecessary portals
                }

                RenderCamera(portal, 0, context, camera);
                var renderTexture = portal.GetRenderTexture();

                // Get a texture from the pool or create a new one
                Texture2D savedTexture;
                if (texturePool.Count > 0)
                {
                    savedTexture = texturePool.Pop();
                    if (savedTexture.width != renderTexture.width || savedTexture.height != renderTexture.height)
                    {
                        savedTexture.Reinitialize(renderTexture.width, renderTexture.height);
                    }
                }
                else
                {
                    savedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                }

                // Copy RenderTexture contents to Texture2D (fast GPU copy)
                Graphics.CopyTexture(renderTexture, savedTexture);

                savedTextures.Push(new TextureToRender()
                {
                    portal = portal,
                    texture = savedTexture
                });
            }

            // Assign the saved textures and return them to the pool
            for (int i = savedTextures.Count; i > 0; i--)
            {
                var a = savedTextures.Pop();
                a.portal.GetPortalMaterial().mainTexture = a.texture;
                texturePool.Push(a.texture);
            }
        }
        void OnBeginCameraRendering2(ScriptableRenderContext context, Camera camera)
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
                
                
               
            }
            for (int i = 0; i < savedTextures.Count; i++)
            {
                // pop textures and set them to corresponding portal
                var a = savedTextures.Pop();
                var m =a.portal.GetPortalMaterial();
                m.mainTexture = a.texture;

            }
        }
        
        private class TextureToRender
        {
            public Portal portal;
            public Texture2D texture;
        }
        

private void RenderCamera(Portal inPortal, int depth, ScriptableRenderContext context, Camera cameraBeingReplicated)
{
    if (depth >= portalDepth)
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

    if (depth < portalDepth)
    {
        var customFrustumPlanes = _camera.GenerateCustomFrustumPlanes(inPortal.GetLinkedOutPortal());

        foreach (var portal in PortalManager.allPortals.Where(
            portal => portal.GetLinkedOutPortal() != null 
                      && portal != inPortal.GetLinkedOutPortal()
                      && Vector3.Dot((portal.transform.position - transformData1.Position).normalized, portal.transform.forward) < 0
                      && customFrustumPlanes.IsLookingThroughDoor(portal.GetRenderer())))
        {
            RenderCamera(portal, depth, context, _camera);

            // Get the render texture from the portal
            var renderTexture = portal.GetRenderTexture();
            
            // Reuse a texture from the pool or create a new one if needed
            Texture2D savedTexture;
            if (texturePool.Count > 0)
            {
                savedTexture = texturePool.Pop();
            }
            else
            {
                savedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            }

            // Efficiently copy texture without reading pixels
            Graphics.CopyTexture(renderTexture, savedTexture);

            // Store the texture for later use
            savedOtherTextures.Push(new TextureToRender()
            {
                portal = portal,
                texture = savedTexture
            });

            // Restore camera properties
            portalCameraController.SetPosition(transformData1.Position);
            portalCameraController.SetRotation(transformData1.Rotation);
            portalCameraController.SetProjectionMatrix(transformData1.ProjectionMatrix);
            portalCameraController.SetTargetTexture(transformData1.InPortalTexture);
        }

        // Assign stored textures back to portals
        while (savedOtherTextures.Count > 0)
        {
            var a = savedOtherTextures.Pop();
            a.portal.GetPortalMaterial().mainTexture = a.texture;
            
            // Return the texture to the pool for reuse
            texturePool.Push(a.texture);
        }
    }

    // Final rendering step
    portalCameraController.SetPosition(transformData1.Position);
    portalCameraController.SetRotation(transformData1.Rotation);
    portalCameraController.SetProjectionMatrix(transformData1.ProjectionMatrix);
    portalCameraController.SetTargetTexture(transformData1.InPortalTexture);
    UniversalRenderPipeline.RenderSingleCamera(context, _camera);
}

        private void RenderCamera2(Portal inPortal,  int depth, ScriptableRenderContext context, Camera cameraBeingReplicated)
        {
            if (depth >= portalDepth)
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
            if (depth < portalDepth)
            {
                var customFrustumPlanes = _camera.GenerateCustomFrustumPlanes(inPortal.GetLinkedOutPortal());
                
                foreach (
                    var portal in PortalManager.allPortals.Where(
                        portal => portal.GetLinkedOutPortal() != null 
                                  && portal != inPortal.GetLinkedOutPortal()
                                  && Vector3.Dot((portal.transform.position - transformData1.Position).normalized, portal.transform.forward) < 0 
                                  // && _camera.IsLooking(portal.GetRenderer())
                                  && customFrustumPlanes.IsLookingThroughDoor(portal.GetRenderer())
                                  )
                        )
                {
                    
                    
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
                    savedOtherTextures.Push(new TextureToRender()
                    {
                        portal = portal,
                        texture =  savedTexture
                    });
                    
                    portalCameraController.SetPosition(transformData1.Position);
                    portalCameraController.SetRotation(transformData1.Rotation);
                    portalCameraController.SetProjectionMatrix(transformData1.ProjectionMatrix);
                    portalCameraController.SetTargetTexture(transformData1.InPortalTexture);
                }

                for (int i = 0; i < savedOtherTextures.Count; i++)
                {
                    // pop textures and set them to corresponding portal
                    var a = savedOtherTextures.Pop();
                    var m =a.portal.GetPortalMaterial();
                    m.mainTexture = a.texture;
                }
            }
                
            portalCameraController.SetPosition(transformData1.Position);
            portalCameraController.SetRotation(transformData1.Rotation);
            portalCameraController.SetProjectionMatrix(transformData1.ProjectionMatrix);
            portalCameraController.SetTargetTexture(transformData1.InPortalTexture);
            UniversalRenderPipeline.RenderSingleCamera(context, _camera);
            
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
