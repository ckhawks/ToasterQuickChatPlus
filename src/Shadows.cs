using System.Reflection;
using UnityEngine.Rendering.Universal;

namespace ToasterCrispyShadows;

public static class Shadows
{
    static readonly FieldInfo _rpField = typeof(PostProcessingManager)
        .GetField("renderPipelineAsset", 
            BindingFlags.Instance | BindingFlags.NonPublic);
    
    public static void UpdateShadows()
    {
        PostProcessingManager ppm = MonoBehaviourSingleton<PostProcessingManager>.Instance;

        if (ppm == null)
        {
            Plugin.LogError("PostProcessingManager is null :(");
        }
        
        if (_rpField == null)
        {
            Plugin.Log("ERROR: FieldInfo for renderPipelineAsset is null!");
            return;
        }

        // 2. Pull out the URP asset
        var rpAsset = (UniversalRenderPipelineAsset)
            _rpField.GetValue(ppm);
        if (rpAsset == null)
        {
            Plugin.Log("ERROR: renderPipelineAsset came back null!");
            return;
        }
        
        // now you can mutate it
        rpAsset.shadowCascadeCount = 4;
        rpAsset.shadowDistance = Plugin.modSettings.ShadowDistance; // TODO put this in config file
        rpAsset.mainLightShadowmapResolution = Plugin.modSettings.ShadowResolution; // TODO put this in config file
        
        Plugin.Log("Updated shadow distance and shadowmap resolution values.");
    }
    
    // Plugin.Log.LogInfo($"Shadow distance: {__instance.renderPipelineAsset.shadowDistance}");
    // Plugin.Log.LogInfo($"Shadow cascade count: {__instance.renderPipelineAsset.shadowCascadeCount}");
    // Plugin.Log.LogInfo($"Shadow cascade count: {__instance.renderPipelineAsset.m_ShadowCascadeCount}");
    // Plugin.Log.LogInfo($"Shadow cascade option: {__instance.renderPipelineAsset.shadowCascadeOption}");
    // Plugin.Log.LogInfo($"Shadow cascades: {__instance.renderPipelineAsset.m_ShadowCascades}");
    // Plugin.Log.LogInfo($"Shadow cascade 2 split: {__instance.renderPipelineAsset.cascade2Split}");
    // Plugin.Log.LogInfo($"Shadow cascade 3 split: {__instance.renderPipelineAsset.cascade3Split}");
    // Plugin.Log.LogInfo($"Shadow cascade 4 split: {__instance.renderPipelineAsset.cascade4Split}");
    // Plugin.Log.LogInfo($"Shadow cascade border: {__instance.renderPipelineAsset.cascadeBorder}");
    // Plugin.Log.LogInfo($"Shadow cascade border: {__instance.renderPipelineAsset.cascadeBorder}");
    // [Info   :Toaster Top Down] PostProcessingManager.Awake
    // [Info   :Toaster Top Down] Shadow distance: 10
    // [Info   :Toaster Top Down] Shadow cascade count: 3
    // [Info   :Toaster Top Down] Shadow cascade count: 3
    // [Info   :Toaster Top Down] Shadow cascades: NoCascades
    // [Info   :Toaster Top Down] Shadow cascade 2 split: 0.25
    // [Info   :Toaster Top Down] Shadow cascade 3 split: (0.10, 0.30)
    // [Info   :Toaster Top Down] Shadow cascade 4 split: (0.07, 0.20, 0.47)
    // [Info   :Toaster Top Down] Shadow cascade border: 0.1
    // [Info   :Toaster Top Down] Shadow cascade border: 0.1
}