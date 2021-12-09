using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GssGetter : MonoBehaviour
{
    private const string _URI = "https://script.google.com/macros/s/AKfycbybwPGWrYarv9B6MFL0mW2iozcIVcqvTf6Aa3268uaPn0svEMTRw8D6QSAaZ5W3Ex0B/exec";
    [SerializeField]
    private string _playerName = "";
    [SerializeField]
    private string _message = "";
    [SerializeField]
    private MethodNames _requestMethod = MethodNames.GetUserDatas;
    [SerializeField]
    private bool _sendRequest = false;
    

    enum MethodNames
    {
        GetPlayerNames,
        GetUserDatas,
    }

    private void Update()
    {
        if(_sendRequest)
        {
            if(_requestMethod == MethodNames.GetUserDatas)
            {
                StartCoroutine(GetUserDatas(_playerName));
            }
            else if (_requestMethod == MethodNames.GetPlayerNames)
            {
                StartCoroutine(GetPlayerNames());
            }
            _sendRequest = false;
        }
    }

    public IEnumerator GetUserDatas(string playerName)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{_URI}?playerName={playerName}&method={MethodNames.GetUserDatas}");

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
                var response = JsonExtension.FromJson<ResponseData>(request_result);
                for (int i = 0; i < response.Length; i++)
                {
                    if(response[i].playerName != null)
                    {
                        Debug.Log($"playerName : {response[i].playerName}, message : {response[i].message}");
                    }
                }
            }
        }
    }

    public IEnumerator GetPlayerNames()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{_URI}?method={MethodNames.GetPlayerNames}");

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
                var response = JsonExtension.FromJson<ResponseData>(request_result);
                for (int i = 0; i < response.Length; i++)
                {
                    Debug.Log($"response[{i}].playerName : {response[i].playerName}");
                }
            }
        }
    }

    [System.Serializable]
    private class ResponseData
    {
        [SerializeField]
        public string playerName;
        [SerializeField]
        public string message;
    }
}