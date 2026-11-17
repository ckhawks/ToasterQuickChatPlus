using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ToasterQuickChatPlus.ui;

public static class MainMenuQuickChatSettings
{
    public static VisualElement rootContainer;
    public static VisualElement mainContainer;
    static VisualElement rowsContainer;

    static readonly List<string> visibilityOptions = new List<string> { "ALL", "TEAM" };

    // What a dropdown choice stands for: a puckstats emote id, or a quick chat library id.
    struct Choice
    {
        public string emoteId;
        public int quickChatId;
    }

    // For each config entry (keys 1-0, 4 chat options per key -> 40 entries).
    static void BuildRows()
    {
        if (rowsContainer == null) return;
        rowsContainer.Clear();

        string emoteNote;
        switch (TrlEmotes.Current)
        {
            case TrlEmotes.State.Ready:
                emoteNote = "Your emotes from Toaster's Reskin Loader are at the top of each list. Set them up at puckstats.io/emotes.";
                break;
            case TrlEmotes.State.Outdated:
                emoteNote = "Update Toaster's Reskin Loader to put your emotes in quick chat.";
                break;
            default:
                emoteNote = "Also supports emotes: install Toaster's Reskin Loader to bind your puckstats.io emotes to quick chat slots.";
                break;
        }
        Label hint = new Label(emoteNote);
        hint.style.fontSize = 16;
        hint.style.color = new Color(0.8f, 0.8f, 0.8f);
        hint.style.marginTop = 8;
        hint.style.whiteSpace = WhiteSpace.Normal;
        rowsContainer.Add(hint);

        // Your emotes first, shown as image + :name:, then the quick chat library.
        var myEmotes = new List<(string id, string label)>();
        foreach (string id in TrlEmotes.MyEmoteIds())
        {
            string name = TrlEmotes.NameFor(id);
            if (name == null) continue;
            string image = TrlEmotes.ImageTagFor(id);
            myEmotes.Add((id, image == null ? $":{name}:" : $"{image} :{name}:"));
        }
        List<QuickChatEntry> library = QuickChats.GetQuickChatsAlphabetical();

        for (int i = 0; i < 40; i++)
        {
            rowsContainer.Add(CreateQuickChatRow(i, myEmotes, library));
        }
    }
    
    public static void Show()
    {
        if (rootContainer == null) Create();
        rootContainer.visible = true;
        rootContainer.enabledSelf = true;
        rootContainer.style.display = DisplayStyle.Flex;
        mainContainer.visible = true;
        mainContainer.enabledSelf = true;
        mainContainer.style.display = DisplayStyle.Flex;
        // MainMenu.mainMenu.Hide();
        // MonoBehaviourSingleton<UIManager>.Instance.ShowMouse();
    }

    public static void Hide()
    {
        if (rootContainer == null) Create();
        mainContainer.visible = false;
        mainContainer.enabledSelf = false;
        mainContainer.style.display = DisplayStyle.None;
        rootContainer.visible = false;
        rootContainer.enabledSelf = false;
        rootContainer.style.display = DisplayStyle.None;
        // MainMenu.mainMenu.Show();
    }

    public static void Create()
    {
        VisualElement root = new VisualElement(); // used to position the menu on the screen
        root.style.position = Position.Absolute;
        root.style.left = 0;
        root.style.top = 0;
        root.style.right = 0;
        root.style.bottom = 0;
        root.style.flexDirection = FlexDirection.Row;
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
            // Application.OpenURL("https://discord.gg/mCmX5dwzsj");
        }
        
        ScrollView quickChatsScrollView = new ScrollView();
        Label quickChatsSectionTitle = new Label("Quick chat assignment");
        quickChatsSectionTitle.style.fontSize = 20;
        quickChatsSectionTitle.style.color = Color.white;
        quickChatsSectionTitle.style.marginTop = 20;
        Label controlsSectionTitle = new Label("Controls");
        controlsSectionTitle.style.fontSize = 20;
        controlsSectionTitle.style.color = Color.white;
        
        
        // quickChatsScrollView.Add(controlsSectionTitle);
        // quickChatsScrollView.Add(quickChatsSectionTitle);
        
        rowsContainer = new VisualElement();
        quickChatsScrollView.Add(rowsContainer);
        BuildRows();
        // Emote names and images arrive (and change) after the menu is built.
        TrlEmotes.Changed -= BuildRows;
        TrlEmotes.Changed += BuildRows;
        contentContainer.Add(quickChatsScrollView);
        bottomRow.Add(closeButton);
        contentContainer.Add(bottomRow);

