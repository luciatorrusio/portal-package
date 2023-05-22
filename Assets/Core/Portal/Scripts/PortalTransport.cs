#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Portal.Scripts;
using Scripts;
using UnityEngine;
using Utils;


[RequireComponent(typeof(Collider))]
public class PortalTransport : MonoBehaviour
{
    
    
    [HideInInspector]
    [SerializeField] private Portal portal;
    private readonly List<TransitioningObject> _objectsOnPortal = new List<TransitioningObject>();
    [HideInInspector]
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
        var originalToClone = new List<KeyValuePair<Transform, Transform>>();
        switch (cloneMode)
        {
            case PortalUtils.CloneMode.CUSTOM:
                clone = customClone.CreateClone(objectCrossing, portal.transform, portal.GetLinkedOutPortal().transform);
                break;
            default:
                clone = CreateGameObjectTree(objectCrossing, portal.GetLinkedOutPortal().transform, originalToClone, true);
                break;
        }
        
        ForwardEvents(clone, objectCrossing);
        // todo
        // IgnoreCollision( objectCrossing);
        var iPortal = objectCrossing.GetComponent<TransitionListener>();
        
        IEnumerable<(Transform original, Transform clone)> oc =
            from kvp in originalToClone
            select (kvp.Key, kvp.Value);
        
