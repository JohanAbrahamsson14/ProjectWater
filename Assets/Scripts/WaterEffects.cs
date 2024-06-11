using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.RendererUtils;
using RenderSettings = UnityEngine.RenderSettings;

public class WaterEffects : MonoBehaviour
{
    public Light sun;
    
    public Color underwaterFogColor = new Color(0.1f, 0.4f, 0.6f, 1.0f);
    public float underwaterFogStart = 4;
    public float underwaterFogEnd = 35;
    public PostProcessProfile underwaterPostProcessProfile;
    public Color underwaterSunColor;
    
    private Color originalFogColor;
    private float originalFogStart = 15;
    private float originalFogEnd = 100;
    private PostProcessProfile originalPostProcessProfile;
    private Color orignialSunColor;
    
    void Start()
    {
        originalFogColor = RenderSettings.fogColor;
        originalFogStart = RenderSettings.fogStartDistance;
        originalFogEnd = RenderSettings.fogEndDistance;
        orignialSunColor = sun.color;
    }

    public void WaterEffectActive()
    {
        RenderSettings.fogColor = underwaterFogColor;
        RenderSettings.fogStartDistance = underwaterFogStart;
        RenderSettings.fogEndDistance = underwaterFogEnd;
        RenderSettings.ambientMode = AmbientMode.Trilight;
        sun.color = underwaterSunColor;
        if (underwaterPostProcessProfile != null)
        {
            var postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
            if (postProcessVolume != null)
            {
                originalPostProcessProfile = postProcessVolume.profile;
                postProcessVolume.profile = underwaterPostProcessProfile;
            }
        }
    }

    public void WaterEffectDisactive()
    {
        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogStartDistance = originalFogStart;
        RenderSettings.fogEndDistance = originalFogEnd;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        sun.color = orignialSunColor;
        if (underwaterPostProcessProfile != null)
        {
            var postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
            if (postProcessVolume != null)
            {
                postProcessVolume.profile = originalPostProcessProfile;
            }
        }
    }
}
