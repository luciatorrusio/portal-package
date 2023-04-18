using System.Collections.Generic;
using Codice.Client.BaseCommands;
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
    private readonly List<KeyValuePair<Transform, Transform>> _originalToCloneList = new List<KeyValuePair<Transform, Transform>>();
    private readonly List<Material> _originalMaterials = new List<Material>();
    private readonly List<Material> _cloneMaterials = new List<Material>();

    public TransitioningObject(Transform original,Transform clone, Transform portalIn, Transform portalOut,List<KeyValuePair<Transform, Transform>> originalToCloneList,  bool implementsIPortal)
    {
        _original = original;
        _originalRigidbody = _original.GetComponent<Rigidbody>();
        _clone = clone;
        _mainCamera = _clone.GetMainCamera();
        if (!(_mainCamera == null)) 
            _mainCamera.SetActive(false);
        _portalIn = portalIn;
        _portalOut = portalOut;
        _originalToCloneList.AddRange(originalToCloneList);
        _implementsIPortal = implementsIPortal;
        _cloneMaterials.AddRange(SetMaterials(clone.gameObject));
        _originalMaterials.AddRange(SetMaterials(original.gameObject));
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

    public Transform GetPortalIn()
    {
        return _portalIn;
    }

    public Transform GetPortalOut()
    {
        return _portalOut;
    }

    public List<KeyValuePair<Transform, Transform>> GetOriginalToCloneList()
    {
        return _originalToCloneList;
    }

    public IEnumerable<( Transform original, Transform clone)> GetOriginalToCloneList2()
    {
        return null;
            //todo
    }

    public List<Material> GetCloneMaterials()
    {
        return _cloneMaterials;
    }
    public List<Material> GetOriginalMaterials()
    {
        return _originalMaterials;
    }
    
    private List<Material> SetMaterials (GameObject g) {
        // todo check skinned mesh renderer
        var renderers = g.GetComponentsInChildren<MeshRenderer> ();
        var matList = new List<Material> ();
        foreach (var renderer in renderers) {
            foreach (var mat in renderer.materials) {
                matList.Add (mat);
            }
        }

        return matList;

    }
}
