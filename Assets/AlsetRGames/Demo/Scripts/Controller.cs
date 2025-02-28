using System.Collections.Generic;
using AlsetRGames.Portal.Core;
using AlsetRGames.Portal.Support;
using UnityEngine;
using Physics = AlsetRGames.Portal.Support.Physics;

namespace Demo.Scripts
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] float speed = 5f;
        [SerializeField] LayerMask layerToIgnore;
        [SerializeField] float gravity = -9.8f;
        [SerializeField] float jumpImpulse = 20;
        [SerializeField] float recoverVelocity = 5f;
        [SerializeField] private float pushForce = 5f; // Strength of the push
        [SerializeField] private float grabDistance = 2f; // How close you need to be to grab
        [SerializeField] private Transform grabPoint; // Where the object will be held (usually in front of the player)
        [SerializeField] private float grabForce = 10f; // How much force is applied to the object when grabbed
        [SerializeField] private Transform camera; // How much force is applied to the object when grabbed
        private GameObject grabbedObject;
        private Transform parent;
        private bool isGrabbing = false;
        CharacterController controller;
        float verticalSpeed = 0;
        [SerializeField] private GameObject objectPrefab; // Assign in Inspector
        [SerializeField] private Transform throwPoint;    // Point where the object will be instantiated
        [SerializeField] private float throwForce = 10f;  // Adjust force as needed


        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            ComputePickingObjects();
            ComputeThrowObject();
            var velocity = ComputeWalkVelocity() + ComputeVerticalVelocity();
            transform.forward = ComputeVerticalRecover(transform.forward);
            var collisions = controller.Move(velocity * Time.deltaTime);
            HandleCollisions(collisions);
        }

        private void ComputeThrowObject()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ThrowObject();
            }
        }
        
        private void ThrowObject()
        {
            if (objectPrefab == null || throwPoint == null)
            {
                Debug.LogWarning("Object Prefab or Throw Point not assigned!");
                return;
            }

            // Instantiate object at throw position
            GameObject newObject = Instantiate(objectPrefab, throwPoint.position, throwPoint.rotation);

            // Add Rigidbody and apply force
            Rigidbody rb = newObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
            }
            // Destroy the object after 10 seconds
            Destroy(newObject, 10f);
        }
        private void ComputePickingObjects()
        {
            if (Input.GetKeyDown(KeyCode.E) && !isGrabbing)
            {
                TryGrabObject();
            }
            else if (Input.GetKeyDown(KeyCode.E) && isGrabbing)
            {
                ReleaseObject();
            }

            // If the object is grabbed, move it with the player
            if (isGrabbing && grabbedObject != null)
            {
                MoveGrabbedObject();
            }
        }
        void TryGrabObject()
        {
            // Check for objects within the grab range
            List<RaycastHit> hit;
            if (Physics.PortalRaycast(camera.position, camera.forward, out hit, grabDistance, layerMask:~layerToIgnore))
            {
                var lastIndex = hit.Count - 1;
                print(hit[lastIndex].collider.name);
                if (hit[lastIndex].collider.CompareTag("Grabbable"))
                {
                    
                    grabbedObject = hit[lastIndex].collider.gameObject;
                    parent = grabbedObject.transform.parent;
                    isGrabbing = true;
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics temporarily
                    grabbedObject.transform.SetParent(grabPoint); // Parent the object to the grab point
                    grabbedObject.transform.localPosition = Vector3.zero; // Position the object correctly in front of the player
                    hit[lastIndex].collider.enabled = false;
                    
                }
            }
        }

        void ReleaseObject()
        {
            if (grabbedObject != null)
            {
                
                grabbedObject.transform.SetParent(parent); // Unparent the object
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false; // Re-enable physics
                grabbedObject.GetComponent<Rigidbody>().AddForce(camera.forward * grabForce, ForceMode.Impulse); // Apply force to give it a little push
                grabbedObject.GetComponent<Collider>().enabled = true;
                grabbedObject = null;
                isGrabbing = false;
               
                
            }
        }

        void MoveGrabbedObject()
        {
            grabbedObject.transform.position = grabPoint.position;
        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var rb = hit.collider.attachedRigidbody;
            if (rb == null || rb.isKinematic)
                return;
            var forceDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rb.AddForce(forceDirection * pushForce, ForceMode.Impulse);
        }

        Vector3 ComputeVerticalVelocity()
        {
            verticalSpeed += gravity * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
                verticalSpeed += jumpImpulse;

            return verticalSpeed * Vector3.up;
        }

        Vector3 ComputeWalkVelocity() => (
            GetMovementFromInput("Vertical", transform.forward) +
            GetMovementFromInput("Horizontal", transform.right)
        ).normalized * speed;

        Vector3 ComputeVerticalRecover(Vector3 forward) =>
            Vector3.MoveTowards(
                forward,
                new Vector3(forward.x, 0, forward.z),
                recoverVelocity * Time.deltaTime
            );

        void HandleCollisions(CollisionFlags collisions)
        {
            if ((collisions & CollisionFlags.Below) != 0)
                verticalSpeed = 0;
        }

        static Vector3 GetMovementFromInput(string axis, Vector3 vector) =>
            Input.GetAxis(axis) * new Vector3(vector.x, 0, vector.z);
    }
}
