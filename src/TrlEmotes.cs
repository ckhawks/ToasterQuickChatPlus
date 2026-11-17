using System;
using System.Reflection;

namespace ToasterQuickChatPlus;

// Optional bridge to Toaster's Reskin Loader's emotes (ToasterReskinLoader.EmoteAPI), found by
// reflection so QCP has no hard dependency on TRL. Without TRL the emote feature doesn't exist.
// An emote is sent as its ":name:" text; TRL on each receiving client draws the image.
public static class TrlEmotes
{
    const int MIN_API_VERSION = 1;

    public enum State
    {
        // TRL isn't installed or its emotes aren't running.
        Absent,
        // TRL is installed but too old to have the emote API.
        Outdated,
        Ready,
    }

    public static event Action Changed;

    static bool searched;
    static bool trlFound;
    static MethodInfo getMyEmoteIds;
    static MethodInfo getMyEmoteName;
    static MethodInfo getEmoteImageTag;
    static PropertyInfo isAvailable;

    public static State Current
    {
        get
        {
            Find();
            if (!trlFound)
            {
                return State.Absent;
            }
            if (getMyEmoteIds == null)
            {
                return State.Outdated;
            }
            return isAvailable != null && (bool)isAvailable.GetValue(null) ? State.Ready : State.Absent;
        }
    }

    // Plugins can load in any order, so this keeps looking until TRL shows up.
    static void Find()
    {
        if (searched)
        {
            return;
        }
        Type api = null;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name == "ToasterReskinLoader")
            {
                trlFound = true;
                api = assembly.GetType("ToasterReskinLoader.EmoteAPI");
                break;
            }
        }
        if (!trlFound)
        {
            return;
        }
        searched = true;
        if (api == null)
        {
            Plugin.Log("Toaster's Reskin Loader is too old for emotes in quick chat.");
            return;
        }
        FieldInfo version = api.GetField("VERSION");
        if (version == null || (int)version.GetValue(null) < MIN_API_VERSION)
        {
            Plugin.Log("Toaster's Reskin Loader emote API is too old for quick chat.");
            return;
        }
        getMyEmoteIds = api.GetMethod("GetMyEmoteIds");
        getMyEmoteName = api.GetMethod("GetMyEmoteName");
        getEmoteImageTag = api.GetMethod("GetEmoteImageTag");
        isAvailable = api.GetProperty("IsAvailable");
        EventInfo changed = api.GetEvent("MyEmotesChanged");
        changed?.AddEventHandler(null, (Action)OnChanged);
        Plugin.Log("Connected to Toaster's Reskin Loader emotes.");
    }

    static void OnChanged()
    {
        try
        {
            Changed?.Invoke();
        }
        catch (Exception e)
        {
            Plugin.LogError($"Updating emote quick chats failed: {e}");
        }
    }

    public static string[] MyEmoteIds()
    {
        return Current == State.Ready ? (string[])getMyEmoteIds.Invoke(null, null) : new string[0];
    }

    // Null when the emote isn't in the player's set any more, or TRL isn't there.
    public static string NameFor(string id)
    {
        return Current == State.Ready ? (string)getMyEmoteName.Invoke(null, new object[] { id }) : null;
    }

    // Rich text drawing the emote, or null while it loads.
    public static string ImageTagFor(string id)
    {
        return Current == State.Ready ? (string)getEmoteImageTag.Invoke(null, new object[] { id }) : null;
    }
}
