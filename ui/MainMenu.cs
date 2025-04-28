using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToasterQuickChatPlus.ui;

public static class MainMenu
{
    public static VisualElement uiMainMenu;
    public static UIMainMenu mainMenu;
    
    [HarmonyPatch(typeof(UIMainMenu), nameof(UIMainMenu.Start))]
    public static class UIMainMenuStart
    {
        [HarmonyPostfix]
        public static void Postfix(UIMainMenu __instance)
        {
            Plugin.Log.LogInfo($"UIMainMenuStart (Postfix)");
            mainMenu = __instance;
            uiMainMenu = __instance.playerButton.parent.parent; // might need to add one more .parent to this
            CreateMainMenuQuickChatsButton(__instance);
        }
    }

    public static void CreateMainMenuQuickChatsButton(UIMainMenu __instance)
    {
        VisualElement containerVisualElement = __instance.settingsButton.parent;
        // containerVisualElement.style.height = new StyleLength(new Length(1000, LengthUnit.Pixel));
        
        if (containerVisualElement == null)
        {
            Plugin.Log.LogError("Container VisualElement not found (parent of playerButton missing)!");
            return;
        }

        Button button = new Button();
        button.text = "QUICK CHAT PLUS";
        button.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
        button.style.unityTextAlign = TextAnchor.MiddleLeft;
        button.style.width = __instance.playerButton.style.width;
        button.style.minWidth = __instance.playerButton.style.minWidth;
        button.style.maxWidth = __instance.playerButton.style.maxWidth;
        button.style.height = __instance.playerButton.style.height;
        button.style.minHeight = __instance.playerButton.style.minHeight;
        button.style.maxHeight = __instance.playerButton.style.maxHeight;
        button.style.marginTop = 8;
        button.style.paddingTop = 8;
        button.style.paddingBottom = 8;
        button.style.paddingLeft = 15;
        button.RegisterCallback<MouseEnterEvent>(new Action<MouseEnterEvent>((evt) =>
        {
            button.style.backgroundColor = Color.white;
            button.style.color = Color.black;
        }));
        button.RegisterCallback<MouseLeaveEvent>(new Action<MouseLeaveEvent>((evt) =>
        {
            button.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
            button.style.color = Color.white;
        }));
        button.RegisterCallback<ClickEvent>(new Action<ClickEvent>(MainMenuQuickChatPlusClickHandler));
        static void MainMenuQuickChatPlusClickHandler(ClickEvent evt)
        {
            // Plugin.Log.LogInfo("QCP Button Clicked!");
            MainMenuQuickChatSettings.Show();
            // Application.OpenURL("http://discord.puckstats.io");
        }
        
        containerVisualElement.Insert(4, button);
        // containerVisualElement.Add(button);
    }
}