        Label suggestionsNote =
            new Label("Have any suggestions?  Join Toaster's Rink - Puck Modding Discord https://discord.gg/mCmX5dwzsj");
        suggestionsNote.style.fontSize = 22;
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
    public static VisualElement CreateQuickChatRow(int slotIndex, List<(string id, string label)> myEmotes, List<QuickChatEntry> library)
    {
        var settings = Plugin.modSettings.quickChatSettings[slotIndex];
        int selectedVisibility = settings.visibility;
        var choices = new List<string>();
        var choiceMap = new Dictionary<string, Choice>();
        string selected = null;
        // An emote that left your set (or with TRL gone) stays put but shows as missing.
        if (!string.IsNullOrEmpty(settings.emote) && !myEmotes.Exists(e => e.id == settings.emote))
        {
            choices.Add(QuickChatPatch.MISSING_EMOTE);
            choiceMap[QuickChatPatch.MISSING_EMOTE] = new Choice { emoteId = settings.emote };
            selected = QuickChatPatch.MISSING_EMOTE;
        }
        foreach (var emote in myEmotes)
        {
            choices.Add(emote.label);
            choiceMap[emote.label] = new Choice { emoteId = emote.id };
            if (emote.id == settings.emote) selected = emote.label;
        }
        foreach (QuickChatEntry entry in library)
        {
            choices.Add(entry.quickchat);
            choiceMap[entry.quickchat] = new Choice { quickChatId = entry.id };
            if (selected == null && string.IsNullOrEmpty(settings.emote) && entry.id == settings.index) selected = entry.quickchat;
        }

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
        quickChatDropdownField.choices = choices;
        quickChatDropdownField.index = selected == null ? -1 : choices.IndexOf(selected);
        quickChatDropdownField.style.minWidth = 500;
        quickChatDropdownField.style.maxWidth = 500;
        quickChatDropdownField.style.width = 500;
        quickChatDropdownField.style.marginRight = 10;
        quickChatDropdownField.style.overflow = Overflow.Hidden;
        quickChatDropdownField.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
        quickChatDropdownField.style.color = Color.white;
        StyleDropdownArrow(quickChatDropdownField);
        quickChatDropdownField.RegisterCallback<ChangeEvent<string>>(
            new EventCallback<ChangeEvent<string>>(evt =>
            {
                // Plugin.Log.LogInfo($"Changed on slot index {slotIndex} from {evt.previousValue} to {evt.newValue}");
                if (!choiceMap.TryGetValue(evt.newValue, out Choice choice)) return;
                if (choice.emoteId != null)
                {
                    Plugin.modSettings.quickChatSettings[slotIndex].emote = choice.emoteId;
                }
                else
                {
                    Plugin.modSettings.quickChatSettings[slotIndex].emote = null;
                    Plugin.modSettings.quickChatSettings[slotIndex].index = choice.quickChatId;
                }
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
        visibilityDropdownField.style.backgroundColor = new StyleColor(new Color(0.15f, 0.15f, 0.15f));
        visibilityDropdownField.style.color = Color.white;
        StyleDropdownArrow(visibilityDropdownField);
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

    // Make it so that if Quick Chat settings menu is open, pressing Escape closes it
    [HarmonyPatch(typeof(UIManager), "OnPauseActionPerformed")]
    private static class UIManagerOnPauseActionPerformedPatch
    {
        [HarmonyPrefix]
        static bool Prefix(UIManager __instance, InputAction.CallbackContext context)
        {
            if (rootContainer == null) return true;

            if (rootContainer.visible || rootContainer.enabledSelf || rootContainer.style.display == DisplayStyle.Flex)
            {
                Hide();
                return false;
            }

            return true;
        }
    }

    static void StyleDropdownArrow(DropdownField dropdown)
    {
        dropdown.RegisterCallback<AttachToPanelEvent>(evt =>
        {
            var input = dropdown.Q(className: "unity-base-popup-field__input");
            if (input == null) return;

            input.style.flexDirection = FlexDirection.Row;
            input.style.justifyContent = Justify.SpaceBetween;
            input.style.alignItems = Align.Center;

            Label arrow = new Label("▼");
            arrow.style.color = Color.white;
            arrow.style.fontSize = 18;
            arrow.style.marginLeft = 4;
            arrow.style.marginRight = 8;
            arrow.style.unityTextAlign = TextAnchor.MiddleCenter;
            arrow.style.paddingBottom = 0;
            arrow.style.paddingTop = 0;
            arrow.pickingMode = PickingMode.Ignore; // clicks pass through to the dropdown
            input.Add(arrow);
        });

    }
}