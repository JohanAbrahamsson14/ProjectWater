using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material passMaterial;
        private RenderTargetIdentifier currentTarget;

        public CustomRenderPass(Material material)
        {
            this.passMaterial = material;
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
            RenderTargetIdentifier destination = RenderTargetHandle.CameraTarget.Identifier();

            cmd.Blit(source, destination, passMaterial);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
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
            renderPassEvent = RenderPassEvent.AfterRendering
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