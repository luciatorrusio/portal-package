
using PortalExtensionMethods;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector4 = UnityEngine.Vector4;

[RequireComponent(typeof(Camera))] 
public class CameraOutMovement : MonoBehaviour
{
    private Transform cameraBeingReplicated;
    [SerializeField] private Transform portalOut;
    private Transform portalIn;
    private Renderer portalInRenderer;
    private Camera _camera;
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        
    }

    public void SetCameraBeingReplicated(Transform cameraBeingReplicated)
    {
        this.cameraBeingReplicated = cameraBeingReplicated;
        _camera.fieldOfView = this.cameraBeingReplicated.GetComponent<Camera>().fieldOfView;
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
        if(cameraBeingReplicated == null)
            return;
        if (!cameraBeingReplicated.IsInFrontOf(portalIn))
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
        var playerToPortal = portalIn.InverseTransformDirection(cameraBeingReplicated.position - portalIn.position);
        Debug.DrawRay(portalIn.position, cameraBeingReplicated.position - portalIn.position, Color.green);
        Debug.DrawRay(portalOut.position, playerToPortal, Color.green);
        transform.localPosition = playerToPortal;
    }

    private void SetAngle()
    {
        Quaternion relativeRot = Quaternion.Inverse(portalIn.rotation) * cameraBeingReplicated.rotation;
        transform.rotation = portalOut.rotation * relativeRot;
        Debug.DrawRay(transform.position, transform.forward *10, Color.red);
        Debug.DrawRay(cameraBeingReplicated.position, cameraBeingReplicated.forward * 10, Color.red);
    }

    private void SetNearClippingPlane()
    {
        if (!_camera.transform.IsInFrontOfWithError(portalOut, 0.5f))
            return;
        var p = new Plane(-portalOut.forward, portalOut.position);
        var clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        var clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(_camera.worldToCameraMatrix)) * clipPlane;
        var newMatrix = _camera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        _camera.projectionMatrix = newMatrix;
    }
}
