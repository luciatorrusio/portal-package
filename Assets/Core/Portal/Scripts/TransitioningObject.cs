using System.Collections.Generic;
using Core.Portal.Utils;
using UnityEngine;

namespace Core.Portal.Scripts
{
    public class TransitioningObject 
    {
        private  Transform _original;
        private  Rigidbody _originalRigidbody;
        private  Transform _clone;
        private readonly GameObject _mainCamera;
        private  Portal  _portalIn;
        private Portal _portalOut;
        private readonly bool _implementsTransitionListener;
        public  bool HasInPortalListener { get; private set; }
        public bool HasOutPortalListener { get; private set; }
        private readonly IEnumerable<( Transform original, Transform clone)> _originalToCloneList;
        private readonly List<Material> _originalMaterials = new List<Material>();
        private readonly List<Material> _cloneMaterials = new List<Material>();
        public Transition Transition { get; }

        public TransitioningObject(Transform original,Transform clone, Portal portalIn, Portal portalOut,IEnumerable<( Transform original, Transform clone)> originalToCloneList,  bool implementsTransitionListener)
        {
            _original = original;
            _originalRigidbody = _original.GetComponent<Rigidbody>();
            _clone = clone;
            _mainCamera = _original.GetMainCamera();
            _portalIn = portalIn;
            _portalOut = portalOut;
            _originalToCloneList= originalToCloneList;
            _implementsTransitionListener = implementsTransitionListener;
            HasInPortalListener = portalIn.GetComponent<TransitionListener>() != null;
            HasOutPortalListener = portalOut.GetComponent<TransitionListener>() != null;
            _cloneMaterials.AddRange(SetMaterials(clone.gameObject));
            _originalMaterials.AddRange(SetMaterials(original.gameObject));
            Transition = new Transition(_original, _clone, _portalIn, _portalOut);
        }
    

        public Transform GetOriginal()
        {
            return _original;
        }

        public GameObject GetMainCamera()
        {
            return _mainCamera;
        }
    
        public Transform GetClone()
        {
            return _clone;
        }

        public bool GetImplementsTransitionListener()
        {
            return _implementsTransitionListener;
        }

        public Rigidbody GetOriginalRigidbody()
        {
            return _originalRigidbody;
        }

        public Portal GetPortalIn()
        {
            return _portalIn;
        }

    
        public void SwitchPortals(Portal portalIn, Portal portalOut)
        {
            _original.forward = _clone.forward;
            _original.rotation = _clone.rotation;
            _clone.gameObject.SetActive(false);
            _original.position = _clone.position;
            _original.localScale = _clone.lossyScale;

        
        
            var newVelocity = PortalUtils.GetRelativeWorldDirection(_originalRigidbody.velocity, _portalIn.transform, _portalOut.transform);
            _originalRigidbody.velocity =  newVelocity ;
            _originalRigidbody.angularVelocity =   PortalUtils.GetRelativeWorldDirection(_originalRigidbody.angularVelocity, _portalIn.transform, _portalOut.transform); ;

            _clone.parent = portalOut.transform;
            _portalIn = portalIn;
            _portalOut = portalOut;
            (HasInPortalListener, HasOutPortalListener) = (HasOutPortalListener, HasInPortalListener);
            Transition._portalIn = portalIn;
            Transition._portalIn = portalOut;
            SetPosition();
            _clone.gameObject.SetActive(true);
        }
        
        private void SetPosition()
        {
            var portalTransform = _portalIn.transform;
            var scale = portalTransform.localScale;
            foreach (var originalToClone in GetOriginalToCloneList())
            {
                if (originalToClone.clone.parent == GetPortalOut().transform)
                {
                
                    //scale
                    var localScale = originalToClone.original.localScale;
                    originalToClone.clone.localScale = new Vector3(localScale.x* (1/scale.y),localScale.y* (1/scale.x), localScale.z* (1/scale.z) );
               
                    // position
                    var objectToPortal = portalTransform.InverseTransformDirection(originalToClone.original.position - portalTransform.position) ;
                    var localPos = new Vector3(-objectToPortal.x* (1/scale.x), objectToPortal.y* (1/scale.y), -objectToPortal.z* (1/scale.z));
                    originalToClone.clone.localPosition =localPos;
                
                    //rotation
                    var rotation = Quaternion.LookRotation(-portalTransform.forward, portalTransform.up);
                    var relativeRot = Quaternion.Inverse(rotation) * originalToClone.original.rotation;
                    originalToClone.clone.rotation =_portalIn.GetLinkedOutPortal().transform.rotation * relativeRot;
                }
                else
                {
                    originalToClone.clone.localScale = originalToClone.original.localScale;
                    originalToClone.clone.localRotation = originalToClone.original.localRotation;
                    originalToClone.clone.localPosition = originalToClone.original.localPosition;
                }
            }

        }

    
        public Portal GetPortalOut()
        {
            return _portalOut;
        }
    
        public IEnumerable<( Transform original, Transform clone)> GetOriginalToCloneList()
        {
            return _originalToCloneList;
        }


        public List<Material> GetCloneMaterials()
        {
            return _cloneMaterials;
        }
        public List<Material> GetOriginalMaterials()
        {
            return _originalMaterials;
        }
    
        private List<Material> SetMaterials (GameObject g) {
            var renderers = g.GetComponentsInChildren<MeshRenderer> ();
            var matList = new List<Material> ();
            foreach (var renderer in renderers) {
                foreach (var mat in renderer.materials) {
                    matList.Add (mat);
                }
            }

            var skinnedRenderers = g.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in skinnedRenderers)
            {
                foreach (var mat in renderer.materials)
                {
                    matList.Add(mat);
                }
            }

            return matList;

        }
    }
}
