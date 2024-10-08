using UnityEngine;

namespace Core.Portal.Scripts
{
    [RequireComponent(typeof(Renderer))] 
    public class PortalTextureSetup : MonoBehaviour
    {
        private  Camera cameraOut;
        private Material _portalInMat;
        [SerializeField] private Material _defaultMaterial;
        [HideInInspector]
        [SerializeField] private Shader shader;

        private RenderTexture _targetTexture;
        [HideInInspector]
        [SerializeField] private Renderer renderer;
    
        // crea el material que tendra el plano y se lo pone al plano
        public void SetCameraMaterial()
        {
            _portalInMat = new Material(shader);
            renderer.material = _portalInMat;
            _targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
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
