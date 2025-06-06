using System.Reflection;
using AYellowpaper.SerializedCollections;
using HarmonyLib;

namespace ToasterQuickChatPlus;

public static class QuickChatPatch
{
    static readonly FieldInfo _quickChatIndexField = typeof(UIChat)
        .GetField("quickChatIndex", 
            BindingFlags.Instance | BindingFlags.NonPublic);    
    
    static readonly FieldInfo _quickChatMessagesField = typeof(UIChat)
        .GetField("quickChatMessages", 
            BindingFlags.Instance | BindingFlags.NonPublic);    
    
    // this is called both times the key is pressed, so it needs to check if IsQuickChatOpen
    [HarmonyPatch(typeof(UIChat), nameof(UIChat.OnQuickChat))]
    public static class UIChatOnQuickChatPatch
    {
        
        // Don't do anything when this is called if the menu is open, because we don't want it to try to handle submenu selection with 5-0
        [HarmonyPrefix]
        public static bool Prefix(UIChat __instance, int index)
        {
            // Plugin.Log.LogInfo($"OnQuickChat {index}");
            UIChat chat = NetworkBehaviourSingleton<UIChat>.Instance;
            if (index > 3 && chat.IsQuickChatOpen)
            {
                return false;
            }
            
            if (chat.IsQuickChatOpen) 
            {
                int quickChatIndex = (int) _quickChatIndexField.GetValue(chat);
                SerializedDictionary<int,string[]> quickChatMessages = (SerializedDictionary<int,string[]>) _quickChatMessagesField.GetValue(chat);
                
                int slotIndex = quickChatIndex * 4 + index;
                if (Plugin.modSettings.quickChatSettings[slotIndex].visibility == 0)
                {
                    chat.Client_SendClientChatMessage(quickChatMessages[quickChatIndex][index], false);
                }
                else
                {
                    chat.Client_SendClientChatMessage(quickChatMessages[quickChatIndex][index], true);
                }
                
                chat.CloseQuickChat();
                chat.IsQuickChatOpen = false;
                quickChatIndex = -1;
                _quickChatIndexField.SetValue(chat, quickChatIndex);
                return false;
            }

            return true;
        }
    }
    
    // this is called when the key is pressed and opens the menu
    // [HarmonyPatch(typeof(UIChat), nameof(UIChat.OpenQuickChat))]
    // public static class UIChatOpenQuickChatPatch
    // {
    //     [HarmonyPostfix]
    //     public static void Postfix(UIChat __instance, int index)
    //     {
    //         Plugin.Log.LogInfo($"OpenQuickChat {index}");
    //     }
    // }

    public static void UpdateMessagesInUIChat()
    {
        UIChat chat = NetworkBehaviourSingleton<UIChat>.Instance;
        SerializedDictionary<int, string[]> qcm = new SerializedDictionary<int, string[]>();
        
        // for each menu
        for (int i = 0; i < 10; i++)
        {
            // get the messages for the menu
            string[] messagesInMenu = new[]
            {
                QuickChats.GetQuickChatByID(Plugin.modSettings.quickChatSettings[i * 4 + 0].index).quickchat,
                QuickChats.GetQuickChatByID(Plugin.modSettings.quickChatSettings[i * 4 + 1].index).quickchat,
                QuickChats.GetQuickChatByID(Plugin.modSettings.quickChatSettings[i * 4 + 2].index).quickchat,
                QuickChats.GetQuickChatByID(Plugin.modSettings.quickChatSettings[i * 4 + 3].index).quickchat,
            };
            qcm.Add(i, messagesInMenu);
        }
        
        _quickChatMessagesField.SetValue(chat, qcm);
    }

    public static void Setup()
    {
        UpdateMessagesInUIChat();
    }
}