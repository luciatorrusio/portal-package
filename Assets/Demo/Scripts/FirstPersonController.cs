using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    public float mouseSensitivity = 100f;
    [SerializeField]
    public float moveSpeed = 5f;
    [SerializeField]
    private Transform playerBody;

    void Update()
    {
        // Mouse input for looking around
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        

        // Rotate the player body (horizontal rotation, around the Y-axis)
        playerBody.Rotate(Vector3.up * mouseX);
        // playerBody.Rotate(Vector3.left * mouseY);

        // // Control vertical rotation (looking up and down)
        // xRotation -= mouseY;
        //
        // // Apply vertical rotation to the camera (not the player body)
        // transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Keyboard input for moving (using arrow keys)
        float moveX = Input.GetAxis("Horizontal");  // Left/Right arrow (A/D or Left/Right arrow keys)
        float moveZ = Input.GetAxis("Vertical");    // Up/Down arrow (W/S or Up/Down arrow keys)
        float moveUp = Input.GetKey(KeyCode.Q)?1:0;
        float moveDown = Input.GetKey(KeyCode.E)?-1:0;

        Vector3 move = playerBody.right * moveX + playerBody.forward * moveZ + playerBody.up *moveUp + playerBody.up * moveDown;
        playerBody.position += move * moveSpeed * Time.deltaTime;
    }
}
