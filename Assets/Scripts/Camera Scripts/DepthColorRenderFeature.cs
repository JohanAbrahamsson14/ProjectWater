using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthColorRenderFeature : ScriptableRendererFeature
{
    class DepthColorRenderPass : ScriptableRenderPass
    {
        public Material material;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;

        public DepthColorRenderPass(Material material)
        {
            this.material = material;
            tempTexture.Init("_TempTexture");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("DepthColorPass");

            Blit(cmd, source, tempTexture.Identifier(), material);
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material material = null;
    }

    public Settings settings = new Settings();
    private DepthColorRenderPass renderPass;

    public override void Create()
    {
        renderPass = new DepthColorRenderPass(settings.material)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material != null)
        {
            renderPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(renderPass);
        }
    }
}
