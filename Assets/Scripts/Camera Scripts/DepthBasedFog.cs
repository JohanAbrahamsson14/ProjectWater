using UnityEngine;

public class DepthBasedFog : MonoBehaviour
{
    public Color upFogColor = Color.cyan;  // Fog color when looking up
    public Color downFogColor = Color.blue; // Fog color when looking down
    public float transitionSpeed = 1f;  // Speed of fog color change

    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponent<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError("DepthBasedFog script needs to be attached to a Camera.");
        }
    }

    void Update()
    {
        AdjustFogBasedOnCameraAngle();
    }

    void AdjustFogBasedOnCameraAngle()
    {
        // Get the forward direction of the camera
        Vector3 cameraForward = playerCamera.transform.forward;

        // Calculate the angle between the forward direction and the upward vector (0 to 180 degrees)
        float angle = Vector3.Angle(cameraForward, Vector3.up);

        // Normalize the angle to a value between 0 and 1
        float normalizedAngle = angle / 180f;

        // Calculate the target fog color based on the normalized angle
        Color targetFogColor = Color.Lerp(upFogColor, downFogColor, normalizedAngle);

        // Smoothly transition the current fog color to the target fog color
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetFogColor, transitionSpeed * Time.deltaTime);
    }
}