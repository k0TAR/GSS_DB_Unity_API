using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GssDbManageWrapper
{
    public class GssGetter
    {
        public static IEnumerator GetUserDatas(string gasUrl, string userName)
        {
            yield return GetGssData(gasUrl, MethodNames.GetUserDatas, userName);
        }

        public static IEnumerator GetUserNames(string gasUrl)
        {
            yield return GetGssData(gasUrl, MethodNames.GetUserNames, "");
        }

        private static IEnumerator GetGssData(string gasUrl, MethodNames methodName, string userName)
        {
            UnityWebRequest request = 
                (methodName == MethodNames.GetUserNames) ?
                    request = UnityWebRequest.Get($"{gasUrl}?method={methodName}") 
                : (methodName == MethodNames.GetUserDatas) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(userName)}={userName}")
                : null;
            if(request == null)
            {
                Debug.LogError($"<color=blue>[GssGetter]</color> Behaviour for \"{methodName}\" is not implemented.");
                yield break;
            }


            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                Debug.LogError($"<color=blue>[GssGetter]</color> Sending data to GAS failed. Error: {request.error}.");
                yield break;
            }
            else
            {
                var request_result = request.downloadHandler.text;
                if (request_result[0] == 'E')
                {
                    Debug.Log(request_result);
                    yield break;
                }
                else
                {
                    var response = JsonExtension.FromJson<PayloadData>(request_result);

                    if(methodName == MethodNames.GetUserNames)
                    {
                        for (int i = 0; i < response.Length; i++)
                        {
                            Debug.Log($"response[{i}].userName : {response[i].userName}");
                        }
                    }
                    else if (methodName == MethodNames.GetUserDatas)
                    {
                        for (int i = 0; i < response.Length; i++)
                        {
                            if(response[i].message == null)
                            {
                                Debug.LogError($"<color=blue>[GssGetter]</color> response[i].message is null.");
                            }
                            Debug.Log($"[{i}] userName : {response[i].userName}, message : {response[i].message}");
                        }
                    }
                }
            }
        }
    }
}
