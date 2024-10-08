using System.Collections.Generic;
using Core.Portal.Editor;
using Core.Portal.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.Portal.Scripts
{
    public class Portal : MonoBehaviour
    {
        #region declarations
        [HideInInspector]
        [SerializeField] private PortalTextureSetup portalTextureSetup;
    
        [SerializeField] [CanBeNull] private Portal linkedOutPortal = null;

        [HideInInspector]
        [SerializeField]private List<Portal> linkedInPortals = new List<Portal>();
        [HideInInspector]
        [SerializeField] private MeshFilter renderPlane;

        [HideInInspector] 
        [SerializeField] private Renderer _renderer;
        [HideInInspector]
        [SerializeField] private PortalTransport portalTransport;

        [SerializeField] private Mesh PortalMesh;
        [SerializeField] private Material defaultMaterial;

        [SerializeField] private PortalUtils.PortalMode portalMode; 
    
        [SerializeField]
#if UNITY_EDITOR
        [ShowOnlyIf("portalMode", PortalUtils.PortalMode.NO_IMAGE)]
#endif
        private Material portalMaterial;
        #endregion

    
        void Start()
        {
            if (linkedOutPortal != null)
                SetAsInPortal();
        }

        #region API
        public void SetLinkedOutPortal(Portal newLinkedOutPortal)
        {
            if(newLinkedOutPortal != null)
            {
                linkedOutPortal.RemoveLinkedInPortal(this);
                linkedOutPortal = newLinkedOutPortal;
                linkedOutPortal.AddLinkedInPortal(this);
                SetAsInPortal();
            }
            else
            {
                RemoveLinkedOutPortal();
            }
        }
        public void RemoveLinkedOutPortal()
        {
            linkedOutPortal.RemoveLinkedInPortal(this);
            linkedOutPortal = null;
            portalTextureSetup.SetDefaultMaterial();
        }
        public Portal GetLinkedOutPortal()
        {
            return linkedOutPortal;
        }
        public void SetPortalMesh()
        {
            if(PortalMesh == null)
                Debug.LogWarning("Mesh is null in " + gameObject.name);
            renderPlane.mesh = PortalMesh;
        }
        public void UpdateDefaultMaterial(Material material)
        {
            portalTextureSetup.UpdateDefaultMaterial(material);
        }
        public void SetPortalMaterial(Material material)
        {
            portalTextureSetup.SetPortalMaterial(material);
        }
        public Material GetPortalMaterial()
        {
            return portalTextureSetup.GetPortalMaterial();
        }
        public IEnumerable<Portal> GetLinkedInPortals()
        {
            return linkedInPortals;
        }

        public void UpdatePortalMode(PortalUtils.PortalMode newMode)
        {
            portalMode = newMode;
            switch (portalMode)
            {
                case PortalUtils.PortalMode.FULL_FUNCTION:
                    portalTransport.enabled = true;
                    PortalManager.AddPortal(this);
                    break;
                case PortalUtils.PortalMode.NO_TRANSPORTATION:
                    portalTransport.enabled = false;
                    PortalManager.AddPortal(this);
                    break;
                case PortalUtils.PortalMode.NO_IMAGE: 
                    portalTransport.enabled = true;
                    PortalManager.RemovePortal(this);
                    portalTextureSetup.SetPortalMaterial(portalMaterial);
                    break;
            }
        }

        public PortalUtils.PortalMode GetPortalMode()
        {
            return portalMode;
        }
        #endregion

        /// <summary>
        /// This function is not intended for use.
        /// </summary>
        public void UpdateDefaultMaterial()
        {
            portalTextureSetup.UpdateDefaultMaterial(defaultMaterial);
        }
        private void AddLinkedInPortal(Portal portal)
        {
            linkedInPortals.Add(portal);
        }

        private void RemoveLinkedInPortal(Portal portal)
        {
            linkedInPortals.Remove(portal);
        }

        private void SetAsInPortal()
        {
            portalTextureSetup.gameObject.SetActive(true);
            portalTextureSetup.SetCameraMaterial();
            gameObject.SetActive(true);
            linkedOutPortal.AddLinkedInPortal(this);
        
            UpdatePortalMode(portalMode);
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (linkedOutPortal != null)
            {
                var localScale = transform.localScale;
                var linkedOutPortalTransform = linkedOutPortal.transform;
                var position = transform.position;
                GizmosExtended.DrawPlane(transform, new Vector2(PortalMesh.bounds.size.x * localScale.x, PortalMesh.bounds.size.z * localScale.y), Color.green);
                GizmosExtended.DrawPlane(linkedOutPortal.transform, new Vector2(linkedOutPortal.PortalMesh.bounds.size.x * linkedOutPortalTransform.localScale.x, linkedOutPortal.PortalMesh.bounds.size.z * linkedOutPortalTransform.localScale.y), Color.red);
                GizmosExtended.DrawArrow(position,linkedOutPortal.transform.position- position, Color.yellow, 2f, 40f);
                GizmosExtended.DrawArrow(linkedOutPortalTransform.position ,linkedOutPortalTransform.forward , Color.red);
                GizmosExtended.DrawArrow(position+(transform.forward* 1f), -transform.forward, Color.green);
            }
            
        
        }
#endif
    
    
        private void OnEnable()
        {
            PortalManager.AddPortal(this);
        }
        private void OnDisable()
        {
            PortalManager.RemovePortal(this);
        }


        public RenderTexture GetRenderTexture()
        {
            return portalTextureSetup.GetRenderTexture();
        }
        
        public Renderer GetRenderer()
        {
            return _renderer;
        }


        public void AddTransitioningObject(TransitioningObject transitioningObject)
        {
            portalTransport.AddTransitioningObject(transitioningObject);
        }
    
    
    }
}

