using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature2 : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material passMaterial;
        private RenderTargetHandle tempTexture;

        public CustomRenderPass(Material material)
        {
            this.passMaterial = material;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            tempTexture.Init("_TemporaryColorTexture");
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (passMaterial == null)
            {
                Debug.LogError("Material is missing!");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("Custom Render Pass");

            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;
            RenderTargetIdentifier destination = tempTexture.Identifier();

            cmd.Blit(source, destination, passMaterial);
            cmd.Blit(destination, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    [System.Serializable]
    public class CustomRenderPassSettings
    {
        public Material material = null;
    }

    public CustomRenderPassSettings settings = new CustomRenderPassSettings();
    private CustomRenderPass renderPass;

    public override void Create()
    {
        renderPass = new CustomRenderPass(settings.material)
        {
            renderPassEvent = RenderPassEvent.BeforeRendering
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material != null)
        {
            renderer.EnqueuePass(renderPass);
        }
    }
}
