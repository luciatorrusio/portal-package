using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using AlsetRGames.Portal.Support;

namespace AlsetRGames.Portal.Core
{
    public class Portal : MonoBehaviour
    {
        #region declarations

        [SerializeField] [CanBeNull] private Portal linkedOutPortal = null;

        [HideInInspector]
        [SerializeField]private List<Portal> linkedInPortals = new List<Portal>();
        
        [SerializeField] private Material defaultMaterial;

        [SerializeField] private PortalUtils.PortalMode portalMode; 
    
        [SerializeField]
#if UNITY_EDITOR
        [ShowOnlyIf("portalMode", PortalUtils.PortalMode.NO_IMAGE)]
#endif
        private Material portalMaterial;

        [SerializeField] private PortalDependencies dependencies = new PortalDependencies();
        
        [Serializable]
        public class PortalDependencies
        {
            [SerializeField] private GameObject renderGameObject;
            private PortalTextureSetup portalTextureSetup;
            private MeshFilter renderPlane;
            private Renderer _renderer;
            private PortalTransport portalTransport;
            public GameObject RenderGameObject => renderGameObject;
            public MeshFilter RenderPlane => renderPlane;
            public Renderer Renderer => _renderer;
            public PortalTransport PortalTransport
            {
                get => portalTransport;
                set => portalTransport = value;
            }
            public PortalTextureSetup PortalTextureSetup => portalTextureSetup;
            
            public void SetRenderGameObjectComponents()
            {
                portalTextureSetup = renderGameObject.GetComponent<PortalTextureSetup>();
                renderPlane = renderGameObject.GetComponent<MeshFilter>();
                _renderer = renderGameObject.GetComponent<Renderer>();
            }
        }
        #endregion

    
        void Start()
        {
            
            if (linkedOutPortal != null)
                SetAsInPortal();
			else
            {
                //dependencies.PortalTextureSetup.gameObject.SetActive(true);
				dependencies.PortalTextureSetup.SetDefaultMaterial();
			}
        }

        private void OnValidate()
        {
            try
            {
                SetDependencies();
            }
            catch (Exception ex)
            {
                Debug.Log("ex:"+ex.Message);
            }
            
        }

        private void SetDependencies()
        {
            if (dependencies.RenderGameObject == null)
                return;
            dependencies.SetRenderGameObjectComponents();
            dependencies.PortalTransport = GetComponent<PortalTransport>();
        }

        #region API
        public void SetLinkedOutPortal(Portal newLinkedOutPortal)
        {
            
            if(newLinkedOutPortal != null)
            {
                if(linkedOutPortal != null)
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
            if(linkedOutPortal != null)
                linkedOutPortal.RemoveLinkedInPortal(this);
            linkedOutPortal = null;
            dependencies.PortalTextureSetup.SetDefaultMaterial();
        }
        public Portal GetLinkedOutPortal()
        {
            return linkedOutPortal;
        }
        
        public void UpdateDefaultMaterial(Material material)
        {
            dependencies.PortalTextureSetup.UpdateDefaultMaterial(material);
        }
        public void SetPortalMaterial(Material material)
        {
            dependencies.PortalTextureSetup.SetPortalMaterial(material);
        }
        public Material GetPortalMaterial()
        {
            return dependencies.PortalTextureSetup.GetPortalMaterial();
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
                    dependencies.PortalTransport.enabled = true;
                    PortalManager.AddPortal(this);
                    break;
                case PortalUtils.PortalMode.NO_TRANSPORTATION:
                    dependencies.PortalTransport.enabled = false;
                    PortalManager.AddPortal(this);
                    break;
                case PortalUtils.PortalMode.NO_IMAGE: 
                    dependencies.PortalTransport.enabled = true;
                    PortalManager.RemovePortal(this);
                    dependencies.PortalTextureSetup.SetPortalMaterial(portalMaterial);
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
            dependencies.PortalTextureSetup.UpdateDefaultMaterial(defaultMaterial);
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
            dependencies.PortalTextureSetup.gameObject.SetActive(true);
            dependencies.PortalTextureSetup.SetCameraMaterial();
            gameObject.SetActive(true);
            linkedOutPortal.AddLinkedInPortal(this);
        
            UpdatePortalMode(portalMode);
        }

        public void UpdateTransitioningObjects()
        {
            dependencies.PortalTransport.UpdateTransitioningObjects();
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (linkedOutPortal != null)
            {
                var localScale = transform.lossyScale;
                var linkedOutPortalTransform = linkedOutPortal.transform;
                var position = transform.position;
                GizmosExtended.DrawPlane(transform, new Vector2(dependencies.RenderPlane.sharedMesh.bounds.size.x * localScale.x, dependencies.RenderPlane.sharedMesh.bounds.size.z * localScale.y), Color.green);
                GizmosExtended.DrawPlane(linkedOutPortal.transform, new Vector2(linkedOutPortal.dependencies.RenderPlane.sharedMesh.bounds.size.x * linkedOutPortalTransform.lossyScale.x, linkedOutPortal.dependencies.RenderPlane.sharedMesh.bounds.size.z * linkedOutPortalTransform.lossyScale.y), Color.red);
                GizmosExtended.DrawArrow(position,linkedOutPortal.transform.position- position, Color.yellow, 2f, 40f);
                GizmosExtended.DrawArrow(linkedOutPortalTransform.position ,linkedOutPortalTransform.forward , Color.red);
                GizmosExtended.DrawArrow(position+(transform.forward* 1f), -transform.forward, Color.green);
            }
            
        
        }
#endif
    
    
        private void OnEnable()
        {
            SetDependencies();
            PortalManager.AddPortal(this);
        }
        private void OnDisable()
        {
            PortalManager.RemovePortal(this);
        }


        public RenderTexture GetRenderTexture()
        {
            return dependencies.PortalTextureSetup.GetRenderTexture();
        }
        
        public Renderer GetRenderer()
        {
            return dependencies.Renderer;
        }


        public void AddTransitioningObject(TransitioningObject transitioningObject)
        {
            dependencies.PortalTransport.AddTransitioningObject(transitioningObject);
        }
    
    
    }
}

