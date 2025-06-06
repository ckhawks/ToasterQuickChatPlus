using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ToasterQuickChatPlus.ui;

public static class MainMenuQuickChatSettings
{
    public static VisualElement rootContainer;
    public static VisualElement mainContainer;
    
    public static void Show()
    {
        if (rootContainer == null) Create();
        rootContainer.visible = true;
        rootContainer.enabledSelf = true;
        mainContainer.visible = true;
        mainContainer.enabledSelf = true;
        // MainMenu.mainMenu.Hide();
        // MonoBehaviourSingleton<UIManager>.Instance.ShowMouse();
    }

    public static void Hide()
    {
        if (rootContainer == null) Create();
        mainContainer.visible = false;
        mainContainer.enabledSelf = false;
        rootContainer.visible = false;
        rootContainer.enabledSelf = false;
        // MainMenu.mainMenu.Show();
    }

    public static void Create()
    {
        VisualElement root = new VisualElement(); // used to position the menu on the screen
        root.style.flexDirection = FlexDirection.Row;
        root.style.height = new StyleLength(Length.Percent(100));
        root.style.width = new StyleLength(Length.Percent(100));
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;
        VisualElement overallContainer = new VisualElement();
        overallContainer.style.backgroundColor = new StyleColor(new Color(0.196f, 0.196f,0.196f, 1));
        overallContainer.style.paddingLeft = new StyleLength(new Length(10));
        overallContainer.style.paddingRight = new StyleLength(new Length(10));
        overallContainer.style.paddingTop = new StyleLength(new Length(10));
        overallContainer.style.paddingBottom = new StyleLength(new Length(10));
        overallContainer.style.maxWidth = new StyleLength(new Length(45, LengthUnit.Percent));
        overallContainer.style.minWidth = new StyleLength(new Length(45, LengthUnit.Percent));
        overallContainer.style.maxHeight = new StyleLength(new Length(75, LengthUnit.Percent));
        overallContainer.style.minHeight = new StyleLength(new Length(75, LengthUnit.Percent));

        VisualElement titleContainer = new VisualElement();
        titleContainer.style.flexDirection = FlexDirection.Row;
        // titleContainer.style.flexGrow = 10f;
        titleContainer.style.justifyContent = Justify.SpaceBetween;
        titleContainer.style.alignItems = Align.Center;
        titleContainer.style.minHeight = 50;
        Label title = new Label("Quick Chat Plus - Settings");
        title.style.fontSize = 30;
        title.style.color = Color.white;
        titleContainer.Add(title);
        Label titleSubtext = new Label($"{QuickChats.quickchats.Count} quickchats");
        titleSubtext.style.fontSize = 15;
        titleSubtext.style.color = Color.white;
        titleContainer.Add(titleSubtext);
        overallContainer.Add(titleContainer);
        
        VisualElement contentContainer = new VisualElement();
        contentContainer.style.flexDirection = FlexDirection.Column;
        overallContainer.Add(contentContainer);

        VisualElement bottomRow = new VisualElement();
        bottomRow.style.flexDirection = FlexDirection.Row;
        bottomRow.style.justifyContent = Justify.FlexEnd;
        bottomRow.style.marginTop = 8;
        bottomRow.style.minHeight = 50;
        bottomRow.style.maxHeight = 50;
        
        
        Button closeButton = new Button();
        closeButton.text = "CLOSE";
        closeButton.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
        closeButton.style.unityTextAlign = TextAnchor.MiddleLeft;
        closeButton.style.width = 250;
        closeButton.style.minWidth = 250;
        closeButton.style.maxWidth = 250;
        closeButton.style.height = 50;
        closeButton.style.minHeight = 50;
        closeButton.style.maxHeight = 50;
        closeButton.style.paddingTop = 12;
        closeButton.style.paddingBottom = 12;
        closeButton.style.paddingLeft = 16;
        closeButton.style.paddingRight = 16;
        closeButton.RegisterCallback<MouseEnterEvent>(new EventCallback<MouseEnterEvent>((evt) =>
        {
            closeButton.style.backgroundColor = Color.white;
            closeButton.style.color = Color.black;
        }));
        closeButton.RegisterCallback<MouseLeaveEvent>(new EventCallback<MouseLeaveEvent>((evt) =>
        {
            closeButton.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
            closeButton.style.color = Color.white;
        }));
        closeButton.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(QuickChatPlusSettingsCloseButtonClickHandler));
        static void QuickChatPlusSettingsCloseButtonClickHandler(ClickEvent evt)
        {
            // Plugin.Log.LogInfo("QCP Close Button Clicked!");
            // MainMenuQuickChatSettings.Show();
            Hide();
            // Application.OpenURL("http://discord.puckstats.io");
        }
        
        ScrollView quickChatsScrollView = new ScrollView();
        Label quickChatsSectionTitle = new Label("Quick chat assignment");
        quickChatsSectionTitle.style.fontSize = 20;
        quickChatsSectionTitle.style.color = Color.white;
        quickChatsSectionTitle.style.marginTop = 20;
        Label controlsSectionTitle = new Label("Controls");
        controlsSectionTitle.style.fontSize = 20;
        controlsSectionTitle.style.color = Color.white;
        
