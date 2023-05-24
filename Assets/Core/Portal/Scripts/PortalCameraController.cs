using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector4 = UnityEngine.Vector4;

namespace Core.Portal.Scripts
{
    [RequireComponent(typeof(Camera))] 
    public class PortalCameraController : MonoBehaviour
    {
        private Transform _cameraBeingReplicated;
        private Transform _portalOut;
        private Transform _portalIn;
        [HideInInspector]
        [SerializeField] private Camera _camera;

        public void SetCameraBeingReplicated(Camera cameraBeingReplicated)
        {
            _cameraBeingReplicated = cameraBeingReplicated.transform;
            _camera.fieldOfView = cameraBeingReplicated.fieldOfView;
        }

        public void SetPortalIn(Transform portalIn)
        {
            _portalIn = portalIn;
        }
        public void SetPortalOut(Transform portalOut)
        {
            _portalOut = portalOut;
        }
        public void SetPositionAndAngle()
        {
        
            if(_portalIn == null)
                return;
            if(_cameraBeingReplicated == null)
                return;
            if (!_cameraBeingReplicated.IsInFrontOf(_portalIn))
                return;
            SetPosition();
        
            SetAngle();
        }
    

        private void SetPosition()
        {
            Gizmos.color = Color.red;
            Debug.DrawRay(_portalIn.position, _cameraBeingReplicated.position - _portalIn.position   , Color.green);
            Vector3 relativePos = _portalIn.InverseTransformPoint(_cameraBeingReplicated.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        
            _camera.transform.position = _portalOut.TransformPoint(relativePos);
#if UNITY_EDITOR
            Debug.DrawRay(_portalOut.position, _camera.transform.position - _portalOut.position  , Color.blue);
#endif
        }

        private void SetAngle()
        {
            Quaternion rotation = Quaternion.LookRotation(-_portalIn.forward, _portalIn.up);
            Quaternion relativeRot = Quaternion.Inverse(rotation) * _cameraBeingReplicated.rotation;
            transform.rotation = _portalOut.rotation * relativeRot;
        }

        public void SetNearClippingPlane()
        {
            if (_camera.transform.IsInFrontOfWithError(_portalOut, 0.5f))
                return;
            var p = new Plane(_portalOut.forward, _portalOut.position);
            var clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            var clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(_camera.worldToCameraMatrix)) * clipPlane;
            var newMatrix = _camera.CalculateObliqueMatrix(clipPlaneCameraSpace);
            _camera.projectionMatrix = newMatrix;
        }
    
    }
}
