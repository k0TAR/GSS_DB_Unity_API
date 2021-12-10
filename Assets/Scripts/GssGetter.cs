using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GssGetter : MonoBehaviour
{
    private const string _URI = "https://script.google.com/macros/s/AKfycbybwPGWrYarv9B6MFL0mW2iozcIVcqvTf6Aa3268uaPn0svEMTRw8D6QSAaZ5W3Ex0B/exec";
    [SerializeField]
    private string _userName = "tester";
    [SerializeField]
    private MethodNames _requestMethod = MethodNames.GetUserDatas;
    [SerializeField]
    private bool _sendRequest = false;
    

    enum MethodNames
    {
        GetUserNames,
        GetUserDatas,
    }

    private void Update()
    {
        if(_sendRequest)
        {
            if(_requestMethod == MethodNames.GetUserDatas)
            {
                StartCoroutine(GetUserDatas(_userName));
            }
            else if (_requestMethod == MethodNames.GetUserNames)
            {
                StartCoroutine(GetPlayerNames());
            }
            _sendRequest = false;
        }
    }

    public IEnumerator GetUserDatas(string userName)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{_URI}?method={MethodNames.GetUserDatas}&userName={userName}");

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
                var response = JsonExtension.FromJson<PayloadData>(request_result);
                for (int i = 0; i < response.Length; i++)
                {
                    if(response[i].userName != null)
                    {
                        Debug.Log($"playerName : {response[i].userName}, message : {response[i].message}");
                    }
                }
            }
        }
    }

    public IEnumerator GetPlayerNames()
    {
        UnityWebRequest request = UnityWebRequest.Get($"{_URI}?method={MethodNames.GetUserNames}");

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
                var response = JsonExtension.FromJson<PayloadData>(request_result);
                for (int i = 0; i < response.Length; i++)
                {
                    Debug.Log($"response[{i}].userName : {response[i].userName}");
                }
            }
        }
    }
}