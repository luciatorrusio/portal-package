#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using AlsetRGames.Portal.Support;

namespace AlsetRGames.Portal.Core
{
    [RequireComponent(typeof(Collider))]
    public class PortalTransport : MonoBehaviour
    {
        [SerializeField] private Portal portal;
        private readonly List<TransitioningObject> _objectsOnPortal = new List<TransitioningObject>();
        
        [SerializeField] private GameObject emptyClone;

        private static readonly int PortalNormal = Shader.PropertyToID("_portalNormal");
        private static readonly int PortalCenter = Shader.PropertyToID("_portalCenter");

        private void Start()
        {
            if (portal == null)
                throw new Exception("Portal In must be initialized in editor");
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!enabled)
                return;
            if(portal.GetLinkedOutPortal() == null)
                return;
            var objectCrossing = other.gameObject;
            if (IsTransitioningObject(objectCrossing))
                return;
        
            var rigidbody = objectCrossing.GetComponent<Rigidbody>();
            if (rigidbody== null)
                return;
            if(!rigidbody.worldCenterOfMass.IsInFrontOf(portal.transform))
                return;
            CreateClone(objectCrossing);
        }

    
        private bool IsTransitioningObject(GameObject go)
        {
            return _objectsOnPortal.FindIndex(item => item.GetClone().gameObject.Equals(go) || item.GetOriginal().gameObject.Equals(go) ) != -1;
        }    

