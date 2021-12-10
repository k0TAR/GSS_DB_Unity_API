using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GssPoster : MonoBehaviour
{
    private const string _URI = "https://script.google.com/macros/s/AKfycbybwPGWrYarv9B6MFL0mW2iozcIVcqvTf6Aa3268uaPn0svEMTRw8D6QSAaZ5W3Ex0B/exec";
    [SerializeField]
    private string _userName = "tester";
    [SerializeField]
    private string _message = "tester Unity Post";
    [SerializeField]
    private MethodNames _requestMethod = MethodNames.SaveUserData;
    [SerializeField]
    private bool _sendRequest = false;


    enum MethodNames
    {
        SaveUserData,
    }

    private void Update()
    {
        if (_sendRequest)
        {
            if (_requestMethod == MethodNames.SaveUserData)
            {
                StartCoroutine(SaveUserData(_userName, _message));
            }
            _sendRequest = false;
        }
    }

    public IEnumerator SaveUserData(string userName, string message)
    {
        var jsonBody = $"{{ \"method\" : \"{MethodNames.SaveUserData}\" , \"userName\" : \"{userName}\", \"message\" : \"{message}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = UnityWebRequest.Post($"{_URI}", "POST");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
            Debug.LogError($"<color=blue>[GSSGetter]</color> Sending data to GAS failed. Error: {request.error}");
        }
        else
        {
            var request_result = request.downloadHandler.text;

            if (request_result[0] == 'E')
            {
                print(request_result);
                yield break;
            }
            else
            {
                print(request_result);
                yield break;
            }
        }
    }
}
