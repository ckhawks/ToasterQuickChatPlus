using System.Collections.Generic;
using System.Linq;

namespace ToasterQuickChatPlus;

public static class QuickChats
{
    public static Dictionary<int, string> quickchats = 
        new Dictionary<int, string>(){
            { 0, "I got it!" },
            { 1, "Need stamina!" },
            { 2, "Take the shot!" },
            { 3, "Defending..." },
            { 4, "Nice shot!" },
            { 5, "Great pass!" },
            { 6, "Thanks!" },
            { 7, "What a save!" },
            { 8, "OMG!" },
            { 9, "Nooo!" },
            { 10, "Wow!" },
            { 11, "Close one!" },
            { 12, "$#@%!" },
            { 13, "No problem." },
            { 14, "Whoops..." },
            { 15, "Sorry!" },
            { 16, "vpas" },
            { 17, "I love Toaster's plugins!" },
            { 18, "Join the Puck Modding Discord: http://discord.puckstats.io" },
            { 19, "Calculated." },
            { 20, "Faking..." },
            { 21, "Nice moves!" },
            { 22, "Please don't hurt me." },
            { 23, "I'm open!" },
            { 24, "In position." },
            { 25, "No way!" },
            { 26, "Great clear!" },
            { 27, "Savage!" },
            { 28, "Holy cow!" },
            { 29, "All yours." },
            { 30, "Okay." },
            { 31, "Siiick!" },
            { 32, "This is Puck!" },
            { 33, "This is Puch!" },
            { 34, "I love jumping!" },
            { 35, "Watch this!" },
            // { 36, "I love jumping!" },
            { 37, "Uh oh..." },
            { 38, "Good try!" },
            { 39, "Nice block!" },
            { 40, "Spread out!" },
            { 41, "Stay back!" },
            { 42, "Be careful." },
            { 43, "Drop...?" },
            { 44, "Watch out!" },
            { 45, "Incoming!" },
            { 46, "Centering..." },
            { 47, "Two on one!" },
            { 48, "Backchecking..." },
            { 49, "Checking..." },
            { 50, "Get open!" },
            { 51, "What a banan..." },
            { 52, "Bang!" },
            { 53, "I'm dead!" },
            { 54, "What a play!" },
            { 55, "Crossing!" },
            { 56, "I love GAFURIX!" },
            { 57, "Who wants toast?" },
            { 58, "Please vote." },
            { 59, "Oh my god..." },
            { 60, "My puch!" },
            { 61, "Help me!" },
            { 62, "gg" },
            { 63, "Good game!" },
            { 64, "Well played." },
            { 65, "Rematch!" },
            { 66, "That was fun!" },
            { 67, "Oops!" },
        };

    // Returns QuickChat objects sorted by their text, alphabetically
    public static List<QuickChat> GetQuickChatsAlphabetical()
    {
        return quickchats
            .Select(kv => new QuickChat
            {
                id = kv.Key,
                quickchat = kv.Value
            })
            .OrderBy(q => q.quickchat)
            .ToList();
    }

    // Returns just the quickchat strings, sorted alphabetically
    public static List<string> GetQuickChatsAlphabeticalAsStrings()
    {
        return GetQuickChatsAlphabetical()
            .Select(q => q.quickchat)
            .ToList();
    }

    // Finds a QuickChat by its text; returns null if not found
    public static QuickChat GetQuickChatByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        // Try to find the first matching entry
        var kv = quickchats
            .FirstOrDefault(pair => pair.Value == name);

        // If Key is 0 but Value isn't matching, FirstOrDefault returns the
        // default KeyValuePair (0, null). So check Value explicitly.
        if (kv.Value == null)
            return null;

        return new QuickChat
        {
            id = kv.Key,
            quickchat = kv.Value
        };
    }
    
    public static QuickChat GetQuickChatByID(int id)
    {
        // Try to find the first matching entry
        var kv = quickchats
            .FirstOrDefault(pair => pair.Key == id);

        return new QuickChat
        {
            id = kv.Key,
            quickchat = kv.Value
        };
    }
}

public class QuickChat
{
    public string quickchat = "";
    public int id = 0;
}