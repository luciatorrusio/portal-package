using System.Collections.Generic;
using Codice.Client.BaseCommands;
using Scripts;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utils;

public class TransitioningObject 
{
    private  Transform _original;
    private  Rigidbody _originalRigidbody;
    private  Transform _clone;
    private readonly GameObject _mainCamera;
    private  Portal  _portalIn;
    private Portal _portalOut;
    private readonly bool _implementsIPortal;
    private readonly IEnumerable<( Transform original, Transform clone)> _originalToCloneList;
    private readonly List<Material> _originalMaterials = new List<Material>();
    private readonly List<Material> _cloneMaterials = new List<Material>();

    public TransitioningObject(Transform original,Transform clone, Portal portalIn, Portal portalOut,IEnumerable<( Transform original, Transform clone)> originalToCloneList,  bool implementsIPortal)
    {
        _original = original;
        _originalRigidbody = _original.GetComponent<Rigidbody>();
        _clone = clone;
        _mainCamera = _clone.GetMainCamera();
        if (!(_mainCamera == null)) 
            _mainCamera.SetActive(false);
        _portalIn = portalIn;
        _portalOut = portalOut;
        _originalToCloneList= originalToCloneList;
        _implementsIPortal = implementsIPortal;
        _cloneMaterials.AddRange(SetMaterials(clone.gameObject));
        _originalMaterials.AddRange(SetMaterials(original.gameObject));
    }

    public void Transport()
    {
        _original.forward = _clone.forward;
        _original.rotation = _clone.rotation;
        _original.position = _clone.position;
        var newVelocity = PortalUtils.GetRelativeWorldDirection(_originalRigidbody.velocity, _portalIn.transform, _portalOut.transform);
        _originalRigidbody.velocity =  newVelocity ;
        _originalRigidbody.angularVelocity =   PortalUtils.GetRelativeWorldDirection(_originalRigidbody.angularVelocity, _portalIn.transform, _portalOut.transform); ;

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
        return ! _original.transform.IsInFrontOf(_portalIn.transform);
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

    public Portal GetPortalIn()
    {
        return _portalIn;
    }

    
    public void SwitchPortals(Portal portalIn, Portal portalOut)
    {
        _clone.gameObject.SetActive(false);
        
        _original.forward = _clone.forward;
        _original.rotation = _clone.rotation;
        (_original.position, _clone.position) = (_clone.position, _original.position);
        var newVelocity = PortalUtils.GetRelativeWorldDirection(_originalRigidbody.velocity, _portalIn.transform, _portalOut.transform);
        _originalRigidbody.velocity =  newVelocity ;
        _originalRigidbody.angularVelocity =   PortalUtils.GetRelativeWorldDirection(_originalRigidbody.angularVelocity, _portalIn.transform, _portalOut.transform); ;
        _clone.gameObject.SetActive(true);
        
        _clone.parent = portalOut.transform;
        _portalIn = portalIn;
        _portalOut = portalOut;
    }

    
    public Portal GetPortalOut()
    {
        return _portalOut;
    }
    
    public IEnumerable<( Transform original, Transform clone)> GetOriginalToCloneList()
    {
        return _originalToCloneList;
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
        var renderers = g.GetComponentsInChildren<MeshRenderer> ();
        var matList = new List<Material> ();
        foreach (var renderer in renderers) {
            foreach (var mat in renderer.materials) {
                matList.Add (mat);
            }
        }

        var skinnedRenderers = g.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in skinnedRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                matList.Add(mat);
            }
        }

        return matList;

    }
}
