using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WaterEffects : MonoBehaviour
{
    public Color underwaterFogColor = new Color(0.1f, 0.4f, 0.6f, 1.0f);
    public float underwaterFogStart = 4;
    public float underwaterFogEnd = 35;
    public PostProcessProfile underwaterPostProcessProfile;
    
    private Color originalFogColor;
    private float originalFogStart = 15;
    private float originalFogEnd = 100;
    private PostProcessProfile originalPostProcessProfile;
    
    void Start()
    {
        originalFogColor = RenderSettings.fogColor;
        originalFogStart = RenderSettings.fogStartDistance;
        originalFogEnd = RenderSettings.fogEndDistance;
    }

    public void WaterEffectActive()
    {
        RenderSettings.fogColor = underwaterFogColor;
        RenderSettings.fogStartDistance = underwaterFogStart;
        RenderSettings.fogEndDistance = underwaterFogEnd;
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
