using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GssDbManageWrapper
{
    public class GssGetter
    {
        public static IEnumerator GetAllDatas(string gasUrl, string gssUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.GetAllDatas, "", feedbackHandler);
        }

        public static IEnumerator GetUserNames(string gasUrl, string gssUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.GetUserNames, "", feedbackHandler);
        }

        public static IEnumerator CheckIfGssUrlValid(string gasUrl, string gssUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, gssUrl, MethodNames.CheckIfGssUrlValid, "", feedbackHandler);
        }

        public static IEnumerator CheckIfGasUrlValid(string gasUrl, Action<object> feedbackHandler = null)
        {
            yield return GetGssData(gasUrl, "", MethodNames.CheckIfGasUrlValid, "", feedbackHandler);
        }


        private static IEnumerator GetGssData(string gasUrl, string gssUrl, MethodNames methodName, string userName, Action<object> feedbackHandler = null)
        {
            UnityWebRequest request =
                (methodName == MethodNames.GetAllDatas
                || methodName == MethodNames.GetUserNames
                || methodName == MethodNames.CheckIfGssUrlValid) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}&{nameof(gssUrl)}={gssUrl}")
                : (methodName == MethodNames.CheckIfGasUrlValid) ?
                    UnityWebRequest.Get($"{gasUrl}?method={methodName}")
                : null;

            if (request == null)
            {
                Debug.LogError($"<color=blue>[GssGetter]</color> Behaviour for \"{methodName}\" is not implemented.");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError($"<color=blue>[GssGetter]</color> Sending data to GAS failed. Error: {request.error}.");
                if (methodName == MethodNames.CheckIfGasUrlValid)
                {
                    feedbackHandler?.Invoke(request.error);
                }
            }
            else
            {
                var request_result = request.downloadHandler.text;

                if (request_result.Contains("Error"))
                {
                    Debug.LogError($"<color=blue>[GssGetter]</color> {request_result}");
                }

                if (methodName == MethodNames.CheckIfGssUrlValid)
                {
                    feedbackHandler?.Invoke(request_result);
                }
                else if (methodName == MethodNames.CheckIfGasUrlValid)
                {
                    feedbackHandler?.Invoke(request_result);
                }
                else
                {
                    Debug.Log(request_result);
                    var response = JsonExtension.FromJson<PayloadData>(request_result);
                    if (methodName == MethodNames.GetAllDatas)
                    {
                        feedbackHandler?.Invoke(response);
                    }
                    else if (methodName == MethodNames.GetUserNames)
                    {
                        feedbackHandler?.Invoke(response);
                    }
                }
            }
        }
    }
}
