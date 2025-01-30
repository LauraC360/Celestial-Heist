using UnityEngine.Rendering;

public static class Utils
{
    public static int GetRenderingLayerMaskFromString(string name)
    {
        var renderingLayerNames = GraphicsSettings.currentRenderPipeline?.renderingLayerMaskNames;
        if (renderingLayerNames == null) 
            return 0;

        for (int i = 0; i < renderingLayerNames.Length; i++)
            if (renderingLayerNames[i] == name)
                return 1 << i;

        return 0;
    }
}
