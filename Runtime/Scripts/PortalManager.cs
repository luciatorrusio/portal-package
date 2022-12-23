using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static List<InPortal> AllInPortals = new List<InPortal>();
    public static List<OutPortal> AllOutPortals = new List<OutPortal>();
    
    [NotNull] private Transform mainCamera;

    void Start()
    {
        var camera = Camera.main;
        if (camera != null)
            mainCamera = camera.transform;
    }

    public Transform GetMainCamera()
    {
        return mainCamera;
    }
    
    private void OnDrawGizmosSelected()
    {
        foreach (var inPortal in AllInPortals)
        {
            // DrawArrow.ForGizmo( transform.position, transform.position - );
            Gizmos.DrawLine(transform.position,inPortal.transform.position);
        }
        foreach (var outPortal in AllOutPortals)
        {
            Gizmos.DrawLine(transform.position,outPortal.transform.position);
        }
        
    }

    
}
