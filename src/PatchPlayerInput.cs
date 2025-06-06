using HarmonyLib;

namespace ToasterQuickChatPlus;

public static class PatchPlayerInput
{
    // static readonly FieldInfo _isFocusedField = typeof(UIComponent)
    //     .GetField("IsFocused", 
    //         BindingFlags.Instance | BindingFlags.NonPublic);    
    //
    [HarmonyPatch(typeof(PlayerInput), "Update")]
    class PatchPlayerInputUpdate
    {
        [HarmonyPostfix]
        static void Postfix(PlayerInput __instance)
        {
            // Plugin.Log($"PlayerInput update1");
            UIChat chat = UIChat.Instance;
            // Plugin.Log($"PlayerInput update2");
            // bool chatIsFocused = (bool) _isFocusedField.GetValue(chat);
            // Plugin.Log($"PlayerInput update3");
            // do not use the actions if chat is open
            if (chat.IsFocused) return;
            // Plugin.Log($"PlayerInput update4");

            if (Plugin.quickchat5Action.WasPressedThisFrame())
            {
                Plugin.Log($"Pressed 5");
                chat.OpenQuickChat(4);
            }
            
            if (Plugin.quickchat6Action.WasPressedThisFrame())
            {
                Plugin.Log($"Pressed 6");
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
                Plugin.Log($"Pressed close");
                chat.CloseQuickChat();
            }
        }
    }
}