using UnityEngine;
using AlsetRGames.Portal.Core;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AlsetRGames.Portal.Core
{
    [RequireComponent(typeof(Renderer))] 
    public class PortalTextureSetup : MonoBehaviour
    {
        private  Camera cameraOut;
        private Material _portalInMat;
        private Material _defaultMaterial;
        
        [SerializeField] private Shader shader;

        private RenderTexture _targetTexture;
        
        private Renderer renderer;
		void Awake(){
			renderer = gameObject.GetComponent<Renderer>();
		}
    
        // crea el material que tendra el plano y se lo pone al plano
        public void SetCameraMaterial()
        {
            _portalInMat = new Material(shader);
            renderer.material = _portalInMat;
            _targetTexture = new RenderTexture(Screen.width, Screen.height, 24,  RenderTextureFormat.ARGB32);
            _targetTexture.antiAliasing = 4;
            _portalInMat.mainTexture = _targetTexture;
        }
    
    

        public RenderTexture GetRenderTexture()
        {
            return _targetTexture;
        }
    
        public void UpdateDefaultMaterial(Material material)
        {
            _defaultMaterial = material;
        }

        public void SetDefaultMaterial()
        {
            if(renderer == null)
                renderer = gameObject.GetComponent<Renderer>();
            renderer.material = _defaultMaterial;
        }

        public void SetPortalMaterial(Material material)
        {
            renderer.material = material;
        }
        
        public Material GetPortalMaterial()
        {
            return renderer.material;
        }
    }
}
