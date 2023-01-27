#nullable enable
using System;
using System.Collections.Generic;
using Scripts;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utils;


[RequireComponent(typeof(Collider))]
public class PortalTransport : MonoBehaviour
{
    
    
    private Transform? _portalOut;
    private bool _notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private Transform portalIn;
    private readonly List<TransitioningObject> _objectsOnPortal  = new List<TransitioningObject>();
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
        if(_portalOut == null)
            return;
        var objectCrossing = other.gameObject;
        if (IsClone(objectCrossing))
            return;
        if(!objectCrossing.transform.IsInFrontOf(portalIn))
            return;
        if (objectCrossing.GetComponent<Rigidbody>() == null)
            return;
        CreateClone(objectCrossing);
    }

    private void TriggerOnPortalEnter(TransitioningObject objectCrossing)
    {
        if(objectCrossing.GetImplementsIPortal())
            objectCrossing.GetOriginal().SendMessage("OnPortalEnter", portalIn.gameObject.GetComponent<Portal>());
        //todo onPortalEnter general
    }
    private bool IsClone(GameObject go)
    {
        return _objectsOnPortal.FindIndex(item => item.GetClone().gameObject.Equals(go) ) != -1;
    }    

    private void CreateClone(GameObject objectCrossing)
    {
        if(_portalOut==null)
            return;
        var customClone = objectCrossing.GetComponent<ICustomClone>();
        
        var cloneMode = customClone == null ? PortalUtils.CloneMode.AUTOMATIC : customClone.GetMode();
        
        GameObject clone;
        switch (cloneMode)
        {
            case PortalUtils.CloneMode.CUSTOM:
                clone = customClone.GetClone();
                break;
            default:
                clone = CreateGameObjectTree(objectCrossing, _portalOut);
                break;
        }
        
        ForwardEvents(clone, objectCrossing);
        var iPortal = objectCrossing.GetComponent<IPortal>();
        var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portalIn,_portalOut, iPortal!=null );
        _objectsOnPortal.Add(objectOnPortal);
        TriggerOnPortalEnter(objectOnPortal);
        
    }

    private GameObject CreateGameObjectTree(GameObject objectCrossing, Transform parent)
    {
        var clone = Instantiate(emptyClone, _portalOut.position, objectCrossing.transform.rotation, parent);
        clone.name = objectCrossing.name + ("(Portal)");
        DuplicateMesh(objectCrossing, clone);
        for (int i = 0; i < objectCrossing.transform.childCount; i++)
        {
            CreateGameObjectTree(objectCrossing.transform.GetChild(i).gameObject, clone.transform);
        }
        return clone;
    }
    private static void DuplicateMesh(GameObject original, GameObject clone)
    {
        //todo
        // vertices, triangles,  uv, uv2, normal, tangent, colors
        CopyTransform(original.transform, clone.transform);
        CopyMesh(original, clone);
        CopyCollider(original, clone);
    }

    private static void CopyMesh(GameObject original, GameObject clone)
    {
        var originalMesh = original.GetComponent<MeshRenderer>();
        if(originalMesh == null)
            return;
        var cloneMesh = clone.AddComponent<MeshRenderer>();
        cloneMesh.sharedMaterials = originalMesh.sharedMaterials;
        var originalMeshFilter = original.GetComponent<MeshFilter>();
        if(originalMeshFilter == null)
            return;
        var cloneMeshFilter = clone.AddComponent<MeshFilter>();
        cloneMeshFilter.sharedMesh = Instantiate(originalMeshFilter.sharedMesh);
    }

    private static void CopyTransform(Transform original, Transform clone)
    {
        clone.localScale = original.localScale;
        clone.position = original.position;
    }

    private static void CopyCollider(GameObject original, GameObject clone)
    {
        Collider originalCollider = original.GetComponent<Collider>();
        
        if(originalCollider == null)
            return;
        var cloneCollider =(Collider) clone.AddComponent(originalCollider.GetType());
        cloneCollider.material = originalCollider.material;
        cloneCollider.contactOffset = originalCollider.contactOffset;
        // CopyComponent(original.GetComponent<Collider>(), clone);
    }

    private void ForwardEvents(GameObject clone, GameObject objectCrossing)
    {
        var eventForwarder = clone.AddComponent<EventForwarder>();
        var eventListener = objectCrossing.AddComponent<EventListener>();
        eventListener.SetEventForwarder(eventForwarder);
    }
    

    private void OnTriggerExit(Collider other)
    {
        if(portalIn == null || _portalOut == null)
            return;
        TransitioningObject? leavingPortal = GetObjectOnPortalLeaving(other.gameObject);
        if (leavingPortal == null)
            return;
        _objectsOnPortal.Remove(leavingPortal);
        
        if (leavingPortal.EnteredPortal())
        {
            leavingPortal.Transport();
            TriggerOnPortalExit(leavingPortal);
        }
            
        Destroy(leavingPortal.GetClone().gameObject);
        Destroy(leavingPortal.GetOriginal().GetComponent<EventListener>());
        
    }
    
    private void TriggerOnPortalExit(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalExit", portalIn.gameObject.GetComponent<Portal>());
        //todo onPortalExit general
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
        foreach (var t in _objectsOnPortal)
        {
            
            // var center = t.GetOriginal().position + t.GetOriginalRigidbody().centerOfMass;
            // Debug.DrawRay(center, Vector3.up*10, Color.magenta);
            // if (!center.IsInFrontOf(portalIn))
            // {
            //         
            //     t.Transport();
            //     TriggerOnPortalExit(t);
            // }   
            // else 
            if (t.GetMainCamera() != null)
            {
                if (!t.GetOriginal().GetMainCamera().transform.IsInFrontOf(portalIn))
                {
                    t.Transport();
                    TriggerOnPortalExit(t);
                }
            }
            else
            {
                ReplicateTransform(t);
                TriggerOnPortalTransitioning(t);
            }
        }
    }
    
    private void ReplicateTransform(TransitioningObject transitioningObject)
    {
        SetPosition(transitioningObject);
        SetAngle(transitioningObject);
    }

    private void SetPosition(TransitioningObject transitioningObject)
    {
        var objectToPortal = portalIn.InverseTransformDirection(transitioningObject.GetOriginal().position - portalIn.gameObject.transform.position);
        transitioningObject.GetClone().localPosition = new Vector3(-objectToPortal.x, objectToPortal.y, -objectToPortal.z);
    }

    private void SetAngle(TransitioningObject transitioningObject)
    {
        if (_portalOut == null) 
            return;
        Quaternion rotation = Quaternion.LookRotation(-portalIn.forward, portalIn.up);
        Quaternion relativeRot = Quaternion.Inverse(rotation) * transitioningObject.GetOriginal().rotation;
        transitioningObject.GetClone().rotation = _portalOut.rotation * relativeRot;
    }


    private static Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields(); 
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
}

