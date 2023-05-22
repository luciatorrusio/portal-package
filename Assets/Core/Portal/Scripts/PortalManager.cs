using System;
using System.Collections.Generic;
using Core.Portal.Scripts;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static HashSet<Portal> allPortals { get; set; }
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        allPortals = new HashSet<Portal>();
    }

    private void Start()
    {
        
        mainCamera = mainCamera ? mainCamera : Camera.main;
    }

    #region API

    public Camera GetMainCamera()
    {
        return Camera.main;
    }

    public void SetMainCamera(Camera camera)
    {
        mainCamera = camera;
    }

    #endregion
    
    
    public static void AddPortal(Portal newPortal)
    {
        allPortals.Add(newPortal);
    }

    public static void RemovePortal(Portal portal)
    {
        allPortals.Remove(portal);
    }
}
