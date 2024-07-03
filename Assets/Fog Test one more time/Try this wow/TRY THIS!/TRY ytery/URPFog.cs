using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class URPFog : MonoBehaviour
{
    [Header("Fog Settings")]
    public Color fogColor = Color.gray;

    [Range(0.0f, 1.0f)]
    public float fogDensity = 0.05f;

    [Range(0.0f, 100.0f)]
    public float fogOffset = 0.0f;

    private Material fogMat;
    private static readonly int FogColorID = Shader.PropertyToID("_FogColor");
    private static readonly int FogDensityID = Shader.PropertyToID("_FogDensity");
    private static readonly int FogOffsetID = Shader.PropertyToID("_FogOffset");
    private static readonly int NearClipPlaneID = Shader.PropertyToID("_NearClipPlane");
    private static readonly int FarClipPlaneID = Shader.PropertyToID("_FarClipPlane");

    void Start()
    {
        Shader fogShader = Shader.Find("Hidden/URPFog");
        if (fogMat == null && fogShader != null)
        {
            fogMat = new Material(fogShader);
            fogMat.hideFlags = HideFlags.HideAndDontSave;
        }

        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (fogMat != null)
        {
            fogMat.SetColor(FogColorID, fogColor);
            fogMat.SetFloat(FogDensityID, fogDensity);
            fogMat.SetFloat(FogOffsetID, fogOffset);
            fogMat.SetFloat(NearClipPlaneID, camera.nearClipPlane);
            fogMat.SetFloat(FarClipPlaneID, camera.farClipPlane);

            var cmd = CommandBufferPool.Get("URPFogEffect");
            cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, fogMat);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
