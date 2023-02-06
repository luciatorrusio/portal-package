using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.PackageManager;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static List<InPortal> AllInPortals = new List<InPortal>();
    public static List<OutPortal> AllOutPortals = new List<OutPortal>();
    // private List<Camera> _allCameras = new List<Camera>();
    // private int _cameraCount;
    // private void Start()
    // {
    //     var all = Camera.allCameras;
    //     _cameraCount = Camera.allCamerasCount;
    //     foreach (var camera in all)
    //     {
    //         if(camera.gameObject.GetComponent<CameraOutMovement>() == null)
    //             _allCameras.Add(camera);
    //     }
    // }

    // private void Update()
    // {
    //     if (_cameraCount == Camera.allCamerasCount)
    //         return;
    //     var all = Camera.allCameras;
    //     foreach (var camera in all)
    //     {
    //         // if(!_allCameras.Contains(camera) && camera.gameObject.GetComponent<CameraOutMovement>() == null)
    //             
    //     }
    // }


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
