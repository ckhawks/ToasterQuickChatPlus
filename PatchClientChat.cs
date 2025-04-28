// using HarmonyLib;
// using UnityEngine.InputSystem.Utilities;
//
// namespace ToasterQuickChatPlus;
//
// public class PatchClientChat
// {
//     [HarmonyPatch(typeof(UIChat), nameof(UIChat.Client_SendClientChatMessage))]
//     class PatchUIChatClientSendClientChatMessage
//     {
//         [HarmonyPrefix]
//         static bool Prefix(UIChat __instance, string message)
//         {
//             Plugin.Log.LogInfo($"Patch: UIChat.Client_SendClientChatMessage (Prefix) was called.");
//             Plugin.chat = __instance;
//             string[] messageParts = message.Split(' ');
//
//             // if (messageParts[0].InvariantEqualsIgnoreCase("/becomepuck") || messageParts[0].InvariantEqualsIgnoreCase("/bep"))
//             // {
//             //     DisableAllCameraModes();
//             //     Plugin.client_spectatorIsPuck = true;
//             //     // Reparent the spectator camera to the puck
//             //     Plugin.spectatorCamera.transform.SetParent(Plugin.puckManager.GetPuck().transform);
//             //
//             //     // Optionally reset the local position and rotation of the camera relative to the puck
//             //     Plugin.spectatorCamera.transform.localPosition = Vector3.zero; // Center the camera on the puck
//             //     Plugin.spectatorCamera.transform.localRotation =
//             //         Quaternion.identity; // Align the camera's rotation with the puck
//             //
//             //     return false;
//             // }
//
//             if (messageParts[0].InvariantEqualsIgnoreCase("/qcconfig"))
//             {
//                 QuickChatPatch.PrintStuff();
//                 return false;
//                 // Plugin.Log.LogInfo($"{QuickChats.GetQuickChatsAlphabetical()}");
//             }
//             
//             return true;
//         }
//     }
// }