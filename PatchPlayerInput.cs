using HarmonyLib;

namespace ToasterQuickChatPlus;

public static class PatchPlayerInput
{
    
    [HarmonyPatch(typeof(PlayerInput), nameof(PlayerInput.Update))]
    class PatchPlayerInputUpdate
    {
        [HarmonyPostfix]
        static void Postfix(PlayerInput __instance)
        {
            UIChat chat = NetworkBehaviourSingleton<UIChat>.Instance;
            
            // do not use the actions if chat is open
            if (chat.isFocused) return;

            if (Plugin.quickchat5Action.WasPressedThisFrame())
            {
                chat.OpenQuickChat(4);
            }
            
            if (Plugin.quickchat6Action.WasPressedThisFrame())
            {
                chat.OpenQuickChat(5);
            }
            
            if (Plugin.quickchat7Action.WasPressedThisFrame())
            {
                chat.OpenQuickChat(6);
            }
            
            if (Plugin.quickchat8Action.WasPressedThisFrame())
            {
                chat.OpenQuickChat(7);
            }
            
            if (Plugin.quickchat9Action.WasPressedThisFrame())
            {
                chat.OpenQuickChat(8);
            }
            
            if (Plugin.quickchat0Action.WasPressedThisFrame())
            {
                chat.OpenQuickChat(9);
            }
            
            if (Plugin.quickchatCloseAction.WasPressedThisFrame() && chat.IsQuickChatOpen)
            {
                chat.CloseQuickChat();
            }
        }
    }
}