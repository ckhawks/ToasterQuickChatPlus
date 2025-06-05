using System;
using System.Reflection;
using HarmonyLib;
using Linework.SoftOutline;
using UnityEngine;

namespace ToasterPuckFX;

public static class PuckEffects
{
    static readonly FieldInfo _puckElevationIndicatorField = typeof(PuckElevationIndicatorController)
        .GetField("puckElevationIndicator",
            BindingFlags.Instance | BindingFlags.NonPublic);

    static readonly FieldInfo _lineRendererField = typeof(PuckElevationIndicator)
        .GetField("lineRenderer",
            BindingFlags.Instance | BindingFlags.NonPublic);
    
    static readonly FieldInfo _puckElevationIndicatorMaterialField = typeof(PuckElevationIndicator)
        .GetField("material",
            BindingFlags.Instance | BindingFlags.NonPublic);

    static readonly FieldInfo _puckOutlineSettingsField = typeof(PostProcessingManager)
        .GetField("puckOutlineSettings",
            BindingFlags.Instance | BindingFlags.NonPublic);

    public static void SetupPuckOutlineSettings()
    {
        PostProcessingManager ppm = PostProcessingManager.Instance;

        // The documentation for this outline library is here
        // https://linework.ameye.dev/soft-outline/

        SoftOutlineSettings puckOutlineSettings = (SoftOutlineSettings) _puckOutlineSettingsField.GetValue(ppm);

        // Default value for this is Color(1, 1, 1, 1)
        puckOutlineSettings.sharedColor = new Color(Plugin.modSettings.PuckOutlineR, Plugin.modSettings.PuckOutlineG,
            Plugin.modSettings.PuckOutlineB, 1f); // not using: Plugin.outline_a.Value
        puckOutlineSettings.kernelSize = Plugin.modSettings.PuckOutlineKernelSize;

        _puckOutlineSettingsField.SetValue(ppm, puckOutlineSettings);

        // didn't really seem like these settings did/changed anything
        // ppm.puckOutlineSettings.blurSpread = 20f;
        // ppm.puckOutlineSettings.intensity = 10;
        // ppm.puckOutlineSettings.gap = 0.5f;
        // ppm.puckOutlineSettings.blurSpread = 0.5f;
    }

    // Modify the verticality line
    [HarmonyPatch(typeof(PuckElevationIndicatorController), nameof(PuckElevationIndicatorController.Start))]
    public static class VerticalityLinePatch
    {
        [HarmonyPostfix]
        public static void Postfix(PuckElevationIndicatorController __instance)
        {
            Color lineColor = new Color(
                Plugin.modSettings.VerticalityLineR, Plugin.modSettings.VerticalityLineG,
                Plugin.modSettings.VerticalityLineB, Plugin.modSettings.VerticalityLineA);

            if (_puckElevationIndicatorField == null)
            {
                Plugin.LogError("ERROR: FieldInfo for _puckElevationIndicatorField is null!");
                return;
            }

            // PuckElevationIndicator puckElevationIndicator = (PuckElevationIndicator)
            //     _puckElevationIndicatorField.GetValue(__instance);

            PuckElevationIndicator puckElevationIndicator = __instance.GetComponent<PuckElevationIndicator>();

            if (_lineRendererField == null)
            {
                Plugin.LogError("ERROR: FieldInfo for _lineRendererField is null!");
                return;
            }
            
            if (_puckElevationIndicatorMaterialField == null)
            {
                Plugin.LogError("ERROR: FieldInfo for _puckElevationIndicatorMaterialField is null!");
                return;
            }
            
            Material puckElevationIndicatorMaterial = (Material) _puckElevationIndicatorMaterialField.GetValue(puckElevationIndicator);

            if (puckElevationIndicatorMaterial == null)
            {
                Plugin.LogError("ERROR: puckElevationIndicatorMaterial is null!");
            }
                
            // Plugin.Log($"puckElevationIndicatorMaterial color: {puckElevationIndicatorMaterial.color}");
            // DumpAllShaderProperties(puckElevationIndicatorMaterial.shader);
            Color elevationIndicatorColor = new Color(Plugin.modSettings.ElevationIndicatorR,
                Plugin.modSettings.ElevationIndicatorG, Plugin.modSettings.ElevationIndicatorB,
                Plugin.modSettings.ElevationIndicatorA);
            // puckElevationIndicatorMaterial.color = elevationIndicatorColor;
            puckElevationIndicatorMaterial.SetColor("_Outer_Color", elevationIndicatorColor);
            // _puckElevationIndicatorMaterialField.SetValue(puckElevationIndicator, puckElevationIndicatorMaterial);
            
            LineRenderer lineRenderer = (LineRenderer)
                _lineRendererField.GetValue(puckElevationIndicator);

            lineRenderer.material.color = lineColor;
            var solid = new Gradient();

            // Define a single color key (white here) at t=0 and t=1
            // Not sure how Color.white here plays into the line yet
            solid.colorKeys = new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            };
            // Define opaque alpha keys at t=0 and t=1
            solid.alphaKeys = new[]
            {
                new GradientAlphaKey(Plugin.modSettings.VerticalityLineStartA, 0f),
                new GradientAlphaKey(Plugin.modSettings.VerticalityLineEndA, 1f)
            };

            lineRenderer.colorGradient = solid;
            // Plugin.Log.LogInfo($"Puck verticality line material color changed");

