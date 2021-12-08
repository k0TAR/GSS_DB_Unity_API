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
            Debug.LogError($"<color=blue>[GSSGetter]</color> Sending data to Google Sheets failed. Error: {request.error}");
        }
        else
        {
            var request_result = request.downloadHandler.text;
            print(request_result);

            var response = JsonExtension.FromJson<ResponseData>(request_result);
            for(int i = 0; i < response.Length; i++)
            {
                Debug.Log($"playerName : {response[i].playerName}, message : {response[i].message}");
            }
        }
    }

    [System.Serializable]
    public class ResponseData
    {
        [SerializeField]
        public string playerName;
        [SerializeField]
        public string message;
    }
}