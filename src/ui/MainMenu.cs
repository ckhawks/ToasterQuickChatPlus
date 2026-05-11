using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToasterQuickChatPlus.ui;

public static class MainMenu
{
    static readonly FieldInfo _playerButtonField = typeof(UIMainMenu)
        .GetField("playerButton",
            BindingFlags.Instance | BindingFlags.NonPublic);

    static readonly FieldInfo _exitGameButtonField = typeof(UIMainMenu)
        .GetField("exitGameButton",
            BindingFlags.Instance | BindingFlags.NonPublic);

    public static VisualElement uiMainMenu;
    public static UIMainMenu mainMenu;

    public static void Setup()
    {
        Plugin.Log($"Setting up MainMenu UI");
        UIMainMenu uiMainMenuInstance = MonoBehaviourSingleton<UIManager>.Instance.MainMenu;

        mainMenu = uiMainMenuInstance;
        uiMainMenu = MonoBehaviourSingleton<UIManager>.Instance.RootVisualElement;
        CreateMainMenuQuickChatsButton(uiMainMenuInstance);
    }

    public static void CreateMainMenuQuickChatsButton(UIMainMenu __instance)
    {
        Button playerButton = (Button)_playerButtonField.GetValue(__instance);
        Button exitGameButton = (Button)_exitGameButtonField.GetValue(__instance);

        // Find the container that holds the menu buttons
        VisualElement containerVisualElement = playerButton.parent;

        if (containerVisualElement == null)
        {
            Plugin.LogError("Container VisualElement not found (parent of playerButton missing)!");
            return;
        }

        Button button = new Button();
        button.text = "QUICK CHAT PLUS";

        // Copy USS classes from an existing button so we match the stylesheet
        foreach (string cls in playerButton.GetClasses())
        {
            button.AddToClassList(cls);
        }

        button.RegisterCallback<MouseEnterEvent>(new EventCallback<MouseEnterEvent>((evt) =>
        {
            button.style.backgroundColor = Color.white;
            button.style.color = Color.black;
        }));
        button.RegisterCallback<MouseLeaveEvent>(new EventCallback<MouseLeaveEvent>((evt) =>
        {
            button.style.backgroundColor = StyleKeyword.Null;
            button.style.color = StyleKeyword.Null;
        }));
        button.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(MainMenuQuickChatPlusClickHandler));
        static void MainMenuQuickChatPlusClickHandler(ClickEvent evt)
        {
            MainMenuQuickChatSettings.Show();
        }

        // Insert just before the Exit Game button
        int exitIndex = containerVisualElement.IndexOf(exitGameButton);
        if (exitIndex >= 0)
        {
            containerVisualElement.Insert(exitIndex, button);
        }
        else
        {
            containerVisualElement.Add(button);
        }
    }
}
