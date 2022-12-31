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

    public TransitioningObject(Transform original,Transform clone, Transform portalIn, Transform portalOut)
    {
        _original = original;
        _originalRigidbody = _original.GetComponent<Rigidbody>();
        _clone = clone;
        _mainCamera = _clone.GetMainCamera();
        if (!(_mainCamera == null)) 
            _mainCamera.SetActive(false);
        _portalIn = portalIn;
        _portalOut = portalOut;
    }

    public void Transport()
    {
        var anglesDifferencePerAxis  = Quaternion.FromToRotation(_portalOut.forward, _portalIn.forward);
        
        var newVelocity = anglesDifferencePerAxis * Quaternion.AngleAxis( 180,  _portalOut.up)* _originalRigidbody.velocity;
        
        _originalRigidbody.velocity =  newVelocity ;
        
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
