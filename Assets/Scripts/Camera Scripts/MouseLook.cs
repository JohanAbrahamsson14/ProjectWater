using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public FirstPersonController player;
    float xRotation = 0f;
    public bool mouseActive;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            mouseActive = !mouseActive;
            Cursor.visible = mouseActive;
            Cursor.lockState = mouseActive ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        if(player.isGrabbed || Cursor.lockState == CursorLockMode.None) return;
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
