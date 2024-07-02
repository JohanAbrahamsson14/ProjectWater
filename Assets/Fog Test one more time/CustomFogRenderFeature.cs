using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomFogRenderFeature : ScriptableRendererFeature
{
    class CustomFogPass : ScriptableRenderPass
    {
        public Material fogMaterial = null;

        private RenderTargetHandle tempTexture;

        public CustomFogPass(Material material)
        {
            fogMaterial = material;
            tempTexture.Init("_TempTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (fogMaterial == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Custom Fog Effect");

            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

            cmd.Blit(source, tempTexture.Identifier());
            cmd.Blit(tempTexture.Identifier(), source, fogMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    [System.Serializable]
    public class CustomFogSettings
    {
        public Material fogMaterial = null;
    }

    public CustomFogSettings settings = new CustomFogSettings();
    CustomFogPass customFogPass;

    public override void Create()
    {
        customFogPass = new CustomFogPass(settings.fogMaterial)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.fogMaterial != null)
        {
            renderer.EnqueuePass(customFogPass);
        }
    }
}
