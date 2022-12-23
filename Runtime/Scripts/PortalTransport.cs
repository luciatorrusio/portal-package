#nullable enable
using System.Collections.Generic;
using PortalExtensionMethods;
using UnityEngine;
using Utils;


[RequireComponent(typeof(Collider))]
public class PortalTransport : MonoBehaviour
{
    private Transform? portalOut;
    [SerializeField] private Transform? portalIn;
    private List<TransitioningObject> _objectsOnPortal;
    [SerializeField] private GameObject emptyClone;

    private void Start()
    {
        _objectsOnPortal = new List<TransitioningObject>();
    }

    public void SetPortalOut(Transform portalOut)
    {
        this.portalOut = portalOut;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(portalOut == null)
            return;
        var objectCrossing = other.gameObject;
        if (objectCrossing.layer == LayerMask.NameToLayer("transitioningObject"))
            return;
        if(!objectCrossing.transform.IsInFrontOf(portalIn))
            return;
        if (objectCrossing.GetComponent<Rigidbody>() == null)
            return;
        
        var clone = Instantiate(objectCrossing, portalOut.position, objectCrossing.transform.rotation, portalOut);
        // var clone = Instantiate(emptyClone, portalOut.position, objectCrossing.transform.rotation, portalOut);
        
        // clone.AddComponent<MeshRenderer>().sharedMaterials = objectCrossing.GetComponent<MeshFilter>().GetComponent<MeshRenderer>().sharedMaterials;
        // clone.AddComponent<MeshFilter>().sharedMesh = Instantiate(objectCrossing.GetComponent<MeshFilter>().sharedMesh);
        
        clone.layer = LayerMask.NameToLayer("transitioningObject");
        // EventForwarder eventForwarder = clone.AddComponent<EventForwarder>();
        
        // var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portalIn, eventForwarder);
        var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portalIn);
        
        _objectsOnPortal.Add(objectOnPortal);

    }
    

    private void OnTriggerExit(Collider other)
    {
        if(portalIn == null || portalOut == null)
            return;
        TransitioningObject? leavingPortal = GetObjectOnPortalLeaving(other.gameObject);
        print("name of on trigger exit: "+other.name);
        if (leavingPortal == null)
            return;
        _objectsOnPortal.Remove(leavingPortal);
        if(leavingPortal.EnteredPortal())
            leavingPortal.Transport();
        Destroy(leavingPortal.GetClone().gameObject);
        
        
    }

    private TransitioningObject? GetObjectOnPortalLeaving(GameObject o)
    {
        var objectOnPortalLeavingIndex = _objectsOnPortal.FindIndex(item => item.GetOriginal().gameObject.Equals(o) );
        return objectOnPortalLeavingIndex == -1 ? null : _objectsOnPortal[objectOnPortalLeavingIndex];
    }

    private void Update()
    {
        foreach (var t in _objectsOnPortal)
        {
            ReplicateTransform(t);
            if (t.GetMainCamera() != null)
            {
                if (!t.GetOriginal().GetMainCamera().transform.IsInFrontOf(portalIn))
                {
                    t.Transport();
                }
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
        transitioningObject.GetClone().localPosition = objectToPortal;
    }

    private void SetAngle(TransitioningObject transitioningObject)
    {
        Quaternion relativeRot = Quaternion.Inverse(portalIn.rotation) * transitioningObject.GetOriginal().rotation;
        transitioningObject.GetClone().rotation = portalOut.rotation * relativeRot;
    }
    

    
    
    
}

