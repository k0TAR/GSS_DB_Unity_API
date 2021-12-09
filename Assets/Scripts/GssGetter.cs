using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GssGetter : MonoBehaviour
{
    private const string URI = "https://script.google.com/macros/s/AKfycbybwPGWrYarv9B6MFL0mW2iozcIVcqvTf6Aa3268uaPn0svEMTRw8D6QSAaZ5W3Ex0B/exec";
    [SerializeField]
    private string _searchingUserId = "";
    [SerializeField]
    private MethodNames _requestMethod = MethodNames.getUserDatas;
    [SerializeField]
    private bool _sendRequest = false;
    

    enum MethodNames
    {
        getUserIds,
        getUserDatas,
    }

    private void Update()
    {
        if(_sendRequest)
        {
            StartCoroutine(GetGssDatas(_searchingUserId));
            _sendRequest = false;
        }
    }

    public IEnumerator GetGssDatas(string userId)
    {
        UnityWebRequest request = UnityWebRequest.Get($"{URI}?userId={userId}&method={_requestMethod}");

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
            if(request_result[0] == 'E')
            {
                yield break;
            }
            else
            {
                var response = JsonExtension.FromJson<ResponseData>(request_result);
                for (int i = 0; i < response.Length; i++)
                {
                    Debug.Log($"playerName : {response[i].playerName}, message : {response[i].message}");
                }
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