#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Scripts;
using UnityEngine;
using Utils;


[RequireComponent(typeof(Collider))]
public class PortalTransport : MonoBehaviour
{
    
    
    private Portal? _portalOut;
    [HideInInspector]
    [SerializeField] private Portal portalIn;
    private readonly List<TransitioningObject> _objectsOnPortal = new List<TransitioningObject>();
    [HideInInspector]
    [SerializeField] private GameObject emptyClone;



    private void Start()
    {
        if (portalIn == null)
            throw new Exception("Portal In must be initialized in editor");
    }

    public void SetPortalOut(Portal portalOut)
    {
        _portalOut = portalOut;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(_portalOut == null)
        {
            return;
        }
        var objectCrossing = other.gameObject;
        if (IsTransitioningObject(objectCrossing))
        {
            return;
        }
        
        var rigidbody = objectCrossing.GetComponent<Rigidbody>();
        if (rigidbody== null)
            return;
        if(!rigidbody.worldCenterOfMass.IsInFrontOf(portalIn.transform))
        {
            return;
        }
        print("entering portal: "+ gameObject.transform);
        CreateClone(objectCrossing);
    }

    
    private bool IsTransitioningObject(GameObject go)
    {
        return _objectsOnPortal.FindIndex(item => item.GetClone().gameObject.Equals(go) || item.GetOriginal().gameObject.Equals(go) ) != -1;
    }    

