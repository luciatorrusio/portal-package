using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 mouseSensitivity = new Vector2(200, 100);
    [SerializeField] Transform cameraTransform;

    void Update()
    {
        var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity.x * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity.y * Time.deltaTime;
        cameraTransform.Rotate(Vector3.left * mouseY);

        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;

        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
    }
}
