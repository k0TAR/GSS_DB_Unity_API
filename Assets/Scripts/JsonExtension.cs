using System;
using UnityEngine;
using System.Collections;

public static class JsonExtension
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.actualPayload;
    }

    [Serializable]
    private class Wrapper<T>
    {
        [SerializeField]
        public T[] actualPayload;
    }
}