using HarmonyLib;
using UnityEngine.InputSystem;

namespace ToasterQuickChatPlus;

public static class PatchPlayerInput
{
    [HarmonyPatch(typeof(PlayerInput), "Update")]
    class PatchPlayerInputUpdate
    {
        [HarmonyPostfix]
        static void Postfix(PlayerInput __instance)
        {
            UIChat chat = MonoBehaviourSingleton<UIManager>.Instance.Chat;
            if (chat.IsFocused) return;

            if (Plugin.quickchat6Action.WasPressedThisFrame())
            {
                Plugin.Log($"Pressed 6");
                NetworkBehaviourSingleton<ChatManager>.Instance.Client_QuickChatAction(5);
            }

            if (Plugin.quickchat7Action.WasPressedThisFrame())
            {
                NetworkBehaviourSingleton<ChatManager>.Instance.Client_QuickChatAction(6);
            }

            if (Plugin.quickchat8Action.WasPressedThisFrame())
            {
                NetworkBehaviourSingleton<ChatManager>.Instance.Client_QuickChatAction(7);
            }

            if (Plugin.quickchat9Action.WasPressedThisFrame())
            {
                NetworkBehaviourSingleton<ChatManager>.Instance.Client_QuickChatAction(8);
            }

            if (Plugin.quickchat0Action.WasPressedThisFrame())
            {
                NetworkBehaviourSingleton<ChatManager>.Instance.Client_QuickChatAction(9);
            }

            if (Plugin.quickchatCloseAction.WasPressedThisFrame() && QuickChatPatch.IsQuickChatEnabled())
            {
                Plugin.Log($"Pressed close");
                NetworkBehaviourSingleton<ChatManager>.Instance.SetQuickChatEnabled(false, null);
            }
        }
    }

    // Suppress pause menu from opening when ESC is used to close the quickchat menu
    [HarmonyPatch(typeof(UIManager), "OnPauseActionPerformed")]
    class PatchUIManagerPause
    {
        [HarmonyPrefix]
        static bool Prefix(InputAction.CallbackContext context)
        {
            if (QuickChatPatch.IsQuickChatEnabled())
            {
                // Quickchat is open — close it and block the pause menu
                NetworkBehaviourSingleton<ChatManager>.Instance.SetQuickChatEnabled(false, null);
                return false;
            }
            return true;
        }
    }
}
