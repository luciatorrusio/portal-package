
using JetBrains.Annotations;
using UnityEngine;
using Utils;

public class InPortal : MonoBehaviour
{
    [CanBeNull] private OutPortal linkedOutPortal = null;
    private bool notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] [NotNull]private PortalTransport _portalTransport;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField] [NotNull]private PortalTextureSetup _portalTextureSetup;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(notBlocked))]
    [SerializeField][NotNull] private Renderer _portalInRenderer;
    
    private void OnEnable()
    {
        Setup();
        PortalManager.AllInPortals.Add(this);
    }
    private void OnDisable()
    {
        if (linkedOutPortal == null)
            return;
        PortalManager.AllInPortals.Remove(this);
    }

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        if (linkedOutPortal == null)
        {
            _portalTextureSetup.SetDefaultMaterial();
            return;
        }
        _portalTextureSetup.SetCameraOut(linkedOutPortal.GetCamera());
        linkedOutPortal.SetLinkedInPortal(this, _portalInRenderer);
        _portalTransport.SetPortalOut(linkedOutPortal.transform);
    }

    public void SetLinkedOutPortal(OutPortal outPortal)
    {
        linkedOutPortal = outPortal;
        //todo check null
        _portalTextureSetup.SetCameraOut(linkedOutPortal.GetCamera());
        linkedOutPortal.SetLinkedInPortal(this, _portalInRenderer);
        _portalTransport.SetPortalOut(linkedOutPortal.transform);
        PortalManager.AllInPortals.Add(this);
    }
    public OutPortal GetLinkedOutPortal()
    {
        return linkedOutPortal;
    }

    public void UnlinkInPortal()
    {
        var currentLinkedPortal = linkedOutPortal;
        linkedOutPortal = null;
        if (currentLinkedPortal?.GetLinkedInPortal() != null)
        {
            currentLinkedPortal.UnlinkOutPortal();
        }
    }

    public void NotVisible()
    {
        if(linkedOutPortal== null)
            return;
        linkedOutPortal.NotVisible();
    }
    public void Visible()
    {
        if(linkedOutPortal== null)
            return;
        linkedOutPortal.Visible();
    }
}
