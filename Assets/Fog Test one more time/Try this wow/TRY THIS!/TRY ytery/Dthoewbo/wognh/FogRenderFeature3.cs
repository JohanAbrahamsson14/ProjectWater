using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogRenderFeature3 : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material fogMaterial = null;
        private RenderTargetIdentifier currentTarget;
        private RenderTargetHandle temporaryColorTexture;

        public CustomRenderPass(Material material)
        {
            this.fogMaterial = material;
            temporaryColorTexture.Init("_TemporaryColorTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(temporaryColorTexture.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("FogEffect");

            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

            Blit(cmd, source, temporaryColorTexture.Identifier(), fogMaterial, 0);
            Blit(cmd, temporaryColorTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
        }
    }

    [System.Serializable]
    public class FogSettings
    {
        public Material fogMaterial = null;
    }

    public FogSettings settings = new FogSettings();
    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings.fogMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.fogMaterial == null)
        {
            Debug.LogWarning("Missing Fog Material. FogRenderFeature will not execute.");
            return;
        }

        m_ScriptablePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
