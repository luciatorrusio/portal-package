using Scripts;
using UnityEngine;
using Utils;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector4 = UnityEngine.Vector4;

[RequireComponent(typeof(Camera))] 
public class CameraOutMovement : MonoBehaviour
{
    private Transform _cameraBeingReplicated;
    private bool _notBlocked = false;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private Transform portalOut;
    private Transform portalIn;
    private Renderer portalInRenderer;
    [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(_notBlocked))]
    [SerializeField] private Camera _camera;

    public void SetCameraBeingReplicated(Camera cameraBeingReplicated)
    {
        this._cameraBeingReplicated = cameraBeingReplicated.transform;
        _camera.fieldOfView = cameraBeingReplicated.fieldOfView;
    }

    public void SetPortalIn(Transform portalIn, Renderer portalInRenderer)
    {
        this.portalInRenderer = portalInRenderer;
        this.portalIn = portalIn;
    }
    private void Update()
    {
        
        if(portalIn == null)
            return;
        if(_cameraBeingReplicated == null)
            return;
        if (!_cameraBeingReplicated.IsInFrontOf(portalIn))
        {
            _camera.enabled = false;
            return;
        }
        _camera.enabled = true; 
        SetPosition();
        
        SetAngle();
        
        SetNearClippingPlane();
    }

    private void SetPosition()
    {
        var playerToPortal = portalIn.InverseTransformDirection(_cameraBeingReplicated.position - portalIn.position);
        Debug.DrawRay(portalIn.position, _cameraBeingReplicated.position - portalIn.position, Color.green);
        Debug.DrawRay(portalOut.position, playerToPortal, Color.green);
        // transform.localPosition = playerToPortal;
        transform.localPosition = new Vector3(-playerToPortal.x, playerToPortal.y, -playerToPortal.z) ;
        
    }

    private void SetAngle()
    {
        Quaternion rotation = Quaternion.LookRotation(-portalIn.forward, portalIn.up);
        Quaternion relativeRot = Quaternion.Inverse(rotation) * _cameraBeingReplicated.rotation;
        transform.rotation = portalOut.rotation * relativeRot;
        
        // var difRotation = Quaternion.FromToRotation(portalOut.forward, portalIn.forward);
        // transform.rotation = difRotation * transform.rotation;
        
        Debug.DrawRay(transform.position, transform.forward *10, Color.red);
        Debug.DrawRay(_cameraBeingReplicated.position, _cameraBeingReplicated.forward * 10, Color.red);
    }

    private void SetNearClippingPlane()
    {
        if (_camera.transform.IsInFrontOfWithError(portalOut, 0.5f))
            return;
        var p = new Plane(-portalOut.forward, portalOut.position);
        var clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        var clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(_camera.worldToCameraMatrix)) * clipPlane;
        var newMatrix = _camera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _camera.projectionMatrix = newMatrix;
    }
}
