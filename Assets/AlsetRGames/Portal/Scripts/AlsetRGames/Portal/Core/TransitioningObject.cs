using System;
using System.Collections.Generic;
using UnityEngine;
using AlsetRGames.Portal.Support;
using JetBrains.Annotations;

namespace AlsetRGames.Portal.Core
{
    public class TransitioningObject 
    {
        private  Transform _original;
        [CanBeNull] private  Rigidbody _originalRigidbody;
        [CanBeNull] private  CharacterController _characterController;
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
            _characterController = _original.GetComponent<CharacterController>();
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
            if (_characterController != null)
            {
                _characterController.enabled = false;
                _characterController.Move(Vector3.zero);
            }
                
            _original.forward = _clone.forward;
            _original.rotation = _clone.rotation;
            _clone.gameObject.SetActive(false);
            _original.position = _clone.position;
            _original.localScale = _clone.lossyScale;
            

            if (!_originalRigidbody.Equals(null))
            {
                var newVelocity = PortalUtils.GetRelativeWorldDirection(_originalRigidbody.velocity, _portalIn.transform, _portalOut.transform);
                _originalRigidbody.velocity =  newVelocity ;
                _originalRigidbody.angularVelocity =   PortalUtils.GetRelativeWorldDirection(_originalRigidbody.angularVelocity, _portalIn.transform, _portalOut.transform); ;

            }
           
            _portalIn = portalIn;
            _portalOut = portalOut;
            (HasInPortalListener, HasOutPortalListener) = (HasOutPortalListener, HasInPortalListener);
            Transition._portalIn = portalIn;
            Transition._portalOut = portalOut;
            SetPosition();
            _clone.gameObject.SetActive(true);
            if(_characterController !=null)
                _characterController.enabled = true;
        }
        
        private void SetPosition()
        {
            var portalTransform = _portalIn.transform;
            var scale = portalTransform.localScale;
            foreach (var originalToClone in GetOriginalToCloneList())
            {
                if (originalToClone.clone.parent == null)
                {
                
                    //scale
                    originalToClone.clone.localScale =
                        GetLocalScaleAsIfParented(originalToClone.original, _portalIn.transform, _portalOut.transform);
                    // position
                    var objectToPortal = portalTransform.InverseTransformDirection(originalToClone.original.position - portalTransform.position) ;
                    var localPos = new Vector3(-objectToPortal.x* (1/_portalIn.transform.lossyScale.x), objectToPortal.y* (1/_portalIn.transform.lossyScale.y), -objectToPortal.z* (1/_portalIn.transform.lossyScale.z));
                    originalToClone.clone.position = _portalOut.transform.TransformPoint(localPos);
                
                    //rotation
                    var rotation = Quaternion.LookRotation(-portalTransform.forward, portalTransform.up);
                    var relativeRot = Quaternion.Inverse(rotation) * originalToClone.original.rotation;
                    originalToClone.clone.rotation =_portalOut.transform.rotation * relativeRot;
                }
                else
                {
                    originalToClone.clone.localScale = originalToClone.original.localScale;
                    originalToClone.clone.localRotation = originalToClone.original.localRotation;
                    originalToClone.clone.localPosition = originalToClone.original.localPosition;
                }
            }

        }
        public static Vector3 GetLocalScaleAsIfParented(Transform original, Transform portalIn, Transform portalOut )
        {
            // Get the object's world scale
            Vector3 originalGlobalScale = original.lossyScale;
            var portalOutGlobalScaleX = Math.Round(portalOut.lossyScale.x, 2);
            var portalOutGlobalScaleY = Math.Round(portalOut.lossyScale.y, 2);
            var portalOutGlobalScaleZ = Math.Round(portalOut.lossyScale.z, 2);
            // Calculate the hypothetical local scale by dividing the object's world scale
            // by the hypothetical parent's world scale
            Vector3 lossyScale = new Vector3( 
                (float)((portalOutGlobalScaleX/ Math.Round(portalIn.lossyScale.x, 2)) * Math.Round(originalGlobalScale.x, 2)),
                (float)((portalOutGlobalScaleY/ Math.Round(portalIn.lossyScale.y, 2)) *  Math.Round(originalGlobalScale.y, 2)),
                (float)(( portalOutGlobalScaleZ/ Math.Round(portalIn.lossyScale.z, 2)) *  Math.Round(originalGlobalScale.z, 2))
            );
            return lossyScale;
            
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
