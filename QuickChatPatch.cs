using AYellowpaper.SerializedCollections;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;

namespace ToasterQuickChatPlus;

public class QuickChatPatch
{
    // public static void PrintStuff()
    // {
    //     UIChat chat = NetworkBehaviourSingleton<UIChat>.Instance;
    //     Plugin.Log.LogInfo($"QCM {chat.quickChatMessages}");
    //     for (int i = 0; i < chat.quickChatMessages.Count; i++)
    //     {
    //         AYellowpaper.SerializedCollections.SerializedKeyValuePair<int, Il2CppStringArray> kvp = chat.quickChatMessages._serializedList._items[i];
    //         string output = "";
    //         for (int j = 0; j < kvp.Value.Length; j++)
    //         {
    //             output += kvp.Value[j] + ", ";
    //         }
    //         Plugin.Log.LogInfo($"{kvp.Key}: {output}");
    //     }
    //     Plugin.Log.LogInfo($"quickChatIndex {chat.quickChatIndex}");
    //     Plugin.Log.LogInfo($"quickChatContainer.name {chat.quickChatContainer.name}");
    //     // Plugin.Log.LogInfo($"quickChatIndex {chat.quickchat}");
    //     // [Info   :Toaster Quick Chat Plus] QCM AYellowpaper.SerializedCollections.SerializedCollectionserializedDictionary`2[System.Int32,Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray]
    //     // [Info   :Toaster Quick Chat Plus] 1344666368: I got it!, Need stamina!, Take the shot!, Defending...,
    //     // [Info   :Toaster Quick Chat Plus] 1344666368: Nice shot!, Great pass!, Thanks!, What a save!,
    //     // [Info   :Toaster Quick Chat Plus] 1344666368: OMG!, Nooo!, Wow!, Close one!,
    //     // [Info   :Toaster Quick Chat Plus] 1344666368: $#@%!, No problem., Whoops..., Sorry!,
    //     // [Info   :Toaster Quick Chat Plus] quickChatIndex -1
    //     // [Info   :Toaster Quick Chat Plus] quickChatContainer.name QuickChatContainer
    // }

    
    // this is called both times the key is pressed, so it needs to check if IsQuickChatOpen
    [HarmonyPatch(typeof(UIChat), nameof(UIChat.OnQuickChat))]
    public static class UIChatOnQuickChatPatch
    {
        // [HarmonyPostfix]
        // public static void Postfix(UIChat __instance, int index)
        // {
        //     // Plugin.Log.LogInfo($"OnQuickChat {index}");
        // }
        
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
                int slotIndex = chat.quickChatIndex * 4 + index;
                if (Plugin.configQuickChatVisibility[slotIndex].Value == 0)
                {
                    chat.Client_SendClientChatMessage(chat.quickChatMessages[chat.quickChatIndex][index], false);
                }
                else
                {
                    chat.Client_SendClientChatMessage(chat.quickChatMessages[chat.quickChatIndex][index], true);
                }
                
                chat.CloseQuickChat();
                chat.IsQuickChatOpen = false;
                chat.quickChatIndex = -1;
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
        SerializedDictionary<int, Il2CppStringArray> qcm = new SerializedDictionary<int, Il2CppStringArray>();
        
        // for each menu
        for (int i = 0; i < 10; i++)
        {
            // get the messages for the menu
            string[] messagesInMenu = new[]
            {
                QuickChats.GetQuickChatByID(Plugin.configQuickChatIndex[i * 4 + 0].Value).quickchat,
                QuickChats.GetQuickChatByID(Plugin.configQuickChatIndex[i * 4 + 1].Value).quickchat,
                QuickChats.GetQuickChatByID(Plugin.configQuickChatIndex[i * 4 + 2].Value).quickchat,
                QuickChats.GetQuickChatByID(Plugin.configQuickChatIndex[i * 4 + 3].Value).quickchat,
            };
            qcm.Add(i, new Il2CppStringArray(messagesInMenu));
        }
        
        chat.quickChatMessages = qcm;
    }

    [HarmonyPatch(typeof(UIChat), nameof(UIChat.Start))]
    public static class UIChatStartPatch
    {
        [HarmonyPostfix]
        public static void Postfix(UIChat __instance)
        {
            // Plugin.Log.LogInfo($"UIChatStartPatch");
            UpdateMessagesInUIChat();
        }
    }
}