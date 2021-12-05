using MiniJSON;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GssGetter : MonoBehaviour
{
    private const string URI = "https://script.google.com/macros/s/AKfycbybwPGWrYarv9B6MFL0mW2iozcIVcqvTf6Aa3268uaPn0svEMTRw8D6QSAaZ5W3Ex0B/exec";

    private void Start()
    {
        StartCoroutine(Get());
    }

    public IEnumerator Get()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{URI}?");

        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var result = request.downloadHandler.text;
            Debug.Log(result);

            var response = Json.Deserialize(result);
            Debug.Log(string.Join(",", response));
        }
    }

    [System.Serializable]
    public class ResponseData
    {
        public string userId;
        public string updateTime;
        public string playerName;
        public string message;
    }
}