using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToasterQuickChatPlus.ui;

public static class MainMenu
{
    static readonly FieldInfo _playerButtonField = typeof(UIMainMenu)
        .GetField("playerButton", 
            BindingFlags.Instance | BindingFlags.NonPublic);   
    
    public static VisualElement uiMainMenu;
    public static UIMainMenu mainMenu;
    
    public static void Setup()
    {
        Plugin.Log($"Setting up MainMenu UI");
        UIMainMenu uiMainMenuInstance = UIMainMenu.Instance;
        
        Button playerButton = (Button) _playerButtonField.GetValue(uiMainMenuInstance);
            
        mainMenu = uiMainMenuInstance;
        uiMainMenu = playerButton.parent.parent; // might need to add one more .parent to this
        CreateMainMenuQuickChatsButton(uiMainMenuInstance);
    }

    public static void CreateMainMenuQuickChatsButton(UIMainMenu __instance)
    {
        Button playerButton = (Button) _playerButtonField.GetValue(__instance);
        
        VisualElement containerVisualElement = playerButton.parent;
        // containerVisualElement.style.height = new StyleLength(new Length(1000, LengthUnit.Pixel));
        
        if (containerVisualElement == null)
        {
            Plugin.LogError("Container VisualElement not found (parent of playerButton missing)!");
            return;
        }

        Button button = new Button();
        button.text = "QUICK CHAT PLUS";
        button.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
        button.style.unityTextAlign = TextAnchor.MiddleLeft;
        button.style.width = playerButton.style.width;
        button.style.minWidth = playerButton.style.minWidth;
        button.style.maxWidth = playerButton.style.maxWidth;
        button.style.height = playerButton.style.height;
        button.style.minHeight = playerButton.style.minHeight;
        button.style.maxHeight = playerButton.style.maxHeight;
        button.style.marginTop = 8;
        button.style.paddingTop = 8;
        button.style.paddingBottom = 8;
        button.style.paddingLeft = 15;
        button.RegisterCallback<MouseEnterEvent>(new EventCallback<MouseEnterEvent>((evt) =>
        {
            button.style.backgroundColor = Color.white;
            button.style.color = Color.black;
        }));
        button.RegisterCallback<MouseLeaveEvent>(new EventCallback<MouseLeaveEvent>((evt) =>
        {
            button.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
            button.style.color = Color.white;
        }));
        button.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(MainMenuQuickChatPlusClickHandler));
        static void MainMenuQuickChatPlusClickHandler(ClickEvent evt)
        {
            MainMenuQuickChatSettings.Show();
        }
        
        containerVisualElement.Insert(4, button);
    }
}