        private void CreateClone(GameObject objectCrossing)
        {
            if(portal.GetLinkedOutPortal() == null)
                return;
            var customClone = objectCrossing.GetComponent<CustomClone>();
        
            var cloneMode = customClone?.GetMode() ?? PortalUtils.CloneMode.AUTOMATIC;
        
            GameObject clone;
            var originalToClone = new List<(Transform original, Transform clone)>();
            switch (cloneMode)
            {
                case PortalUtils.CloneMode.CUSTOM:
                    clone = customClone.CreateClone(objectCrossing, portal.transform, portal.GetLinkedOutPortal().transform, originalToClone);
                    break;
                default:
                    clone = CreateGameObjectTree(objectCrossing, portal.GetLinkedOutPortal().transform, originalToClone, true);
                    break;
            }
        
            ForwardEvents(clone, objectCrossing);
            var transitionListener = objectCrossing.GetComponent<TransitionListener>();
        
            var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portal,portal.GetLinkedOutPortal(), originalToClone, transitionListener!=null );
            _objectsOnPortal.Add(objectOnPortal);
            TriggerOnPortalEnter(objectOnPortal);
        
        }

        public void AddTransitioningObject(TransitioningObject transitioningObject)
        {
            _objectsOnPortal.Add(transitioningObject);
        }

        private GameObject CreateGameObjectTree(GameObject objectCrossing, Transform portalOut,List<(Transform original, Transform clone)> originalToClone,  bool firstIteration)
        {
            var objectToPortal = portal.transform.InverseTransformDirection(objectCrossing.transform.position - portal.transform.position);
            var localPosition = new Vector3(-objectToPortal.x, objectToPortal.y, -objectToPortal.z);
            var clone = Instantiate(emptyClone, portal.GetLinkedOutPortal().transform.position + localPosition, objectCrossing.transform.localRotation);
            originalToClone.Add( (objectCrossing.transform, clone.transform));
            clone.name = objectCrossing.name + ("(Portal)");
            DuplicateMesh(objectCrossing, clone, originalToClone, firstIteration, portal.transform, portalOut);
            for (int i = 0; i < objectCrossing.transform.childCount; i++)
            {
                CreateGameObjectTree(objectCrossing.transform.GetChild(i).gameObject, clone.transform, originalToClone, false);
            }
            return clone;
        }
        private static void DuplicateMesh(GameObject original, GameObject clone,List<(Transform original, Transform clone)> originalToClone,  bool firstIteration, Transform portalIn, Transform portalOut)
        {
            CopyTransform(original.transform, clone.transform, firstIteration, portalIn, portalOut);
            CopyMesh(original, clone, originalToClone);
            CopyCollider(original, clone);
        }

        private static void CopyMesh(GameObject original, GameObject clone, List<(Transform original, Transform clone)> originalToClone)
        {
            var originalMesh = original.GetComponent<Renderer>();
            if(originalMesh == null)
                return;
            if(originalMesh.GetType() == typeof(MeshRenderer))
                CopyRenderer(originalMesh, clone, original);
            else if(originalMesh.GetType() == typeof(SkinnedMeshRenderer))
            {
                // print("Skinned mesh renderer");
                CopySkinnedMeshRenderer((SkinnedMeshRenderer)originalMesh, clone, originalToClone);
            }
        
        }
        private static void CopySkinnedMeshRenderer(SkinnedMeshRenderer originalRenderer, GameObject newObject, List<(Transform original, Transform clone)> originalToClone)
        {
        
            var newRenderer = newObject.AddComponent<SkinnedMeshRenderer>();
            newRenderer.forceMatrixRecalculationPerRender = true;
            newRenderer.sharedMesh = originalRenderer.sharedMesh;
            newRenderer.materials = originalRenderer.materials;
            newRenderer.enabled = originalRenderer.enabled;
        
            var cloneLookup = originalToClone.ToDictionary(pair => pair.original, pair => pair.clone);

            var bones = new List<Transform>();
            foreach (var bone in originalRenderer.bones)
            {
                if (cloneLookup.TryGetValue(bone, out var cloneBone))
                {
                    bones.Add(cloneBone);
                }
            }
        

            newRenderer.bones = bones.ToArray();
            if (cloneLookup.TryGetValue(originalRenderer.rootBone, out var rootBone))
            {
                newRenderer.rootBone = rootBone;
            }
            newRenderer.quality = originalRenderer.quality;
            newRenderer.updateWhenOffscreen = originalRenderer.updateWhenOffscreen;
            newRenderer.localBounds = originalRenderer.localBounds;
        }

        private static void CopyRenderer(Renderer originalMesh, GameObject clone, GameObject original)
        {
            var cloneMesh = clone.AddComponent<MeshRenderer>();
            cloneMesh.sharedMaterials = originalMesh.sharedMaterials;
            cloneMesh.enabled = originalMesh.enabled;
            var originalMeshFilter = original.GetComponent<MeshFilter>();
            if(originalMeshFilter == null)
                return;
            var cloneMeshFilter = clone.AddComponent<MeshFilter>();
            cloneMeshFilter.sharedMesh = Instantiate(originalMeshFilter.sharedMesh);
        }

        private static void CopyTransform(Transform original, Transform clone, bool firstIteration,Transform portalIn, Transform portalOut)
        {
            clone.localScale = GetLocalScaleAsIfParented(original,portalIn, portalOut );
            if(firstIteration)
                return;
            //clone.position = original.position;
        }

        private static void CopyCollider(GameObject original, GameObject clone)
        {
            var originalCollider = original.GetComponent<Collider>();
        
            if(originalCollider == null)
                return;
            var cloneCollider = (Collider)clone.AddComponent(originalCollider.GetType());
            cloneCollider.enabled = originalCollider.enabled;
            cloneCollider.material = originalCollider.material;
            cloneCollider.isTrigger =originalCollider.isTrigger;
            cloneCollider.sharedMaterial = originalCollider.sharedMaterial;
        
            if (cloneCollider.GetType() == typeof(MeshCollider))
            {
                var mesh = (MeshCollider)cloneCollider;
                mesh.sharedMesh = ((MeshCollider)originalCollider).sharedMesh;
                mesh.convex = ((MeshCollider)originalCollider).convex;

            }
            else if(cloneCollider.GetType() == typeof(BoxCollider))
            {
                var mesh = (BoxCollider)cloneCollider;
                mesh.center = ((BoxCollider)originalCollider).center;
                mesh.size = ((BoxCollider)originalCollider).size;
            
            }
            else if(cloneCollider.GetType() == typeof(CapsuleCollider))
            {
                var mesh = (CapsuleCollider)cloneCollider;
                mesh.center = ((CapsuleCollider)originalCollider).center;
                mesh.direction = ((CapsuleCollider)originalCollider).direction;
                mesh.height = ((CapsuleCollider)originalCollider).height;
                mesh.radius = ((CapsuleCollider)originalCollider).radius;
            
            }
            else if(cloneCollider.GetType() == typeof(SphereCollider))
            {
                var mesh = (SphereCollider)cloneCollider;
                mesh.center = ((SphereCollider)originalCollider).center;
                mesh.radius = ((SphereCollider)originalCollider).radius;
            }
            else if(cloneCollider.GetType() == typeof(WheelCollider))
            {
                var mesh = (WheelCollider)cloneCollider;
                mesh.center = ((WheelCollider)originalCollider).center;
                mesh.radius = ((WheelCollider)originalCollider).radius;
                mesh.mass = ((WheelCollider)originalCollider).mass;
                mesh.brakeTorque = ((WheelCollider)originalCollider).brakeTorque;
                mesh.forwardFriction = ((WheelCollider)originalCollider).forwardFriction;
                mesh.motorTorque = ((WheelCollider)originalCollider).motorTorque;
                mesh.sidewaysFriction = ((WheelCollider)originalCollider).sidewaysFriction;
                mesh.sprungMass = ((WheelCollider)originalCollider).sprungMass;
                mesh.steerAngle = ((WheelCollider)originalCollider).steerAngle;
                mesh.suspensionDistance = ((WheelCollider)originalCollider).suspensionDistance;
                mesh.wheelDampingRate = ((WheelCollider)originalCollider).wheelDampingRate;
                mesh.forceAppPointDistance = ((WheelCollider)originalCollider).forceAppPointDistance;
            }
        }

        private void ForwardEvents(GameObject clone, GameObject objectCrossing)
        {
            var eventForwarder = clone.AddComponent<EventForwarder>();
            eventForwarder.SetOriginalObject(objectCrossing);
        }

        private void ExitPortal(TransitioningObject leavingPortal)
        {
            if(leavingPortal.GetClone() == null)
                return;
            StopCropMaterial(leavingPortal);
            TriggerOnPortalExit(leavingPortal);
            Destroy(leavingPortal.GetClone().gameObject); 
        }


        private void OnTriggerExit(Collider other)
        {
            if(!enabled || portal == null || portal.GetLinkedOutPortal() == null)
                return;
            TransitioningObject? leavingPortal = GetObjectOnPortalLeaving(other.gameObject);
            if (leavingPortal == null ||  leavingPortal.GetClone()==null)
                return;
            _objectsOnPortal.Remove(leavingPortal);
            if(!CrossPortal(leavingPortal))
                ExitPortal(leavingPortal);

        }
        # region TRIGGERS
        private void TriggerOnPortalEnter(TransitioningObject enteringPortal)
        {
            if(enteringPortal.GetImplementsTransitionListener())
                enteringPortal.GetOriginal().SendMessage("OnPortalEnter", enteringPortal.Transition);
            if(enteringPortal.HasInPortalListener)
                enteringPortal.GetPortalIn().SendMessage("OnPortalEnter", enteringPortal.Transition);
            if(enteringPortal.HasOutPortalListener)
                enteringPortal.GetPortalOut().SendMessage("OnPortalEnter", enteringPortal.Transition);
        }
        private void TriggerOnPortalExit(TransitioningObject leavingPortal)
        {
            if(leavingPortal.GetImplementsTransitionListener())
                leavingPortal.GetOriginal().SendMessage("OnPortalExit", leavingPortal.Transition);
            if(leavingPortal.HasInPortalListener)
                leavingPortal.GetPortalIn().SendMessage("OnPortalExit", leavingPortal.Transition);
            if(leavingPortal.HasOutPortalListener)
                leavingPortal.GetPortalOut().SendMessage("OnPortalExit", leavingPortal.Transition);
        }
        private void TriggerOnPortalTransitioning(TransitioningObject transitioningPortal)
        {
            if(transitioningPortal.GetImplementsTransitionListener())
                transitioningPortal.GetOriginal().SendMessage("OnPortalTransitioning", transitioningPortal.Transition);
            if(transitioningPortal.HasInPortalListener)
                transitioningPortal.GetPortalIn().SendMessage("OnPortalCrossed", transitioningPortal.Transition);
            if(transitioningPortal.HasOutPortalListener)
                transitioningPortal.GetPortalOut().SendMessage("OnPortalCrossed", transitioningPortal.Transition);
        
        }
        private void TriggerOnPortalCrossed(TransitioningObject crossingPortal)
        {
            if(crossingPortal.GetImplementsTransitionListener())
                crossingPortal.GetOriginal().SendMessage("OnPortalCrossed", crossingPortal.Transition);
            if(crossingPortal.HasInPortalListener)
                crossingPortal.GetPortalIn().SendMessage("OnPortalCrossed", crossingPortal.Transition);
            if(crossingPortal.HasOutPortalListener)
                crossingPortal.GetPortalOut().SendMessage("OnPortalCrossed", crossingPortal.Transition);
        }
    
        # endregion
        private TransitioningObject? GetObjectOnPortalLeaving(GameObject o)
        {
            var objectOnPortalLeavingIndex = _objectsOnPortal.FindIndex(item => item.GetOriginal().gameObject.Equals(o) );
            return objectOnPortalLeavingIndex == -1 ? null : _objectsOnPortal[objectOnPortalLeavingIndex];
        }


        public void UpdateTransitioningObjects()
        {
            if(portal.GetLinkedOutPortal() == null)
                return;
            for (var j = 0; j < _objectsOnPortal.Count ; j++)
            {
                var t = _objectsOnPortal[j];
                CrossPortal(t);
                TriggerOnPortalTransitioning(t);
            }
        }
        private void LateUpdate()
        {
            if(portal.GetLinkedOutPortal() == null)
                return;
            for (var j = 0; j < _objectsOnPortal.Count ; j++)
            {
                var t = _objectsOnPortal[j];
                CrossPortal(t);
                TriggerOnPortalTransitioning(t);
            }
        }
        private bool CrossPortal(TransitioningObject t)
        {
            ReplicateTransform(t);
            var worldCenterOfMass = t.GetOriginal().position + t.GetOriginalRigidbody().centerOfMass;
            if (!worldCenterOfMass.IsInFrontOfWithError(portal.transform, 0.1f))
            {
                t.GetPortalOut().AddTransitioningObject(t);
                t.SwitchPortals( portal.GetLinkedOutPortal(),portal.GetLinkedOutPortal().GetLinkedOutPortal());
                _objectsOnPortal.Remove(t);
                TriggerOnPortalCrossed(t);
                // t.GetOriginal().localScale = new Vector3(t.GetClone().localScale.x* (t.GetPortalOut().transform.localScale.x),t.GetClone().localScale.y* (t.GetPortalOut().transform.localScale.y), t.GetClone().localScale.z* (t.GetPortalOut().transform.localScale.x) );

                return true;
                
            }

            return false;
            // if (t.GetMainCamera() != null)
            // {
            //     
            //     if (!t.GetMainCamera().transform.position.IsInFrontOfWithError(portal.transform, 0.1f))
            //     // if (!t.GetMainCamera().transform.IsInFrontOf(portal.transform))
            //     {
            //         t.GetPortalOut().AddTransitioningObject(t);
            //         t.SwitchPortals( portal.GetLinkedOutPortal(),portal.GetLinkedOutPortal().GetLinkedOutPortal());
            //         _objectsOnPortal.Remove(t);
            //         TriggerOnPortalCrossed(t);
            //         return;
            //     }
            // }
        }

    
    
    
        private void ReplicateTransform(TransitioningObject transitioningObject)
        {
            if(transitioningObject.GetClone()!= null )
            {
                SetPosition(transitioningObject);
                var portalTransform = portal.transform;
                CropMaterial(transitioningObject, portal.GetLinkedOutPortal().transform.position, portal.GetLinkedOutPortal().transform.forward, portalTransform.position, portalTransform.forward);   
            }
        }

        private void SetPosition(TransitioningObject transitioningObject)
        {
            var portalTransform = portal.transform;
            var portalInGlobalScale = portalTransform.lossyScale;
            foreach (var originalToClone in transitioningObject.GetOriginalToCloneList())
            {
                if (originalToClone.clone.parent == null)
                {
                
                    //scale
                    originalToClone.clone.localScale =
                        GetLocalScaleAsIfParented(originalToClone.original, transitioningObject.GetPortalIn().transform, transitioningObject.GetPortalOut().transform);
                    // position
                    var objectToPortal = portalTransform.InverseTransformDirection(originalToClone.original.position - portalTransform.position) ;
                    var localPos = new Vector3(-objectToPortal.x* (1/portalInGlobalScale.x), objectToPortal.y* (1/portalInGlobalScale.y), -objectToPortal.z* (1/portalInGlobalScale.z));
                    originalToClone.clone.position = transitioningObject.GetPortalOut().transform.TransformPoint(localPos);
                
                    //rotation
                    var rotation = Quaternion.LookRotation(-portalTransform.forward, portalTransform.up);
                    var relativeRot = Quaternion.Inverse(rotation) * originalToClone.original.rotation;
                    originalToClone.clone.rotation =portal.GetLinkedOutPortal().transform.rotation * relativeRot;
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
        public void SetLossyScale(Transform target, Vector3 targetLossyScale)
        {
            if (target.parent == null)
            {
                // If no parent, local scale equals the desired lossy scale.
                target.localScale = targetLossyScale;
            }
            else
            {
                // Calculate the parent lossy scale.
                Vector3 parentLossyScale = target.parent.lossyScale;
            
                // Adjust local scale to achieve the desired lossy scale.
                target.localScale = new Vector3(
                    targetLossyScale.x / parentLossyScale.x,
                    targetLossyScale.y / parentLossyScale.y,
                    targetLossyScale.z / parentLossyScale.z
                );
            }
        }

        private void StopCropMaterial(TransitioningObject transitioningObject)
        {
            CropMaterial(transitioningObject);
        }

        private void CropMaterial(TransitioningObject transitioningObject, Vector4 portalOutPos = default, Vector4 portalOutForward = default, Vector4 portalInPos = default, Vector4 portalInForward = default )
        {
            for (int i = 0; i < transitioningObject.GetCloneMaterials().Count; i++)
            {
                transitioningObject.GetCloneMaterials()[i].SetVector (PortalCenter, portalOutPos );
                transitioningObject.GetCloneMaterials()[i].SetVector (PortalNormal, portalOutForward);
                
                transitioningObject.GetOriginalMaterials()[i].SetVector (PortalCenter, portalInPos);
                transitioningObject.GetOriginalMaterials()[i].SetVector (PortalNormal, portalInForward);
            }
        }
    
    
    
    
    }
}

