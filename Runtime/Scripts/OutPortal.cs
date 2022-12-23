using GizmosExtendedNamespace;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

// using Utils;

public class OutPortal : MonoBehaviour
{
    [CanBeNull] private InPortal linkedInPortal;
    [CanBeNull] private Renderer linkedInPortalRenderer;
    private bool notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private CameraOutMovement _cameraOutMovement;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private Camera _camera;
    private void OnEnable()
    {
        PortalManager.AllOutPortals.Add(this);
    }
    
    private void OnDisable()
    {
        PortalManager.AllOutPortals.Remove(this);
    }
    
    public void Start()
    {
        if (linkedInPortal == null)
        {
            _camera.enabled = false;
            return;
        }
           
        _camera.enabled = true;
        _cameraOutMovement.SetPortalIn(linkedInPortal.transform, linkedInPortalRenderer);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (linkedInPortal != null)
        {
            GizmosExtended.DrawPlane(transform, new Vector2(2,3), Color.red);
            GizmosExtended.DrawPlane(linkedInPortal.transform, new Vector2(2,3), Color.green);
            GizmosExtended.DrawArrow(linkedInPortal.transform.position,transform.position- linkedInPortal.transform.position, Color.yellow, 2f, 40f);
            GizmosExtended.DrawArrow(linkedInPortal.transform.position+(linkedInPortal.transform.forward* 1f), -linkedInPortal.transform.forward, Color.green);
            GizmosExtended.DrawArrow(transform.position, -transform.forward, Color.red);
        }
        //     Gizmos.DrawLine(transform.position, linkedInPortal.transform.position);
            
    }
    public void SetLinkedInPortal(InPortal inPortal, Renderer renderer)
    {
        linkedInPortal = inPortal;
        linkedInPortalRenderer = renderer;
        _camera.enabled = true;
        _cameraOutMovement.SetPortalIn(linkedInPortal.transform, linkedInPortalRenderer);
    }

    public InPortal GetLinkedInPortal()
    {
        return linkedInPortal;
    }

    public void UnlinkOutPortal()
    {
        var currentLinkedPortal = linkedInPortal;
        linkedInPortal = null;
        if (currentLinkedPortal?.GetLinkedOutPortal() != null)
        {
            currentLinkedPortal.UnlinkInPortal();
        }
    }

    public void Visible()
    {
        _cameraOutMovement.enabled = true;
        _camera.enabled = true;
    }

    public void NotVisible()
    {
        _cameraOutMovement.enabled = false;
        _camera.enabled = false;
    }

    public Camera GetCamera()
    {
        return _camera;
    }
}
