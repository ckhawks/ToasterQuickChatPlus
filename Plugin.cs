using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine.InputSystem;

namespace ToasterQuickChatPlus;

[BepInPlugin("pw.stellaric.plugins.toasterquickchatplus", "Toaster Quick Chat Plus", "1.0.0.0")]
public class Plugin : BasePlugin
{
    // the "configurable" things
    private readonly Harmony _harmony = new Harmony("pw.stellaric.plugins.toasterquickchatplus");
    
    // plugin managers
    public static new ManualLogSource Log;
    
    public static InputAction quickchat5Action;
    public static InputAction quickchat6Action;
    public static InputAction quickchat7Action;
    public static InputAction quickchat8Action;
    public static InputAction quickchat9Action;
    public static InputAction quickchat0Action;
    public static InputAction quickchatCloseAction;
    
    
    // store your ConfigEntries in arrays for easy access
    public static ConfigEntry<int>[] configQuickChatIndex = new ConfigEntry<int>[40];
    public static ConfigEntry<int>[] configQuickChatVisibility = new ConfigEntry<int>[40];
    
    // TODO make it so you can customize the default controls
    public static ConfigEntry<string> configBindingQuickchat1;
    public static ConfigEntry<string> configBindingQuickchat2;
    public static ConfigEntry<string> configBindingQuickchat3;
    public static ConfigEntry<string> configBindingQuickchat4;
    public static ConfigEntry<string> configBindingQuickchat5;
    public static ConfigEntry<string> configBindingQuickchat6;
    public static ConfigEntry<string> configBindingQuickchat7;
    public static ConfigEntry<string> configBindingQuickchat8;
    public static ConfigEntry<string> configBindingQuickchat9;
    public static ConfigEntry<string> configBindingQuickchat0;
    public static ConfigEntry<string> configBindingQuickchatEscape;
    
    public override void Load()
    {
        HarmonyFileLog.Enabled = true;
        
        ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "ToasterQuickChatPlus.cfg"), true);
        
        configBindingQuickchat1 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat1", 
            "<keyboard>/1", 
            new ConfigDescription("Menu 1 (and item 1)"));
        configBindingQuickchat2 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat2", 
            "<keyboard>/2", 
            new ConfigDescription("Menu 2 (and item 2)"));
        configBindingQuickchat3 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat3", 
            "<keyboard>/3", 
            new ConfigDescription("Menu 3 (and item 3)"));
        configBindingQuickchat4 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat4", 
            "<keyboard>/4", 
            new ConfigDescription("Menu 4 (and item 4)"));
        configBindingQuickchat5 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat5", 
            "<keyboard>/5", 
            new ConfigDescription("Menu 5"));
        configBindingQuickchat6 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat6", 
            "<keyboard>/6", 
            new ConfigDescription("Menu 6"));
        configBindingQuickchat7 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat7", 
            "<keyboard>/7", 
            new ConfigDescription("Menu 7"));
        configBindingQuickchat8 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat8", 
            "<keyboard>/8", 
            new ConfigDescription("Menu 8"));
        configBindingQuickchat9 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat9", 
            "<keyboard>/9", 
            new ConfigDescription("Menu 9"));
        configBindingQuickchat0 = configFile.Bind<string>(
            "Controls",
            "bindingQuickchat0", 
            "<keyboard>/0", 
            new ConfigDescription("Menu 0"));
        configBindingQuickchatEscape = configFile.Bind<string>(
            "Controls",
            "bindingQuickchatEscape", 
            "<keyboard>/escape", 
            new ConfigDescription("Escape"));
        
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
        
        for (int i = 0; i < 40; i++)
        {
            string section = "QuickChatData";
            string nameIdx  = $"QuickChat_{i:00}_Message";
            string nameTeam = $"QuickChat_{i:00}_Visibility";

            configQuickChatIndex[i] = configFile.Bind(
                section,
                nameIdx,
                slotDefaults[i].idx,  // default index
                $"Slot {i:00} quickchat index (0-N)");
            configQuickChatVisibility[i] = configFile.Bind(
                section,
                nameTeam,
                slotDefaults[i].visibility,  // default: not team
                $"Slot {i:00} visibility (0: all, 1: team)");
        }
        
        // register keybinds
        quickchat5Action = new InputAction(binding: configBindingQuickchat5.Value);
        quickchat5Action.Enable();
        quickchat6Action = new InputAction(binding: configBindingQuickchat6.Value);
        quickchat6Action.Enable();
        quickchat7Action = new InputAction(binding: configBindingQuickchat7.Value);
        quickchat7Action.Enable();
        quickchat8Action = new InputAction(binding: configBindingQuickchat8.Value);
        quickchat8Action.Enable();
        quickchat9Action = new InputAction(binding: configBindingQuickchat9.Value);
        quickchat9Action.Enable();
        quickchat0Action = new InputAction(binding: configBindingQuickchat0.Value);
        quickchat0Action.Enable();
        quickchatCloseAction = new InputAction(binding: configBindingQuickchatEscape.Value);
        quickchatCloseAction.Enable();
        
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded! Patching methods...");
        _harmony.PatchAll();
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is all patched! Patched methods:");
        
        var originalMethods = Harmony.GetAllPatchedMethods();
        foreach (var method in originalMethods)
        {
            Log.LogInfo($" - {method.DeclaringType.FullName}.{method.Name}");
        }
    }
}