    public void CreateClone(GameObject objectCrossing)
    {
        if(_portalOut==null)
            return;
        var customClone = objectCrossing.GetComponent<ICustomClone>();
        
        var cloneMode = customClone == null ? PortalUtils.CloneMode.AUTOMATIC : customClone.GetMode();
        
        GameObject clone;
        var originalToClone = new List<KeyValuePair<Transform, Transform>>();
        var originalMaterials = new List<Material>();
        var cloneMaterials = new List<Material>();
        switch (cloneMode)
        {
            case PortalUtils.CloneMode.CUSTOM:
                clone = customClone.CreateClone(objectCrossing, portalIn.transform, _portalOut.transform);
                break;
            default:
                clone = CreateGameObjectTree(objectCrossing, _portalOut.transform, originalToClone, originalMaterials, cloneMaterials, true);
                break;
        }
        
        ForwardEvents(clone, objectCrossing);
        // todo
        // IgnoreCollision( objectCrossing);
        var iPortal = objectCrossing.GetComponent<IPortal>();
        
        IEnumerable<(Transform original, Transform clone)> oc =
            from kvp in originalToClone
            select (kvp.Key, kvp.Value);
        
        var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portalIn,_portalOut, oc, iPortal!=null );
        _objectsOnPortal.Add(objectOnPortal);
        TriggerOnPortalEnter(objectOnPortal);
        
    }

    public void AddTransitioningObject(TransitioningObject transitioningObject)
    {
        _objectsOnPortal.Add(transitioningObject);
    }

    private void IgnoreCollision(GameObject objectCrossing)
    {
        var collisionHandlerIn = objectCrossing.AddComponent<CollisionHandler>();
        collisionHandlerIn.SetPortal(portalIn.transform);
    }

    private GameObject CreateGameObjectTree(GameObject objectCrossing, Transform parent,List<KeyValuePair<Transform, Transform>> originalToClone, List<Material> originalMaterials, List<Material> cloneMaterial,  bool firstIteration)
    {
        var objectToPortal = portalIn.transform.InverseTransformDirection(objectCrossing.transform.position - portalIn.transform.position);
        var localPosition = new Vector3(-objectToPortal.x, objectToPortal.y, -objectToPortal.z);
        var clone = Instantiate(emptyClone, _portalOut.transform.position + localPosition, objectCrossing.transform.localRotation, parent);
        originalToClone.Add( new KeyValuePair<Transform, Transform>(objectCrossing.transform, clone.transform));
        clone.name = objectCrossing.name + ("(Portal)");
        DuplicateMesh(objectCrossing, clone, originalToClone,originalMaterials, cloneMaterial, firstIteration);
        for (int i = 0; i < objectCrossing.transform.childCount; i++)
        {
            CreateGameObjectTree(objectCrossing.transform.GetChild(i).gameObject, clone.transform, originalToClone,originalMaterials, cloneMaterial, false);
        }
        return clone;
    }
    private static void DuplicateMesh(GameObject original, GameObject clone,List<KeyValuePair<Transform, Transform>> originalToClone, List<Material> originalMaterials, List<Material> cloneMaterial,  bool firstIteration)
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
        if(portalIn == null || _portalOut == null)
            return;
        TransitioningObject? leavingPortal = GetObjectOnPortalLeaving(other.gameObject);
        if (leavingPortal == null ||  leavingPortal.GetClone()==null)
            return;
        // print("exiting "+portalIn.name );
        _objectsOnPortal.Remove(leavingPortal);

        ExitPortal(leavingPortal);

    }
    
    private void TriggerOnPortalEnter(TransitioningObject objectCrossing)
    {
        if(objectCrossing.GetImplementsIPortal())
            objectCrossing.GetOriginal().SendMessage("OnPortalEnter", objectCrossing._transitioningPortalObject);
    }
    private void TriggerOnPortalExit(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalExit", leavingPortal._transitioningPortalObject);
    }
    private void TriggerOnPortalTransitioning(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalTransitioning", leavingPortal._transitioningPortalObject);
        
    }
    private void TriggerOnPortalCrossed(TransitioningObject crossingPortal)
    {
        if(crossingPortal.GetImplementsIPortal())
            crossingPortal.GetOriginal().SendMessage("OnPortalCrossed", crossingPortal._transitioningPortalObject);
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

            if (!t.GetOriginalRigidbody().worldCenterOfMass.IsInFrontOfWithError(portalIn.transform, 0.5f))
            {
                // print("changing from "+portalIn.name +" to "+_portalOut.name);
                t.GetPortalOut().AddTransitioningObject(t);
                t.SwitchPortals( _portalOut,portalIn);
                _objectsOnPortal.Remove(t);
                TriggerOnPortalCrossed(t);
                return;
                
            } 
            if (t.GetMainCamera() != null)
            {
                if (!t.GetOriginal().GetMainCamera().transform.IsInFrontOf(portalIn.transform))
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
            CropMaterial(transitioningObject);   
        }
    }

    private void SetPosition(TransitioningObject transitioningObject)
    {
        // print("portal that has transitioningObject: "+portalIn.name);
        foreach (var originalToClone in transitioningObject.GetOriginalToCloneList())
        {
            if (originalToClone.clone.parent == transitioningObject.GetPortalOut().transform)
            {
                print("hi");
                //scale
                originalToClone.clone.localScale = originalToClone.original.localScale;
               
                // position
                var objectToPortal = portalIn.transform.InverseTransformDirection(originalToClone.original.position - portalIn.transform.position) ;
                var localPos = new Vector3(-objectToPortal.x* (1/portalIn.transform.localScale.x), objectToPortal.y* (1/portalIn.transform.localScale.y), -objectToPortal.z* (1/portalIn.transform.localScale.z));
                originalToClone.clone.localPosition =localPos;
                
                //rotation
                var rotation = Quaternion.LookRotation(-portalIn.transform.forward, portalIn.transform.up);
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

    private void CropMaterial(TransitioningObject transitioningObject)
    {
        for (int i = 0; i < transitioningObject.GetCloneMaterials().Count; i++)
        {
            transitioningObject.GetCloneMaterials()[i].SetVector ("_portalCenter", _portalOut.transform.position);
            transitioningObject.GetCloneMaterials()[i].SetVector ("_portalNormal", _portalOut.transform.forward);
            
            transitioningObject.GetOriginalMaterials()[i].SetVector ("_portalCenter", portalIn.transform.position);
            transitioningObject.GetOriginalMaterials()[i].SetVector ("_portalNormal", portalIn.transform.forward);
        }
    }
}

