using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer),PostProcessEvent.AfterStack, "Outline")]
public sealed class PostProcessOutline : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Outline intensity.")]
    public FloatParameter intensity = new FloatParameter { value = 0.5f };
    
    [Tooltip("Outline Thickness")]
    public FloatParameter thickness = new FloatParameter { value = 1.0f };

    [Tooltip("Minimum Depth")]
    public FloatParameter minDepth = new FloatParameter { value = 0.0f };

    [Tooltip("Maximum Depth")]
    public FloatParameter maxDepth = new FloatParameter { value = 1.0f };
}

public class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    private Shader shader;
    private Material material;
    
    public override void Init()
    {
        shader = Shader.Find("Hidden/Outline");
        material = new Material(shader);
    }    
        
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get("Hidden/Outline");
        //sheet.properties.SetFloat("_Intensity", settings.intensity);
        sheet.properties.SetFloat("_Thickness", settings.thickness);
        sheet.properties.SetFloat("_MinDepth", settings.minDepth);
        sheet.properties.SetFloat("_MaxDepth", settings.maxDepth);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}