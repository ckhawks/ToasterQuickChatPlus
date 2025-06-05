using System.IO;
using System.Text.Json;

namespace ToasterPuckFX;

public class ModSettings
{
    public float VerticalityLineR { get; set; } = 0f;
    public float VerticalityLineG { get; set; } = 0f;
    public float VerticalityLineB { get; set; } = 0f;
    public float VerticalityLineA { get; set; } = 0.8f;
    public float VerticalityLineStartA { get; set; } = 0.5f;
    public float VerticalityLineEndA { get; set; } = 1f;

    public float ElevationIndicatorR { get; set; } = 0f;
    public float ElevationIndicatorG { get; set; } = 0f;
    public float ElevationIndicatorB { get; set; } = 0f;
    public float ElevationIndicatorA { get; set; } = 1f;    
    
    public float PuckOutlineR { get; set; } = 1f;
    public float PuckOutlineG { get; set; } = 1f;
    public float PuckOutlineB { get; set; } = 1f;
    public int PuckOutlineKernelSize { get; set; } = 1;

    public bool PuckTrailEnabled { get; set; } = false;
    public float PuckTrailStartWidth { get; set; } = 0.1f;
    public float PuckTrailEndWidth { get; set; } = 0f;
    public float PuckTrailLifetimeSeconds { get; set; } = 0.6f;
    public float PuckTrailColorR { get; set; } = 0f;
    public float PuckTrailColorG { get; set; } = 0f;
    public float PuckTrailColorB { get; set; } = 0f;

    static string ConfigurationFileName = $"{Plugin.MOD_NAME}.json";

    public static ModSettings Load()
    {
        var path = GetConfigPath();
        var dir = Path.GetDirectoryName(path);

        // 1) make sure "config/" exists
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            Plugin.Log($"Created missing /config directory");
        }
        
        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                var settings = JsonSerializer.Deserialize<ModSettings>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                return settings ?? new ModSettings();
            }
            catch (JsonException je)
            {
                Plugin.Log($"Corrupt config JSON, using defaults: {je.Message}");
                return new ModSettings();
            }
        }
        
        var defaults = new ModSettings();
        File.WriteAllText(path,
            JsonSerializer.Serialize(defaults, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
                
        Plugin.Log($"Config file `{path}` did not exist, created with defaults.");
        return defaults;
    }

    public void Save()
    {
        var path = GetConfigPath();
        var dir  = Path.GetDirectoryName(path);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path,
            JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
    }

    public static string GetConfigPath()
    {
        string rootPath = Path.GetFullPath(".");
        string configPath = Path.Combine(rootPath, "config", ConfigurationFileName);
        return configPath;
    }
}