            // these properties weirdly didn't do anything or made the line not visible
            // __instance.puckElevationIndicator.material.color = lineColor;
            // __instance.puckElevationIndicator.lineRenderer.startColor = lineColor;
            // __instance.puckElevationIndicator.lineRenderer.endColor = lineColor;
        }
    }

    // Modify the Puck trail
    [HarmonyPatch(typeof(PuckManager), nameof(PuckManager.AddPuck))]
    public static class PuckFuckerAddPuckPatch
    {
        [HarmonyPostfix]
        public static void Postfix(PuckManager __instance, Puck puck)
        {
            // Plugin.Log($"Updating trail for spawned puck");
            if (!Plugin.modSettings.PuckTrailEnabled)
            {
                // Plugin.LogError($"Puck trail not enabled");
                return; 
            }


            if (puck == null)
            {
                Plugin.LogError($"Spawned puck not found.");
                return;
            }
                

            GameObject puckRootGameObject = puck.gameObject;
            Transform trailGameObjectTransform = puckRootGameObject.transform.FindChild("Trail");
            if (trailGameObjectTransform == null)
            {
                Plugin.LogError($"Could not find trail game object transform");
                return;
            }
                

            GameObject trailGameObject = trailGameObjectTransform.gameObject;
            if (trailGameObject == null)
            {
                Plugin.LogError($"Could not find trail game object");
                return;
            }
               

            TrailRenderer trailRenderer = trailGameObject.GetComponent<TrailRenderer>();
            if (trailRenderer == null)
            {
                Plugin.LogError($"Could not find trail renderer");
                return;
            }
                

            // Enable the trail renderer
            trailRenderer.enabled = Plugin.modSettings.PuckTrailEnabled;
            trailRenderer.emitting = Plugin.modSettings.PuckTrailEnabled;

            // Set the color
            trailRenderer.material.color = new Color(Plugin.modSettings.PuckTrailColorR,
                Plugin.modSettings.PuckTrailColorG, Plugin.modSettings.PuckTrailColorB, 1f);

            trailRenderer.time = Plugin.modSettings.PuckTrailLifetimeSeconds;

            trailRenderer.startWidth = Plugin.modSettings.PuckTrailStartWidth;
            trailRenderer.endWidth = Plugin.modSettings.PuckTrailEndWidth;

            // I am not sure that this part does anything
            // trailRenderer.startColor = new Color(1, 1, 1, 1);
            // trailRenderer.endColor = new Color(1, 1, 1, 0f);
            trailRenderer.colorGradient = new Gradient
            {
                // Define a single color key (white here) at t=0 and t=1
                colorKeys = new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                // Define opaque alpha keys at t=0 and t=1
                alphaKeys = new[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 1f)
                }
            };
            // Plugin.Log($"Trail updated");
        }
    }
    
    // [HarmonyPatch(typeof(PuckManager), nameof(PuckManager.Server_SpawnPuck))]
    // public static class PuckFuckerStartPatch
    // {
    //     [HarmonyPostfix]
    //     public static void Postfix(PuckManager __instance, Vector3 position,
    //         Quaternion rotation,
    //         Vector3 velocity,
    //         Puck __result,
    //         bool isReplay = false
    //         )
    //     {
    //         
    //     }
    // }

    // public static void DumpAllShaderProperties(Shader shader)
    // {
    //     if (shader == null)
    //     {
    //         Debug.LogError("Shader is null!");
    //         return;
    //     }
    //
    //     int count = shader.GetPropertyCount();
    //     Debug.Log($"Shader '{shader.name}' has {count} properties:");
    //
    //     for (int i = 0; i < count; i++)
    //     {
    //         // basic info
    //         string name = shader.GetPropertyName(i);
    //         string desc = shader.GetPropertyDescription(i);
    //         var type = shader.GetPropertyType(i);
    //         var flags = shader.GetPropertyFlags(i);
    //
    //         // default values vary by type
    //         float defFloat = 0;
    //         Vector4 defVector = Vector4.zero;
    //         Color defColor = Color.black;
    //         string defTexture = null;
    //
    //         switch (type)
    //         {
    //             case UnityEngine.Rendering.ShaderPropertyType.Float:
    //             case UnityEngine.Rendering.ShaderPropertyType.Range:
    //                 defFloat = shader.GetPropertyDefaultFloatValue(i);
    //                 break;
    //             case UnityEngine.Rendering.ShaderPropertyType.Vector:
    //                 defVector = shader.GetPropertyDefaultVectorValue(i);
    //                 break;
    //             case UnityEngine.Rendering.ShaderPropertyType.Color:
    //                 defColor = shader.GetPropertyDefaultVectorValue(i);
    //                 break;
    //             case UnityEngine.Rendering.ShaderPropertyType.Texture:
    //                 defTexture = shader.GetPropertyTextureDefaultName(i);
    //                 break;
    //         }
    //
    //         // any attributes, like [HDR], [MainTex], etc.
    //         var attrs = shader.GetPropertyAttributes(i);
    //
    //         // log
    //         Plugin.Log(String.Format(
    //             "  [{0}] '{1}' ({2}) desc='{3}' flags={4}\n" +
    //             "       defaultFloat={5} defaultVector={6} defaultColor={7} defaultTex='{8}'\n" +
    //             "       attributes=[{9}]",
    //             i, name, type, desc, flags,
    //             defFloat, defVector, defColor, defTexture,
    //             string.Join(",", attrs)
    //         ));
    //     }
    // }
}