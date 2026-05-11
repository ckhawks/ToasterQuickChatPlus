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
            int slotsPerMenu = 4;
            QuickChat[] messages = new QuickChat[slotsPerMenu];

            for (int slot = 0; slot < slotsPerMenu; slot++)
            {
                int settingsIndex = menuIndex * slotsPerMenu + slot;
                var settings = Plugin.modSettings.quickChatSettings[settingsIndex];
                messages[slot] = new QuickChat
                {
                    Content = QuickChats.GetQuickChatByID(settings.index).quickchat,
                    IsTeamChat = settings.visibility == 1
                };
            }

            modQuickChats[menuIndex] = messages;
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
