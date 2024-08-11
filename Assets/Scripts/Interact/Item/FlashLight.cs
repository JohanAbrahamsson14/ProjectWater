using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : Tool
{
    public GameObject lightCone;
    public bool isLightActive;
    private Material flashLightMaterial;
    private Color flashLightOnColor;

    public void Awake()
    {
        flashLightMaterial = GetComponent<Renderer>().material;
        flashLightOnColor = flashLightMaterial.GetColor("_EmissionColor");
        ToolAction();
    }

    public override void ToolAction()
    {
        base.ToolAction();
        isLightActive = !isLightActive;
        lightCone.SetActive(isLightActive);
        flashLightMaterial.SetColor("_EmissionColor", isLightActive ? flashLightOnColor : Color.black);
    }
}
