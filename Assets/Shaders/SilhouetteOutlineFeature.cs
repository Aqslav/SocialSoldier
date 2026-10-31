using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class SilhouetteOutlineFeature : ScriptableRendererFeature
{
    // Objects with this rendering-layer bit get outlined
    public const uint OutlineLayer = 1u << 20;

    public Material material;               // material using Custom/SilhouetteOutline
    public Color color = Color.white;
    [Range(1, 10)] public float width = 3;

    OutlinePass pass;

    public override void Create()
    {
        pass = new OutlinePass { renderPassEvent = RenderPassEvent.AfterRenderingTransparents };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null) return;
        if (renderingData.cameraData.cameraType == CameraType.Preview) return;
        material.SetColor("_OutlineColor", color);
        material.SetFloat("_OutlineWidth", width);
        pass.material = material;
        renderer.EnqueuePass(pass);
    }

    class OutlinePass : ScriptableRenderPass
    {
        public Material material;
        static readonly List<ShaderTagId> tags = new()
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit")
        };

        class MaskData { public RendererListHandle list; }
        class CompositeData { public Material mat; public TextureHandle mask; }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resources  = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            var renderData = frameData.Get<UniversalRenderingData>();
            var lightData  = frameData.Get<UniversalLightData>();

            // Mask texture (single channel, no MSAA, no depth)
            var desc = cameraData.cameraTargetDescriptor;
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;
            desc.colorFormat = RenderTextureFormat.R8;
            TextureHandle mask = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_OutlineMask", true);

            // Pass 1: draw outlined objects as white into the mask
            using (var builder = renderGraph.AddRasterRenderPass<MaskData>("Outline Mask", out var data))
            {
                var drawing = RenderingUtils.CreateDrawingSettings(tags, renderData, cameraData, lightData, SortingCriteria.None);
                drawing.overrideMaterial = material;
                drawing.overrideMaterialPassIndex = 0;
                var filter = new FilteringSettings(RenderQueueRange.all, -1, OutlineLayer);
                data.list = renderGraph.CreateRendererList(new RendererListParams(renderData.cullResults, drawing, filter));

                builder.UseRendererList(data.list);
                builder.SetRenderAttachment(mask, 0);
                builder.SetRenderFunc((MaskData d, RasterGraphContext ctx) => ctx.cmd.DrawRendererList(d.list));
            }

            // Pass 2: draw the line around the mask onto the camera image
            using (var builder = renderGraph.AddRasterRenderPass<CompositeData>("Outline Composite", out var data))
            {
                data.mat = material;
                data.mask = mask;
                builder.UseTexture(mask, AccessFlags.Read);
                builder.SetRenderAttachment(resources.activeColorTexture, 0);
                builder.SetRenderFunc((CompositeData d, RasterGraphContext ctx) =>
                    Blitter.BlitTexture(ctx.cmd, d.mask, new Vector4(1, 1, 0, 0), d.mat, 1));
            }
        }
    }
}