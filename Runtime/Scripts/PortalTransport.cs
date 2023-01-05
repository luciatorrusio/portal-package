#nullable enable
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scripts;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utils;


[RequireComponent(typeof(Collider))]
public class PortalTransport : MonoBehaviour
{
    
    
    private Transform? _portalOut;
    private bool _notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private Transform portalIn;
    private readonly List<TransitioningObject> _objectsOnPortal  = new List<TransitioningObject>();
    // [SerializeField] private GameObject emptyClone;



    private void Start()
    {
        if (portalIn == null)
            throw new Exception("Portal In must be initialized in editor");
    }

    public void SetPortalOut(Transform portalOut)
    {
        this._portalOut = portalOut;
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
        // todo se esta haciendo un clon del clon, hay que checkear
        CreateClone(objectCrossing);
    }

    private void TriggerOnPortalEnter(TransitioningObject objectCrossing)
    {
        if(objectCrossing.GetImplementsIPortal())
            objectCrossing.GetOriginal().SendMessage("OnPortalEnter", portalIn.gameObject);
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
        var clone = Instantiate(objectCrossing, _portalOut.position, objectCrossing.transform.rotation, _portalOut);
        //todo create object tree
        ForwardEvents(clone, objectCrossing);
        var iPortal = objectCrossing.GetComponent<IPortal>();
        var objectOnPortal = new TransitioningObject(objectCrossing.transform, clone.transform, portalIn,_portalOut, iPortal!=null );
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
        
        
    }
    
    private void TriggerOnPortalExit(TransitioningObject leavingPortal)
    {
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalExit", portalIn.gameObject);
        //todo onPortalExit general
    }
    
    private void TriggerOnPortalTransitioning(TransitioningObject leavingPortal)
    {
        // todo send message o directamente llamar la funcion... como es un interface
        if(leavingPortal.GetImplementsIPortal())
            leavingPortal.GetOriginal().SendMessage("OnPortalTransitioning", portalIn.gameObject);
        //todo onPortalTransitioning general
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
        if (_portalOut == null) 
            return;
        Quaternion rotation = Quaternion.LookRotation(-portalIn.forward, portalIn.up);
        Quaternion relativeRot = Quaternion.Inverse(rotation) * transitioningObject.GetOriginal().rotation;
        transitioningObject.GetClone().rotation = _portalOut.rotation * relativeRot;
    }


}

