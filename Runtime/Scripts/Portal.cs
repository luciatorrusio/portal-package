
using System;
using GizmosExtendedNamespace;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

public class Portal : MonoBehaviour
{
    
    // SET VARIABLES
    private bool _notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private InPortal _inPortal;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private OutPortal _outPortal;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private PortalTextureSetup portalTextureSetup;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private GameObject transport;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private CameraOutMovement cameraOutMovement;
    [SerializeField] [CanBeNull] private Portal linkedOutPortal = null;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private Transform renderPlane;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private Transform frame;

    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private BoxCollider _collider;
    
    [SerializeField] private Vector3 scale = new Vector3(1, 1, 1);
    
    [NotNull] private Camera mainCamera;
    
    
    void Awake()
    {
        var camera = Camera.main;

        if (camera != null)
        {
            mainCamera = camera;
        }
            
    }
    void Start()
    {
        if (linkedOutPortal != null)
            SetAsInPortal();
        
    }

    private Camera GetMainCamera()
    {
        if (mainCamera == null)
            throw new Exception("no main camera found in the scene");
        return mainCamera;
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
        cameraOutMovement.SetCameraBeingReplicated(GetMainCamera());
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
        cameraOutMovement.SetCameraBeingReplicated(GetMainCamera());
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
            GizmosExtended.DrawArrow(linkedOutPortal.transform.position ,linkedOutPortal.transform.forward , Color.red);
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
    
    public Portal GetLinkedOutPortal()
    {
        return linkedOutPortal;
    }

    public void SetScale()
    {
        renderPlane.localScale = new Vector3(scale.x * 0.18f, 1, scale.y * 0.246f);
        frame.localScale = scale;
        _collider.center = new Vector3(0, 0, (scale.z * 1.67f) / 2);
        _collider.size =  new Vector3(scale.x * 1.8f, scale.y * 2.5f, scale.z * 1.67f);
    }
}
