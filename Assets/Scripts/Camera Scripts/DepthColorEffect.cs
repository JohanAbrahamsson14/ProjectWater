using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DepthColorEffect : MonoBehaviour
{
    public Material depthColorMaterial;

    private void Start()
    {
        if (depthColorMaterial == null)
        {
            Debug.LogError("DepthColorMaterial is not assigned.");
        }
        else
        {
            Debug.Log("DepthColorMaterial assigned successfully.");
        }

        // Additional Debug Log to confirm Start method is running
        Debug.Log("DepthColorEffect script started.");
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Debug.Log("OnRenderImage called."); // Debug log to confirm this method is called

        if (depthColorMaterial != null)
        {
            Debug.Log("Applying depth color material.");
            Graphics.Blit(src, dest, depthColorMaterial);
        }
        else
        {
            Debug.LogError("DepthColorMaterial is not assigned or missing.");
            Graphics.Blit(src, dest);
        }
    }
}