using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace GssDbManageWrapper
{
    public static class GssPoster
    {
        public static IEnumerator SaveDatas(string gasUrl, string gssUrl, string userName, string datas, Action<object> feedbackHandler = null)
        {
            var jsonBody = $"{{ \"method\" : \"{MethodNames.SaveDatas}\" , \"gssUrl\" : \"{gssUrl}\", \"userName\" : \"{userName}\", \"{nameof(datas)}\" : [{datas}] }}";
            byte[] payloadRaw = Encoding.UTF8.GetBytes(jsonBody);

            yield return PostToGss(gasUrl, MethodNames.SaveDatas, payloadRaw, feedbackHandler);
        }

        public static IEnumerator RemoveData(string gasUrl, string gssUrl, string userName, string data, Action<object> feedbackHandler = null)
        {
            var jsonBody = $"{{ \"method\" : \"{MethodNames.RemoveData}\" , \"gssUrl\" : \"{gssUrl}\", \"userName\" : \"{userName}\", \"{nameof(data)}\" : {data} }}";
            byte[] payloadRaw = Encoding.UTF8.GetBytes(jsonBody);

            yield return PostToGss(gasUrl, MethodNames.RemoveData, payloadRaw, feedbackHandler);
        }

        public static IEnumerator RemoveUserDatas(string gasUrl, string gssUrl, string userName, Action<object> feedbackHandler = null)
        {
            var jsonBody = $"{{ \"method\" : \"{MethodNames.RemoveUserDatas}\" , \"gssUrl\" : \"{gssUrl}\", \"userName\" : \"{userName}\" }}";
            byte[] payloadRaw = Encoding.UTF8.GetBytes(jsonBody);

            yield return PostToGss(gasUrl, MethodNames.RemoveUserDatas, payloadRaw, feedbackHandler);
        }

        private static IEnumerator PostToGss(string gasUrl, MethodNames methodName, byte[] payload, Action<object> feedbackHandler = null)
        {
            UnityWebRequest request =
                (methodName == MethodNames.SaveDatas || methodName == MethodNames.RemoveData || methodName == MethodNames.RemoveUserDatas) ?
                    UnityWebRequest.Post($"{gasUrl}", "POST")
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
                Debug.LogError($"<color=blue>[GssPoster]</color> Sending data to GAS failed. Error: {request.error}");
            }
            else
            {
                var request_result = request.downloadHandler.text;

                feedbackHandler?.Invoke(request_result);
                yield break;
            }
        }


    }
}

