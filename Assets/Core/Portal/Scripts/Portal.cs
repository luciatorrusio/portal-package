
using System;
using GizmosExtendedNamespace;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

public class Portal : MonoBehaviour
{
    
    // SET VARIABLES
    private bool _notBlocked = false;
    // [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [HideInInspector]
    [SerializeField] private InPortal _inPortal;
    
    [HideInInspector]
    [SerializeField] private OutPortal _outPortal;
    
    [HideInInspector]
    [SerializeField] private PortalTextureSetup portalTextureSetup;
    
    [SerializeField] [CanBeNull] private Portal linkedOutPortal = null;
    
    [HideInInspector]
    [SerializeField] private Transform renderPlane;
    
    [HideInInspector]
    [SerializeField] private Transform frame;
    
    [HideInInspector]
    [SerializeField] private PortalTransport portalTransport;

    [HideInInspector]
    [SerializeField] private BoxCollider _collider;

    [SerializeField] private Mesh PortalMesh = null;
    // [SerializeField] private Vector3 colliderScaleMultiplier = new Vector3(1, 1, 1);
    // [SerializeField] private Vector3 scale = new Vector3(1, 1, 1);
    
    [NotNull] private Camera mainCamera;
    [HideInInspector]
    [SerializeField] private Renderer _renderer;
    
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
            _inPortal.SetLinkedOutPortal(linkedOutPortal.GetOutPortal());
        }

        portalTextureSetup.gameObject.SetActive(true);
        gameObject.SetActive(true);
        linkedOutPortal.SetAsOutPortal();
        
        return _inPortal;
    }

    public OutPortal SetAsOutPortal()
    {
        _outPortal.enabled = true;
        return _outPortal;
    }

    private void Setup(bool isInPortal)
    {
        // InPortal
        _inPortal.enabled = isInPortal;
        linkedOutPortal = isInPortal ? linkedOutPortal: null;
        if (linkedOutPortal != null)
        {
            _inPortal.SetLinkedOutPortal(linkedOutPortal.GetOutPortal());
        }

        portalTextureSetup.gameObject.SetActive(isInPortal);
        gameObject.SetActive(isInPortal);
        
        // OutPortal
        _outPortal.enabled = !isInPortal;
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
        // renderPlane.localScale =new Vector3(scale.x, scale.z, scale.y);
        // frame.localScale = new Vector3(scale.x, scale.z, scale.y);
        // _collider.center = new Vector3(0, 0, (scale.z*colliderScaleMultiplier.z)/ 2);
        // _collider.size =  new Vector3(scale.x * colliderScaleMultiplier.x , scale.y*colliderScaleMultiplier.y, scale.z*colliderScaleMultiplier.z  );
    }

    public void SetMeshFilter()
    {
        if(PortalMesh == null)
            Debug.LogWarning("Mesh is null in " + gameObject.name);
        renderPlane.GetComponent<MeshFilter>().mesh = PortalMesh;
    }
    
    private void OnEnable()
    {
        PortalRecursion.AddPortal(this);
    }
    private void OnDisable()
    {
        PortalRecursion.RemovePortal(this);
    }

    public bool isVisible()
    {
        return _renderer.isVisible;
    }

    public RenderTexture GetRenderTexture()
    {
        return portalTextureSetup.GetRenderTexture();
    }

    public GameObject GetRenderPlane()
    {
        return portalTextureSetup.gameObject;
    }

    public void AddTransitioningObject(TransitioningObject transitioningObject)
    {
        portalTransport.AddTransitioningObject(transitioningObject);
    }
    
    
}
