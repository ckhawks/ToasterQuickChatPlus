using System.IO;
using System.Text.Json;

namespace ToasterQuickChatPlus;

public class ModSettings
{
    public string BindingQuickchat5 { get; set; } = "<keyboard>/5";
    public string BindingQuickchat6 { get; set; } = "<keyboard>/6";
    public string BindingQuickchat7 { get; set; } = "<keyboard>/7";
    public string BindingQuickchat8 { get; set; } = "<keyboard>/8";
    public string BindingQuickchat9 { get; set; } = "<keyboard>/9";
    public string BindingQuickchat0 { get; set; } = "<keyboard>/0";
    public string BindingQuickchatEscape { get; set; } = "<keyboard>/escape";

    public QuickChatSettings[] quickChatSettings { get; set; }

    public class QuickChatSettings
    {
        public int index { get; set; } = 0;
        public int visibility { get; set; } = 0;
    }

    static string ConfigurationFileName = $"{Plugin.MOD_NAME}.json";

    public ModSettings()
    {
        (int idx, int visibility)[] slotDefaults = new (int, int)[]
        {
            // Menu 1
            (0, 1),  // slot 00 → chat 0 | all-chat 0 / team 1
            (1, 1),
            (2, 1),
            (3, 1), // #4
            // Menu 2
            (4, 0),
            (5, 0),
            (6, 0), 
            (7, 0),
            // Menu 3
            (8, 0),
            (9, 0),
            (10, 0),
            (11, 0),
            // Menu 4
            (12, 0),
            (13, 0),
            (14, 0),
            (15, 0),
            // end of defaults
            // Menu 5 - end of game
            (62, 0),
            (64, 0),
            (66, 0),
            (65, 0),
            // Menu 6
            (16, 0),
            (19, 0),
            (21, 0),
            (22, 0),
            // Menu 7
            (58, 0),
            (61, 0),
            (30, 0),
            (28, 0),
            // Menu 8
            (40, 1),
            (41, 1),
            (46, 1),
            (45, 0),
            // Menu 9
            (51, 0),
            (37, 0),
            (35, 0),
            (34, 0),
            // Menu 0 - silly
            (57, 0),
            (56, 0),
            (17, 0),
            (18, 0),
        };
        
        quickChatSettings = new QuickChatSettings[slotDefaults.Length];
        for (int i = 0; i < slotDefaults.Length; i++)
        {
            quickChatSettings[i] = new QuickChatSettings
            {
                index = slotDefaults[i].idx,
                visibility = slotDefaults[i].visibility
            };
        }
    }
    
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
                        PropertyNameCaseInsensitive = true,
                        IncludeFields               = true
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
                WriteIndented = true,
                IncludeFields               = true
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
                WriteIndented = true,
                IncludeFields               = true
            }));
        Plugin.Log($"Saved config file to {path}");
    }

    public static string GetConfigPath()
    {
        string rootPath = Path.GetFullPath(".");
        string configPath = Path.Combine(rootPath, "config", ConfigurationFileName);
        return configPath;
    }
}