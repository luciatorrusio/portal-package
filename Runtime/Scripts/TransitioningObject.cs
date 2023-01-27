using Scripts;
using UnityEngine;
using Utils;

public class TransitioningObject 
{
    private readonly Transform _original;
    private readonly Rigidbody _originalRigidbody;
    private readonly Transform _clone;
    private readonly GameObject _mainCamera;
    private readonly Transform _portalIn;
    private readonly Transform _portalOut;
    private readonly bool _implementsIPortal;

    public TransitioningObject(Transform original,Transform clone, Transform portalIn, Transform portalOut, bool implementsIPortal)
    {
        _original = original;
        _originalRigidbody = _original.GetComponent<Rigidbody>();
        _clone = clone;
        _mainCamera = _clone.GetMainCamera();
        if (!(_mainCamera == null)) 
            _mainCamera.SetActive(false);
        _portalIn = portalIn;
        _portalOut = portalOut;
        _implementsIPortal = implementsIPortal;
    }

    public void Transport()
    {
        
        _original.forward = _clone.forward;
        _original.rotation = _clone.rotation;
        _original.position = _clone.position;
        var newVelocity = PortalUtils.GetRelativeWorldDirection(_originalRigidbody.velocity, _portalIn, _portalOut);
        _originalRigidbody.velocity =  newVelocity ;
        _originalRigidbody.angularVelocity =   PortalUtils.GetRelativeWorldDirection(_originalRigidbody.angularVelocity, _portalIn, _portalOut); ;

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

    public bool GetImplementsIPortal()
    {
        return _implementsIPortal;
    }

    public Rigidbody GetOriginalRigidbody()
    {
        return _originalRigidbody;
    }
    
}
