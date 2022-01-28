using UnityEngine;

public static class GssUrlManager
{
    public static void SaveUrl(string gssUrl)
    {
        if (string.IsNullOrEmpty(gssUrl))
        {
            Debug.LogError($"<color=blue>[GssUrlManager]</color> field is empty.");
            return;
        }

        KeyManager.SaveKey(KeyManager.GetDataPathGssUrl(), gssUrl);
    }

    public static string GetUrl()
    {
        return KeyManager.GetKeyData(KeyManager.GetDataPathGssUrl());
    }

    public static bool IsUrlAssigned()
    {
        var gssUrl = GetUrl();
        return !string.IsNullOrEmpty(gssUrl);
    }

    public static void ResetUrl()
    {
        KeyManager.RemoveKeyFile(KeyManager.GetDataPathGssUrl());
    }
}
