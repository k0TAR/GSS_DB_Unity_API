using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace GssDbManageWrapper
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(GssDbHub))]
    public class GssDbHubTester : MonoBehaviour
    {
        [Header("テスト用のパラメータ")]
        [SerializeField]
        private Text _debugText;
        [SerializeField]
        private string _userName = "tester";
        [SerializeField]
        private List<SamplePayLoadDataStructure> _payloadDatas;
        [SerializeField]
        private MethodNames _requestMethod = MethodNames.GetUserNames;

        private GssDbHub _gssDbHub;

        private void Start()
        {
            if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        }

        public void SendRequest()
        {
            Debug.Log($"<color=blue>[GssDbHubTester]</color> Sending data to GAS... method={_requestMethod}.");

            if (_requestMethod == MethodNames.GetAllDatas)
            {
                _gssDbHub.GetAllDatas(GetAllDatasFeedback);
            }
            else if (_requestMethod == MethodNames.GetUserNames)
            {
                _gssDbHub.GetUserNames(GetUserNamesFeedback);
            }
            else if (_requestMethod == MethodNames.SaveDatas)
            {
                _gssDbHub.SaveDatas(_userName, _payloadDatas);
            }
            else if (_requestMethod == MethodNames.RemoveData)
            {
                _gssDbHub.RemoveData(_userName, _payloadDatas[0]);
            }
            else if (_requestMethod == MethodNames.RemoveUserDatas)
            {
                _gssDbHub.RemoveUserDatas(_userName);
            }
        }



        private void GetAllDatasFeedback(PayloadData[] datas)
        {
            _debugText.text = "userName: message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                _debugText.text = string.Concat(_debugText.text, $"[{i}] {datas[i].userName}:\n");
                _debugText.text = string.Concat(_debugText.text, $"{datas[i].ExtractData<SamplePayLoadDataStructure>().ToString()}.\n");
            }
        }
        private void GetUserNamesFeedback(PayloadData[] datas)
        {
            _debugText.text = "userName:\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<SamplePayLoadDataStructure>(datas[i].data);
                _debugText.text = string.Concat(_debugText.text, $"[{i}] {datas[i].userName}\n");
            }
        }
        private void GetUserDatasFeedback(PayloadData[] datas)
        {
            _debugText.text = "userName: message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var color = JsonUtility.FromJson<Color>(datas[i].data);
                _debugText.text = string.Concat(_debugText.text, $"[{i}] {datas[i].userName}:\n");
                _debugText.text = string.Concat(_debugText.text, $"{color}.\n");
            }
        }


        private void PostFeedback(string response)
        {
            if (response.Contains("succeeded") || !response.Contains("Error"))
            {

            }
            else
            {
                Debug.LogError($"<color=red>[GssLocalManager]</color> " + response);
            }
        }


    }
}


