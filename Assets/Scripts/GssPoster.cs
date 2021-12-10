using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GssDbManageWrapper
{
    public static class GssPoster
    {
        public static IEnumerator SaveUserData(string gasUrl, string userName, string message)
        {
            var jsonBody = $"{{ \"method\" : \"{MethodNames.SaveUserData}\" , \"userName\" : \"{userName}\", \"message\" : \"{message}\"}}";
            byte[] payloadRaw = Encoding.UTF8.GetBytes(jsonBody);

            yield return PostToGss(gasUrl, MethodNames.SaveUserData, payloadRaw);
        }

        private static IEnumerator PostToGss(string gasUrl, MethodNames methodName, byte[] payload)
        {
            UnityWebRequest request =
                (methodName == MethodNames.SaveUserData) ?
                    request = UnityWebRequest.Post($"{gasUrl}", "POST")
                : null;
            if (request == null)
            {
                Debug.LogError($"<color=blue>[GssPoster]</color> Behaviour for \"{methodName}\" is not implemented.");
                yield break;
            }


            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(payload);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();


            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
                Debug.LogError($"<color=blue>[GssPoster]</color> Sending data to GAS failed. Error: {request.error}");
            }
            else
            {
                var request_result = request.downloadHandler.text;

                if (request_result[0] == 'E')
                {
                    Debug.Log($"<color=blue>[GssPoster]</color> {request_result}");
                    yield break;
                }
                else
                {
                    Debug.Log(request_result);
                    yield break;
                }
            }
        }


    }
}