        var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portal,portal.GetLinkedOutPortal(), oc, iPortal!=null );
        _objectsOnPortal.Add(objectOnPortal);
        TriggerOnPortalEnter(objectOnPortal);
        
    }

    public void AddTransitioningObject(TransitioningObject transitioningObject)
    {
        _objectsOnPortal.Add(transitioningObject);
    }

    //todo
    private void IgnoreCollision(GameObject objectCrossing)
    {
        var collisionHandlerIn = objectCrossing.AddComponent<CollisionHandler>();
        collisionHandlerIn.SetPortal(portal.transform);
    }

    private GameObject CreateGameObjectTree(GameObject objectCrossing, Transform parent,List<KeyValuePair<Transform, Transform>> originalToClone,  bool firstIteration)
    {
        var objectToPortal = portal.transform.InverseTransformDirection(objectCrossing.transform.position - portal.transform.position);
        var localPosition = new Vector3(-objectToPortal.x, objectToPortal.y, -objectToPortal.z);
        var clone = Instantiate(emptyClone, portal.GetLinkedOutPortal().transform.position + localPosition, objectCrossing.transform.localRotation, parent);
        originalToClone.Add( new KeyValuePair<Transform, Transform>(objectCrossing.transform, clone.transform));
        clone.name = objectCrossing.name + ("(Portal)");
        DuplicateMesh(objectCrossing, clone, originalToClone, firstIteration);
        for (int i = 0; i < objectCrossing.transform.childCount; i++)
        {
            CreateGameObjectTree(objectCrossing.transform.GetChild(i).gameObject, clone.transform, originalToClone, false);
        }
        return clone;
    }
    private static void DuplicateMesh(GameObject original, GameObject clone,List<KeyValuePair<Transform, Transform>> originalToClone,  bool firstIteration)
    {
        CopyTransform(original.transform, clone.transform, firstIteration);
        CopyMesh(original, clone, originalToClone);
        CopyCollider(original, clone);
    }

    private static void CopyMesh(GameObject original, GameObject clone, List<KeyValuePair<Transform, Transform>> originalToClone)
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
    private static void CopySkinnedMeshRenderer(SkinnedMeshRenderer originalRenderer, GameObject newObject, List<KeyValuePair<Transform, Transform>> originalToClone)
    {
        
        var newRenderer = newObject.AddComponent<SkinnedMeshRenderer>();
        newRenderer.forceMatrixRecalculationPerRender = true;
        newRenderer.sharedMesh = originalRenderer.sharedMesh;
        newRenderer.materials = originalRenderer.materials;
        
        var bones = new List<Transform>();
        foreach (var bone in originalRenderer.bones)
        {
            foreach (var keyValuePair in originalToClone)
            {
                if (keyValuePair.Key == bone)
                {
                    bones.Add(keyValuePair.Value);
                }
            }
        }
        

        newRenderer.bones = bones.ToArray();
        newRenderer.rootBone = originalToClone.Find(x => x.Key == originalRenderer.rootBone ).Value;
        newRenderer.quality = originalRenderer.quality;
        newRenderer.updateWhenOffscreen = originalRenderer.updateWhenOffscreen;
        newRenderer.localBounds = originalRenderer.localBounds;
    }

    private static void CopyRenderer(Renderer originalMesh, GameObject clone, GameObject original)
    {
        var cloneMesh = clone.AddComponent<MeshRenderer>();
        cloneMesh.sharedMaterials = originalMesh.sharedMaterials;
        
        var originalMeshFilter = original.GetComponent<MeshFilter>();
        if(originalMeshFilter == null)
            return;
        var cloneMeshFilter = clone.AddComponent<MeshFilter>();
        cloneMeshFilter.sharedMesh = Instantiate(originalMeshFilter.sharedMesh);
    }

    private static void CopyTransform(Transform original, Transform clone, bool firstIteration)
    {
        clone.localScale = original.localScale;
        if(firstIteration)
            return;
        clone.position = original.position;
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
        var eventListener = objectCrossing.AddComponent<EventListener>();
        eventListener.SetEventForwarder(eventForwarder);
    }

    private void ExitPortal(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetClone() == null)
            return;
        StopCropMaterial(leavingPortal);
        TriggerOnPortalExit(leavingPortal);
        Destroy(leavingPortal.GetClone().gameObject);  
        DestroyAddedComponents(leavingPortal.GetOriginal());
    }

    private void DestroyAddedComponents(Transform original)
    {
        Destroy(original.GetComponent<EventListener>());
        Destroy(original.GetComponent<CollisionHandler>());
    }
    
    
    private void OnTriggerExit(Collider other)
    {
        if(!enabled || portal == null || portal.GetLinkedOutPortal() == null)
            return;
        TransitioningObject? leavingPortal = GetObjectOnPortalLeaving(other.gameObject);
        if (leavingPortal == null ||  leavingPortal.GetClone()==null)
            return;
        _objectsOnPortal.Remove(leavingPortal);

        ExitPortal(leavingPortal);

    }
    # region TRIGGERS
    private void TriggerOnPortalEnter(TransitioningObject objectCrossing)
    {
        if(objectCrossing.GetImplementsIPortal())
            objectCrossing.GetOriginal().SendMessage("OnPortalEnter", objectCrossing.Transition);
    }
    private void TriggerOnPortalExit(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalExit", leavingPortal.Transition);
    }
    private void TriggerOnPortalTransitioning(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalTransitioning", leavingPortal.Transition);
        
    }
    private void TriggerOnPortalCrossed(TransitioningObject crossingPortal)
    {
        if(crossingPortal.GetImplementsIPortal())
            crossingPortal.GetOriginal().SendMessage("OnPortalCrossed", crossingPortal.Transition);
    }
    
    # endregion
    private TransitioningObject? GetObjectOnPortalLeaving(GameObject o)
    {
        var objectOnPortalLeavingIndex = _objectsOnPortal.FindIndex(item => item.GetOriginal().gameObject.Equals(o) );
        return objectOnPortalLeavingIndex == -1 ? null : _objectsOnPortal[objectOnPortalLeavingIndex];
    }

    
    private void Update()
    {
        if(portal.GetLinkedOutPortal() == null)
            return;
        for (var j = 0; j < _objectsOnPortal.Count ; j++)
        {
            var t = _objectsOnPortal[j];
            if (!t.GetOriginalRigidbody().worldCenterOfMass.IsInFrontOfWithError(portal.transform, 0.1f))
            {
                t.GetPortalOut().AddTransitioningObject(t);
                t.SwitchPortals( portal.GetLinkedOutPortal(),portal.GetLinkedOutPortal().GetLinkedOutPortal());
                _objectsOnPortal.Remove(t);
                TriggerOnPortalCrossed(t);
                print("trigger from 1");
                return;
                
            } 
            if (t.GetMainCamera() != null)
            {
                if (!t.GetMainCamera().transform.IsInFrontOfWithError(portal.transform, 0.1f))
                {
                    t.GetPortalOut().AddTransitioningObject(t);
                    t.SwitchPortals( portal.GetLinkedOutPortal(),portal.GetLinkedOutPortal().GetLinkedOutPortal());
                    _objectsOnPortal.Remove(t);
                    TriggerOnPortalCrossed(t);
                    print("trigger from 2");
                    return;
                }
            }
             
            ReplicateTransform(t);
            TriggerOnPortalTransitioning(t);
        }
        
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
        var scale = portalTransform.localScale;
        foreach (var originalToClone in transitioningObject.GetOriginalToCloneList())
        {
            if (originalToClone.clone.parent == transitioningObject.GetPortalOut().transform)
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

    private void StopCropMaterial(TransitioningObject transitioningObject)
    {
        CropMaterial(transitioningObject);
    }

    private void CropMaterial(TransitioningObject transitioningObject, Vector4 portalOutPos = default, Vector4 portalOutForward = default, Vector4 portalInPos = default, Vector4 portalInForward = default )
    {
        for (int i = 0; i < transitioningObject.GetCloneMaterials().Count; i++)
        {
            transitioningObject.GetCloneMaterials()[i].SetVector (PortalCenter, portalOutPos);
            transitioningObject.GetCloneMaterials()[i].SetVector (PortalNormal, portalOutForward);
            
            transitioningObject.GetOriginalMaterials()[i].SetVector (PortalCenter, portalInPos);
            transitioningObject.GetOriginalMaterials()[i].SetVector (PortalNormal, portalInForward);
        }
    }
    
    
    
    
}

