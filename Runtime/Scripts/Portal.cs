
using GizmosExtendedNamespace;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Utils;

// using Utils;

public class Portal : MonoBehaviour
{
    
    // SET VARIABLES
    private bool notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private InPortal _inPortal;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private OutPortal _outPortal;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private PortalTextureSetup portalTextureSetup;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private GameObject transport;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] private CameraOutMovement cameraOutMovement;
    
    // [SerializeField]private bool isInPortal;
    
    // [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.Not, nameof(isInPortal))]
    [SerializeField] private Transform cameraBeingReplicated;
    
    // [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(isInPortal))]
    // [SerializeField] [CanBeNull] private OutPortal linkedOutPortal = null;
    [SerializeField] [CanBeNull] private Portal linkedOutPortal = null;
    
    void Start()
    {
        if (linkedOutPortal != null)
            SetAsInPortal();
        
    }

    public InPortal SetAsInPortal()
    {
        // InPortal
        _inPortal.enabled = true;
        if (linkedOutPortal != null)
        {
            portalTextureSetup.SetCameraOut(linkedOutPortal.GetCamera());
            _inPortal.SetLinkedOutPortal(linkedOutPortal.GetOutPortal());
        }

        portalTextureSetup.gameObject.SetActive(true);
        transport.SetActive(true);
        linkedOutPortal.SetAsOutPortal();
        
        return _inPortal;
    }

    public OutPortal SetAsOutPortal()
    {
        // OutPortal
        _outPortal.enabled = true;
        cameraOutMovement.SetCameraBeingReplicated(cameraBeingReplicated);
        cameraOutMovement.gameObject.SetActive(true);
        return _outPortal;
    }

    private void Setup(bool isInPortal)
    {
        // InPortal
        _inPortal.enabled = isInPortal;
        linkedOutPortal = isInPortal ? linkedOutPortal: null;
        if (linkedOutPortal != null)
        {
            portalTextureSetup.SetCameraOut(linkedOutPortal.GetCamera());
            _inPortal.SetLinkedOutPortal(linkedOutPortal.GetOutPortal());
        }

        portalTextureSetup.gameObject.SetActive(isInPortal);
        transport.SetActive(isInPortal);
        
        // OutPortal
        _outPortal.enabled = !isInPortal;
        cameraOutMovement.SetCameraBeingReplicated(cameraBeingReplicated);
        cameraOutMovement.gameObject.SetActive(!isInPortal);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (linkedOutPortal != null)
        {
            GizmosExtended.DrawPlane(transform, new Vector2(2,3), Color.green);
            GizmosExtended.DrawPlane(linkedOutPortal.transform, new Vector2(2,3), Color.red);
            GizmosExtended.DrawArrow(transform.position,linkedOutPortal.transform.position- transform.position, Color.yellow, 2f, 40f);
            GizmosExtended.DrawArrow(linkedOutPortal.transform.position,-linkedOutPortal.transform.forward , Color.red);
            GizmosExtended.DrawArrow(transform.position+(transform.forward* 1f), -transform.forward, Color.green);
        }
            
        
    }


    public Camera GetCamera()
    {
        return _outPortal.GetCamera();
    }

    public OutPortal GetOutPortal()
    {
        return _outPortal;
    }
}
