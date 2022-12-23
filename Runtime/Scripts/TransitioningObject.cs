using JetBrains.Annotations;
using PortalExtensionMethods;
// using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;
using Utils;

public class TransitioningObject 
{
    private readonly Transform _original;
    private readonly Rigidbody _originalRigidbody;
    private readonly Transform _clone;
    private readonly GameObject _mainCamera;
    private readonly Transform _portalIn;
    private EventForwarder _eventForwarder;

    public TransitioningObject(Transform original,Transform clone, Transform portalIn)
    {
        _original = original;
        _originalRigidbody = _original.GetComponent<Rigidbody>();
        _clone = clone;
        _mainCamera = _clone.GetMainCamera();
        if (!(_mainCamera == null)) 
            _mainCamera.SetActive(false);
        _portalIn = portalIn;
    }

    public void Transport()
    {
        
        var anglesDifferencePerAxis  = Quaternion.FromToRotation(_original.forward, _clone.forward).eulerAngles;
        _originalRigidbody.velocity =   Quaternion.AngleAxis(anglesDifferencePerAxis.y, Vector3.up) * Quaternion.AngleAxis(anglesDifferencePerAxis.z, Vector3.forward) *
                                        Quaternion.AngleAxis(anglesDifferencePerAxis.x, Vector3.right) * _originalRigidbody.velocity; ;
        
        _original.forward = _clone.forward;
        _original.rotation = _clone.rotation;
        _original.position = _clone.position;
        
    }
    

    public Transform GetOriginal()
    {
        return _original;
    }

    public GameObject GetMainCamera()
    {
        return _mainCamera;
    }

    public bool EnteredPortal()
    {
        return ! _original.transform.IsInFrontOf(_portalIn);
    }
    
    public Transform GetClone()
    {
        return _clone;
    }
    
    
}