        List<string> quickChatsSortedAsStrings = QuickChats.GetQuickChatsAlphabeticalAsStrings();
        List<string> visibilityOptions = new List<string>();
        visibilityOptions.Add("ALL");
        visibilityOptions.Add("TEAM");
        
        // quickChatsScrollView.Add(controlsSectionTitle);
        // quickChatsScrollView.Add(quickChatsSectionTitle);
        
        // for each config entry (keys 1-0, 4 chat options per key -> 40 entries)
        for (int i = 0; i < 40; i++)
        {
            quickChatsScrollView.Add(CreateQuickChatRow(
                i, 
                quickChatsSortedAsStrings, 
                quickChatsSortedAsStrings.IndexOf(QuickChats.quickchats[Plugin.modSettings.quickChatSettings[i].index]),
                visibilityOptions, 
                Plugin.modSettings.quickChatSettings[i].visibility
            ));
        }
        contentContainer.Add(quickChatsScrollView);
        bottomRow.Add(closeButton);
        contentContainer.Add(bottomRow);

        Label suggestionsNote =
            new Label("Have any suggestions?  Join Toaster's Rink - Puck Modding Discord http://discord.puckstats.io/");
        suggestionsNote.style.fontSize = 14;
        suggestionsNote.style.color = Color.white;
        suggestionsNote.style.marginTop = 10;
        suggestionsNote.style.unityTextAlign = TextAnchor.MiddleCenter;
        quickChatsScrollView.Add(suggestionsNote);
        
        MainMenu.uiMainMenu.Add(root);
        root.Add(overallContainer);
        root.visible = false;
        root.enabledSelf = false;
        
        mainContainer = overallContainer;
        rootContainer = root;
    }

    // slotIndex = used to know which config slot that should be modified
    // index = used to know which name in the sorted list of messages should be selected
    // 
    public static VisualElement CreateQuickChatRow(int slotIndex, List<string> quickChatsSortedAsStrings, int selectedIndexFromSorted, List<string> visibilityOptions, int selectedVisibility)
    {
        // TODO add event handling for modifying to update the config
        VisualElement rowContainer = new VisualElement();
        rowContainer.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
        rowContainer.style.marginTop = 8;
        rowContainer.style.paddingTop = 8;
        rowContainer.style.paddingLeft = 16;
        rowContainer.style.paddingRight = 16;
        rowContainer.style.paddingBottom = 8;
        rowContainer.style.flexDirection = FlexDirection.Row;
        rowContainer.style.justifyContent = Justify.FlexStart;
        Label quickChatLabel = new Label($"{slotIndex / 4 + 1} - {(slotIndex % 4) + 1}");
        quickChatLabel.style.minWidth = 120;
        quickChatLabel.style.width = 120;
        quickChatLabel.style.maxWidth = 120;
        quickChatLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
        quickChatLabel.style.marginRight = 10;
        DropdownField quickChatDropdownField = new DropdownField();
        quickChatDropdownField.choices = quickChatsSortedAsStrings;
        quickChatDropdownField.index = selectedIndexFromSorted;
        quickChatDropdownField.style.minWidth = 500;
        quickChatDropdownField.style.maxWidth = 500;
        quickChatDropdownField.style.width = 500;
        quickChatDropdownField.style.marginRight = 10;
        quickChatDropdownField.style.overflow = Overflow.Hidden;
        quickChatDropdownField.RegisterCallback<ChangeEvent<string>>(
            new EventCallback<ChangeEvent<string>>(evt =>
            {
                // Plugin.Log.LogInfo($"Changed on slot index {slotIndex} from {evt.previousValue} to {evt.newValue}");
                Plugin.modSettings.quickChatSettings[slotIndex].index = QuickChats.GetQuickChatByName(evt.newValue).id;
                QuickChatPatch.UpdateMessagesInUIChat();
                Plugin.modSettings.Save();
            })
        );
        DropdownField visibilityDropdownField = new DropdownField();
        visibilityDropdownField.choices = visibilityOptions;
        visibilityDropdownField.index = selectedVisibility;
        visibilityDropdownField.style.minWidth = 140;
        visibilityDropdownField.style.maxWidth = 140;
        visibilityDropdownField.style.width = 140;
        visibilityDropdownField.RegisterCallback<ChangeEvent<string>>(
            new EventCallback<ChangeEvent<string>>(evt =>
            {
                // Plugin.Log.LogInfo($"Changed on slot index {slotIndex} from {evt.previousValue} to {evt.newValue}");
                if (evt.newValue == "TEAM")
                {
                    Plugin.modSettings.quickChatSettings[slotIndex].visibility = 1;
                }
                if (evt.newValue == "ALL")
                {
                    Plugin.modSettings.quickChatSettings[slotIndex].visibility = 0;
                }
                Plugin.modSettings.Save();
            })
        );
        rowContainer.Add(quickChatLabel);
        rowContainer.Add(quickChatDropdownField);
        rowContainer.Add(visibilityDropdownField);
        return rowContainer;
    }
}