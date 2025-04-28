namespace ToasterQuickChatPlus;

public static class TUtils
{
    public static System.Collections.Generic.List<T> ToSystemList<T>(Il2CppSystem.Collections.Generic.List<T> il2CPPList)
    {
        System.Collections.Generic.List<T> systemList = new System.Collections.Generic.List<T>(il2CPPList.Count); // Create a new System.Collections.Generic.List<T>
        foreach (T item in il2CPPList)
        {
            systemList.Add(item); // Add each item from the Il2Cpp list to the System list
        }
    
        return systemList;
    }
    
    public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(System.Collections.Generic.List<T> systemList)
    {
        Il2CppSystem.Collections.Generic.List<T> il2CPPList = new Il2CppSystem.Collections.Generic.List<T>(systemList.Count); // Create a new System.Collections.Generic.List<T>
        foreach (T item in systemList)
        {
            il2CPPList.Add(item); // Add each item from the Il2Cpp list to the System list
        }
    
        return il2CPPList;
    }
}