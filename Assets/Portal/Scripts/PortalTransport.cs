#nullable enable
using System;
using System.Collections.Generic;
using Scripts;
using UnityEngine;
using Utils;


[RequireComponent(typeof(Collider))]
public class PortalTransport : MonoBehaviour
{
    
    
    private Transform? _portalOut;
    private bool _notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private Transform portalIn;
    private readonly List<TransitioningObject> _objectsOnPortal = new List<TransitioningObject>();
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private GameObject emptyClone;



    private void Start()
    {
        if (portalIn == null)
            throw new Exception("Portal In must be initialized in editor");
    }

    public void SetPortalOut(Transform portalOut)
    {
        _portalOut = portalOut;
    }
    private void OnTriggerEnter(Collider other)
    {
        print("onTriggerEnter " + portalIn.name);
        if(_portalOut == null)
        {
            print("no linked out portal");
            return;
        }
        var objectCrossing = other.gameObject;
        if (IsClone(objectCrossing))
        {
            print(objectCrossing.name + " is a clone");
            return;
        }

        var rigidbody = objectCrossing.GetComponent<Rigidbody>();
        if (rigidbody== null)
            return;
        if(!rigidbody.worldCenterOfMass.IsInFrontOfWithError(portalIn, 0.2f))
        {
            print( objectCrossing.name + " not in front of " + portalIn.name);
            return;
        }
        
        CreateClone(objectCrossing);
    }

    private void TriggerOnPortalEnter(TransitioningObject objectCrossing)
    {
        if(objectCrossing.GetImplementsIPortal())
            objectCrossing.GetOriginal().SendMessage("OnPortalEnter", portalIn.gameObject.GetComponent<Portal>());
    }
    private bool IsClone(GameObject go)
    {
        return _objectsOnPortal.FindIndex(item => item.GetClone().gameObject.Equals(go) ) != -1;
    }    

    public void CreateClone(GameObject objectCrossing)
    {
        if(_portalOut==null)
            return;
        var customClone = objectCrossing.GetComponent<ICustomClone>();
        
        var cloneMode = customClone == null ? PortalUtils.CloneMode.AUTOMATIC : customClone.GetMode();
        
        GameObject clone;
        var originalToClone = new List<KeyValuePair<Transform, Transform>>();
        switch (cloneMode)
        {
            case PortalUtils.CloneMode.CUSTOM:
                clone = customClone.CreateClone(objectCrossing, portalIn, _portalOut);
                break;
            default:
                clone = CreateGameObjectTree(objectCrossing, _portalOut, originalToClone, true);
                break;
        }
        
        ForwardEvents(clone, objectCrossing);
        // todo
        // IgnoreCollision( objectCrossing);
        var iPortal = objectCrossing.GetComponent<IPortal>();
        var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portalIn,_portalOut, originalToClone, iPortal!=null );
        _objectsOnPortal.Add(objectOnPortal);
        TriggerOnPortalEnter(objectOnPortal);
        
    }

    private void IgnoreCollision(GameObject objectCrossing)
    {
        var collisionHandlerIn = objectCrossing.AddComponent<CollisionHandler>();
        collisionHandlerIn.SetPortal(portalIn);
    }

    private GameObject CreateGameObjectTree(GameObject objectCrossing, Transform parent,List<KeyValuePair<Transform, Transform>> originalToClone,  bool firstIteration)
    {
        var objectToPortal = portalIn.InverseTransformDirection(objectCrossing.transform.position - portalIn.gameObject.transform.position);
        var localPosition = new Vector3(-objectToPortal.x, objectToPortal.y, -objectToPortal.z);
        var clone = Instantiate(emptyClone, _portalOut.position + localPosition, objectCrossing.transform.localRotation, parent);
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
            print("Skinned mesh renderer");
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
        print(newRenderer.bones.Length);
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
        cloneCollider.enabled = ((BoxCollider)originalCollider).enabled;
        cloneCollider.material = ((BoxCollider)originalCollider).material;
        cloneCollider.isTrigger = ((BoxCollider)originalCollider).isTrigger;
        cloneCollider.sharedMaterial = ((BoxCollider)originalCollider).sharedMaterial;
        
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

        if (leavingPortal.EnteredPortal())
        {
            leavingPortal.Transport();
            TriggerOnPortalExit(leavingPortal);
        }
        Destroy(leavingPortal.GetClone().gameObject);  
        Destroy(leavingPortal.GetOriginal().GetComponent<EventListener>());
        Destroy(leavingPortal.GetOriginal().GetComponent<CollisionHandler>());
    }
    
    
    private void OnTriggerExit(Collider other)
    {
        
        if(portalIn == null || _portalOut == null)
            return;
        TransitioningObject? leavingPortal = GetObjectOnPortalLeaving(other.gameObject);
        if (leavingPortal == null ||  leavingPortal.GetClone()==null)
            return;
        _objectsOnPortal.Remove(leavingPortal);

        ExitPortal(leavingPortal);

    }
    
    private void TriggerOnPortalExit(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalExit", portalIn.gameObject.GetComponent<Portal>());
    }
    
    private void TriggerOnPortalTransitioning(TransitioningObject leavingPortal)
    {
        // todo send message o directamente llamar la funcion... como es un interface
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalTransitioning", portalIn.gameObject.GetComponent<Portal>());
        
    }

    private TransitioningObject? GetObjectOnPortalLeaving(GameObject o)
    {
        var objectOnPortalLeavingIndex = _objectsOnPortal.FindIndex(item => item.GetOriginal().gameObject.Equals(o) );
        return objectOnPortalLeavingIndex == -1 ? null : _objectsOnPortal[objectOnPortalLeavingIndex];
    }

    
    private void Update()
    {
        if(_portalOut == null)
            return;
        for (int j = 0; j < _objectsOnPortal.Count ; j++)
        {
            var t = _objectsOnPortal[j];

            if (!t.GetOriginalRigidbody().worldCenterOfMass.IsInFrontOf(portalIn))
            {
                // todo intercambiar de lugar con clon
                _objectsOnPortal.Remove(t);
                ExitPortal(t);
                
                return;
                
            } 
            if (t.GetMainCamera() != null)
            {
                if (!t.GetOriginal().GetMainCamera().transform.IsInFrontOf(portalIn))
                {
                    _objectsOnPortal.Remove(t);
                    ExitPortal(t);
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
        }
    }

    private void SetPosition(TransitioningObject transitioningObject)
    {
        
        foreach (var keyValuePair in transitioningObject.GetOriginalToCloneList())
        {
            if (keyValuePair.Value.parent == transitioningObject.GetPortalOut())
            {
                keyValuePair.Value.localScale = keyValuePair.Key.localScale;
               
                var objectToPortal = portalIn.InverseTransformDirection(keyValuePair.Key.position - portalIn.gameObject.transform.position);
                var localPos = new Vector3(-objectToPortal.x, objectToPortal.y, -objectToPortal.z);
                keyValuePair.Value.position = _portalOut.TransformPoint(localPos);
                var rotation = Quaternion.LookRotation(-portalIn.forward, portalIn.up);
                var relativeRot = Quaternion.Inverse(rotation) * keyValuePair.Key.rotation;
                keyValuePair.Value.rotation =_portalOut.rotation * relativeRot;
            }
            else
            {
                keyValuePair.Value.localScale = keyValuePair.Key.localScale;
                keyValuePair.Value.localRotation = keyValuePair.Key.localRotation;
                keyValuePair.Value.localPosition = keyValuePair.Key.localPosition;
            }
                
            
        }
        
    }
}

