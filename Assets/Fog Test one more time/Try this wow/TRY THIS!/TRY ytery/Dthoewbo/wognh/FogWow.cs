using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class FogWow : MonoBehaviour {
    [Header("Fog")]
    public Shader fogShader;
    public Color fogColor;
    
    [Range(0.0f, 1.0f)]
    public float fogDensity;
    
    [Range(0.0f, 100.0f)]
    public float fogOffset;
    
    private Material fogMat;

    void Start() {
        if (fogMat == null) {
            fogMat = new Material(fogShader);
            fogMat.hideFlags = HideFlags.HideAndDontSave;
        }

        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
    }

    void Update() {
        if (fogMat != null) {
            fogMat.SetVector("_FogColor", fogColor);
            fogMat.SetFloat("_FogDensity", fogDensity);
            fogMat.SetFloat("_FogOffset", fogOffset);
        }
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (fogMat == null) {
            fogMat = new Material(fogShader);
        }
        fogMat.SetVector("_FogColor", fogColor);
        fogMat.SetFloat("_FogDensity", fogDensity);
        fogMat.SetFloat("_FogOffset", fogOffset);
        Graphics.Blit(source, destination, fogMat);
    }
}