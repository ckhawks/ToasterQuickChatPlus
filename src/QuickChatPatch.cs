using System;
using System.Collections.Generic;
using System.Reflection;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using HarmonyLib;
using UnityEngine.UIElements;

namespace ToasterQuickChatPlus;

public static class QuickChatPatch
{
    static readonly FieldInfo _isQuickChatEnabledField = typeof(ChatManager)
        .GetField("isQuickChatEnabled",
            BindingFlags.Instance | BindingFlags.NonPublic);

    static readonly FieldInfo _quickChatCategoryField = typeof(ChatManager)
        .GetField("quickChatCategory",
            BindingFlags.Instance | BindingFlags.NonPublic);

    static readonly FieldInfo _quickChatTimeoutTweenField = typeof(ChatManager)
        .GetField("quickChatTimeoutTween",
            BindingFlags.Instance | BindingFlags.NonPublic);

    static readonly FieldInfo _quickChatsField = typeof(ChatManager)
        .GetField("quickChats",
            BindingFlags.Instance | BindingFlags.NonPublic);

    static readonly FieldInfo _quickChatCategoryLabelField = typeof(UIChat)
        .GetField("quickChatCategoryLabel",
            BindingFlags.Instance | BindingFlags.NonPublic);

    // Store the mod's quickchat data per menu index (0-9)
    // Each entry is an array of QuickChat structs
    static readonly Dictionary<int, QuickChat[]> modQuickChats = new Dictionary<int, QuickChat[]>();

    // Track the currently open menu index (mod-level, since we may use indices beyond the enum)
    static int currentMenuIndex = -1;

    // Per slot of the open menu: false for an emote slot whose emote is gone, which sends nothing.
    static bool[] currentSendable = new bool[0];

    public const string MISSING_EMOTE = "(missing emote)";

    [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.Client_QuickChatAction))]
    public static class ChatManagerQuickChatActionPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ChatManager __instance, int index)
        {
            bool isEnabled = (bool)_isQuickChatEnabledField.GetValue(__instance);

            if (isEnabled)
            {
                // Second press — select a message from the current menu
                if (currentMenuIndex < 0 || !modQuickChats.ContainsKey(currentMenuIndex))
                {
                    SetQuickChatDisabled(__instance);
                    return false;
                }

                QuickChat[] messages = modQuickChats[currentMenuIndex];
                if (index < 0 || index >= messages.Length)
                {
                    return false;
                }
                if (index >= currentSendable.Length || !currentSendable[index])
                {
                    return false;
                }

                QuickChat quickChat = messages[index];
                SetQuickChatDisabled(__instance);
                __instance.Client_SendChatMessage(quickChat.Content, true, quickChat.IsTeamChat);
                return false;
            }
            else
            {
                // First press — open a category menu
                if (index < 0 || index > 9 || !modQuickChats.ContainsKey(index))
                {
                    return false;
                }

                currentMenuIndex = index;
                // Rebuilt on every open so emote slots pick up renames and set changes.
                modQuickChats[index] = BuildMenu(index, out currentSendable);
                _isQuickChatEnabledField.SetValue(__instance, true);

                // Use a valid enum value for the field so the game doesn't break
                QuickChatCategory category = (QuickChatCategory)(index <= 4 ? index : 0);
                _quickChatCategoryField.SetValue(__instance, (QuickChatCategory?)category);

                // Fire the UI event so UIChat shows the messages
                EventManager.TriggerEvent("Event_OnQuickChatEnabled", new Dictionary<string, object>
                {
                    { "category", category },
                    { "quickChats", modQuickChats[index] }
                });

                // Start 5-second timeout
                Tween existingTween = (Tween)_quickChatTimeoutTweenField.GetValue(__instance);
                if (existingTween != null)
                {
                    existingTween.Kill(false);
                }
                Tween timeoutTween = DOVirtual.DelayedCall(5f, () =>
                {
                    SetQuickChatDisabled(__instance);
                }, true);
                _quickChatTimeoutTweenField.SetValue(__instance, timeoutTween);

                return false;
            }
        }
    }

    static void SetQuickChatDisabled(ChatManager chatManager)
    {
        _isQuickChatEnabledField.SetValue(chatManager, false);
        _quickChatCategoryField.SetValue(chatManager, null);
        currentMenuIndex = -1;
        currentSendable = new bool[0];

        Tween existingTween = (Tween)_quickChatTimeoutTweenField.GetValue(chatManager);
        if (existingTween != null)
        {
            existingTween.Kill(false);
        }

        EventManager.TriggerEvent("Event_OnQuickChatDisabled", null);
    }

    public static bool IsQuickChatEnabled()
    {
        var chatManager = NetworkBehaviourSingleton<ChatManager>.Instance;
        if (chatManager == null) return false;
        return (bool)_isQuickChatEnabledField.GetValue(chatManager);
    }

    public static void UpdateMessagesInUIChat()
    {
        modQuickChats.Clear();

        for (int menuIndex = 0; menuIndex < 10; menuIndex++)
        {
            modQuickChats[menuIndex] = BuildMenu(menuIndex, out _);
        }

        // Also update the ChatManager's quickChats dictionary for vanilla categories (0-4)
        var chatManager = NetworkBehaviourSingleton<ChatManager>.Instance;
        if (chatManager != null)
        {
            var quickChats = new SerializedDictionary<QuickChatCategory, QuickChat[]>();
            for (int i = 0; i < 5; i++)
            {
                if (modQuickChats.ContainsKey(i))
                {
                    quickChats[(QuickChatCategory)i] = modQuickChats[i];
                }
            }
            _quickChatsField.SetValue(chatManager, quickChats);
        }
    }

    const int SLOTS_PER_MENU = 4;

    // The menu's messages from settings. An emote slot sends ":name:" with the emote's current
    // name; when the emote left the player's set (or TRL is missing) it shows MISSING_EMOTE and
    // is not sendable.
    static QuickChat[] BuildMenu(int menuIndex, out bool[] sendable)
    {
        QuickChat[] messages = new QuickChat[SLOTS_PER_MENU];
        sendable = new bool[SLOTS_PER_MENU];
        for (int slot = 0; slot < SLOTS_PER_MENU; slot++)
        {
            var settings = Plugin.modSettings.quickChatSettings[menuIndex * SLOTS_PER_MENU + slot];
            string content;
            if (string.IsNullOrEmpty(settings.emote))
            {
                content = QuickChats.GetQuickChatByID(settings.index).quickchat;
                sendable[slot] = true;
            }
            else
            {
                string name = TrlEmotes.NameFor(settings.emote);
                content = name == null ? MISSING_EMOTE : ":" + name + ":";
                sendable[slot] = name != null;
            }
            messages[slot] = new QuickChat
            {
                Content = content,
                IsTeamChat = settings.visibility == 1
            };
        }
        return messages;
    }

    [HarmonyPatch(typeof(UIChat), nameof(UIChat.ShowQuickChat))]
    public static class UIChatShowQuickChatPatch
    {
        [HarmonyPostfix]
        public static void Postfix(UIChat __instance)
        {
            if (currentMenuIndex >= 5)
            {
                var label = (UnityEngine.UIElements.Label)_quickChatCategoryLabelField.GetValue(__instance);
                if (label != null)
                {
                    label.text = "CUSTOM";
                }
            }
        }
    }

    public static void Setup()
    {
        UpdateMessagesInUIChat();
    }
}
