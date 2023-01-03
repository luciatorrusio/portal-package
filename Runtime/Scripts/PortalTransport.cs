#nullable enable
using System.Collections.Generic;
using Scripts;
using UnityEngine;
using UnityEngine.PlayerLoop;
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
        if (IsClone(objectCrossing))
            return;
        if(!objectCrossing.transform.IsInFrontOf(portalIn))
            return;
        if (objectCrossing.GetComponent<Rigidbody>() == null)
            return;
        // todo se esta haciendo un clon del clon, hay que checkear
        CreateClone(objectCrossing);
    }

    private void TriggerOnPortalEnter(TransitioningObject objectCrossing)
    {
        if(objectCrossing.GetImplementsIPortal())
            objectCrossing.GetOriginal().SendMessage("OnPortalEnter", portalIn.gameObject);
    }
    private bool IsClone(GameObject go)
    {
        return _objectsOnPortal.FindIndex(item => item.GetClone().gameObject.Equals(go) ) != -1;
    }    

    private void CreateClone(GameObject objectCrossing)
    {
        var clone = Instantiate(objectCrossing, portalOut.position, objectCrossing.transform.rotation, portalOut);
        //todo create object tree
        ForwardEvents(clone, objectCrossing);
        var iPortal = objectCrossing.GetComponent<IPortal>();
        var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portalIn,portalOut, iPortal!=null );
        _objectsOnPortal.Add(objectOnPortal);
        TriggerOnPortalEnter(objectOnPortal);
        
    }

    private void ForwardEvents(GameObject clone, GameObject objectCrossing)
    {
        var eventForwarder = clone.AddComponent<EventForwarder>();
        var eventListener = objectCrossing.AddComponent<EventListener>();
        eventListener.SetEventForwarder(eventForwarder);
    }
    

    private void OnTriggerExit(Collider other)
    {
        if(portalIn == null || portalOut == null)
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
        
        
    }
    
    private void TriggerOnPortalExit(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalExit", portalIn.gameObject);
    }
    
    private void TriggerOnPortalTransitioning(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalTransitioning", portalIn.gameObject);
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
            TriggerOnPortalTransitioning(t);
            if (t.GetMainCamera() != null)
            {
                if (!t.GetOriginal().GetMainCamera().transform.IsInFrontOf(portalIn))
                {
                    t.Transport();
                    TriggerOnPortalExit(t);
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
        transitioningObject.GetClone().localPosition = new Vector3(-objectToPortal.x, objectToPortal.y, -objectToPortal.z);
    }

    private void SetAngle(TransitioningObject transitioningObject)
    {
        Quaternion rotation = Quaternion.LookRotation(-portalIn.forward, portalIn.up);
        Quaternion relativeRot = Quaternion.Inverse(rotation) * transitioningObject.GetOriginal().rotation;
        transitioningObject.GetClone().rotation = portalOut.rotation * relativeRot;
    }
    
    public Component CopyComponent(Component original, GameObject destination)
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

