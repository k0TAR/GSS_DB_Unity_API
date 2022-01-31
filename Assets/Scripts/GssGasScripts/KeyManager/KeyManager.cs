using System;
using System.IO;
using UnityEngine;

public static class KeyManager
{
    public const string GSS_URL_PATH = "Assets/Resources/gssUrl.json";
    public const string GAS_URL_PATH = "Assets/Resources/gasUrl.json";

    public static string GetDataPathGssUrl()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var getFilesDir = currentActivity.Call<AndroidJavaObject>("getFilesDir"))
        {
            string secureDataPathForAndroid = getFilesDir.Call<string>("getCanonicalPath");
            return Path.Combine(secureDataPathForAndroid, "gssUrl.json");
        }
#else
        // TODO: 本来は各プラットフォームに対応した処理が必要
        return "Assets/Resources/gssUrl.json";
#endif
    }

    public static string GetDataPathGasUrl()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var getFilesDir = currentActivity.Call<AndroidJavaObject>("getFilesDir"))
        {
            string secureDataPathForAndroid = getFilesDir.Call<string>("getCanonicalPath");
            return Path.Combine(secureDataPathForAndroid, "gasUrl.json");
        }
#else
        // TODO: 本来は各プラットフォームに対応した処理が必要
        return "Assets/Resources/gasUrl.json";
#endif
    }


    //https://blog.mbaas.nifcloud.com/entry/9044
    public static string GetKeyData(string filePath, Action feedbackHandler = null)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> {nameof(filePath)} is empty.");
            return null;
        }

        string fileData;
        try
        {
            var streamReader = new StreamReader(filePath);
            fileData = streamReader.ReadToEnd();
            streamReader.Close();
        }
        catch
        {
            //Debug.LogError($"<color=blue>[KeyManager]</color> could not load the file.");
            return null;
        }


        var keys = JsonUtility.FromJson<Keys>(fileData);
        feedbackHandler?.Invoke();
        return keys.key;
    }

    public static void SaveKey(string filePath, string key, Action feedbackHandler = null)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> {nameof(key)} is empty.");
            return;
        }

        var jsonData = JsonUtility.ToJson(new Keys(key));

        try
        {
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(jsonData);
            streamWriter.Flush();
            streamWriter.Close();
        }
        catch
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> could not save the key.");
        }

        feedbackHandler?.Invoke();
    }

    public static void RemoveKeyFile(string filePath, Action feedbackHandler = null)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> {nameof(filePath)} is empty.");
            return;
        }

        try
        {
            File.Delete(filePath);
        }
        catch
        {
            Debug.LogError($"<color=blue>[KeyManager]</color> could delete the file: {filePath}.");
        }

        feedbackHandler?.Invoke();
    }

    [Serializable]
    private class Keys
    {
        public string key;

        public Keys(string key)
        {
            this.key = key;
        }
    }
}
