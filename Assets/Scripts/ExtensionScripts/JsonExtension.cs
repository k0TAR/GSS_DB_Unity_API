using System;
using UnityEngine;

public static class JsonExtension
{
    public static T[] FromJson<T>(string json)
    {
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        if (wrapper == null)
        {
            Debug.LogError($"<color=blue>[JsonExtension]</color> FromJson did not work as expected.");
        }
        return wrapper.payloadDatas;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] payloadDatas;
    }
}