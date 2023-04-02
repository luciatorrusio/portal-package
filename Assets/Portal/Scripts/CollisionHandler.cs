using System;
using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;
using Physics = UnityEngine.Physics;

public class CollisionHandler : MonoBehaviour
{
    private Transform _portalIn;
    private Collider _collider;
    private List<Collider> ignoredColliders;

    private void Start()
    {
        ignoredColliders = new List<Collider>();
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.GetContact(0).point.IsInFrontOf(_portalIn))
        {
            var c = collision.gameObject.GetComponent<Collider>();
            ignoredColliders.Add(c);
            Physics.IgnoreCollision(_collider, c);
        }
    }

    private void OnDestroy()
    {
        foreach (var c in ignoredColliders)
        {
            print(c);
            Physics.IgnoreCollision( _collider, c, false);    
        }
        
    }

    public void SetPortal(Transform portalIn)
    {
        _portalIn = portalIn;
    }
}
