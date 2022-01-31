using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace GssDbManageWrapper
{
    [RequireComponent(typeof(GssDbHub))]
    [RequireComponent(typeof(AreaDataManager))]
    [RequireComponent(typeof(UserDataManager))]
    public class GssDbHubTester : MonoBehaviour
    {
        [Header("テスト用のパラメータ")]
        [SerializeField]
        private Text _uiText;
        [SerializeField]
        private string _userName = "tester";
        [SerializeField]
        private int _areaId = 0;
        [SerializeField]
        private int _vertexId = 0;
        [SerializeField]
        private Vector3 _position = new Vector3(0, 0, 0);
        [SerializeField]
        private List<SamplePayLoadDataStructure> _messageJsons;
        [SerializeField]
        private MethodNames _requestMethod = MethodNames.GetUserNames;
        [SerializeField]
        private bool _sendRequest = false;


        private GssDbHub _gssDbHub;
        AreaDataManager _areaDataManager;
        UserDataManager _userDataManager;

        private void Start()
        {
            if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
            if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();
            if (_areaDataManager == null) _areaDataManager = GetComponent<AreaDataManager>();
        }

        private void Update()
        {
            ForTesting();
        }

        private void ForTesting()
        {
            if (_sendRequest)
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
                else if (_requestMethod == MethodNames.GetUserDatas)
                {
                    _gssDbHub.GetUserDatas(GetUserDatasFeedback);
                }
                else if (_requestMethod == MethodNames.SaveData)
                {
                    //_gssDbHub.SaveData(_userName, new MessageJson(false, _areaId, _vertexId, _position));
                }
                else if (_requestMethod == MethodNames.SetUserData)
                {
                    _gssDbHub.SetUserData(new UserData(_userName, _userDataManager.RandomColor()));
                }
                else if (_requestMethod == MethodNames.UpdateDatas)
                {
                    _gssDbHub.UpdateDatas(_userName, _messageJsons);
                }
                else if (_requestMethod == MethodNames.RemoveData)
                {
                    _gssDbHub.RemoveData(_userName, _areaId, _vertexId);
                }
                else if (_requestMethod == MethodNames.RemoveArea)
                {
                    _gssDbHub.RemoveArea(_userName, _areaId);
                }
                _sendRequest = false;
            }
        }



        private void GetAllDatasFeedback(PayloadData[] datas)
        {
            _uiText.text = "userName: message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName}:\n");
                _uiText.text = string.Concat(_uiText.text, $"{datas[i].ExtractData<SamplePayLoadDataStructure>().ToString()}.\n");
            }
        }
        private void GetUserNamesFeedback(PayloadData[] datas)
        {
            _uiText.text = "userName:\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var messageJson = JsonUtility.FromJson<SamplePayLoadDataStructure>(datas[i].data);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName}\n");
            }
        }
        private void GetUserDatasFeedback(PayloadData[] datas)
        {
            _uiText.text = "userName: message\n";
            for (int i = 0; i < datas.Length; i++)
            {
                var color = JsonUtility.FromJson<Color>(datas[i].data);
                _uiText.text = string.Concat(_uiText.text, $"[{i}] {datas[i].userName}:\n");
                _uiText.text = string.Concat(_uiText.text, $"{color}.\n");
            }
        }


        private void PostFeedback(string response)
        {
            if (response.Contains("succeeded") || !response.Contains("Error"))
            {
                UpdateLocalAllDatas();
                UpdateLocalUserNames();
            }
            else
            {
                Debug.LogError($"<color=red>[GssLocalManager]</color> " + response);
            }
        }

        private void UpdateLocalAllDatas()
        {
            _gssDbHub.GetAllDatas(LocalAllDatasSetFeedBack);
        }

        private void UpdateLocalUserNames()
        {
            _gssDbHub.GetUserNames(LocalUserNamesSetFeedBack);
        }

        private void LocalAllDatasSetFeedBack(PayloadData[] datas)
        {
            _areaDataManager.RefreshAllDatas(datas);
            TempUIVisualizeAllDatas();
        }
        private void LocalUserNamesSetFeedBack(PayloadData[] datas)
        {
            _userDataManager.UpdateDatas(datas);
            TempUIVisualizeAllNames();
        }

        private void TempUIVisualizeAllDatas()
        {
            var datas = _areaDataManager.GetAllDatas();
            _uiText.text = "";
            for (int i = 0; i < datas.Count; i++)
            {
                _uiText.text = string.Concat(_uiText.text, $"Data[{i}]:\n");
                _uiText.text = string.Concat(_uiText.text, $"areaId: {datas[i].areaId} ");
                _uiText.text = string.Concat(_uiText.text, $"vertexId: {datas[i].vertexId}\n");
                _uiText.text = string.Concat(_uiText.text, $"position: {datas[i].position.ToString()}\n");
            }
        }
        private void TempUIVisualizeAllNames()
        {
            var datas = _userDataManager._userNames.ToArray();
            _uiText.text = "";
            for (int i = 0; i < datas.Length; i++)
            {
                _uiText.text = string.Concat(_uiText.text, $"Data[{i}]:\n");
                _uiText.text = string.Concat(_uiText.text, $"userName: {datas[i]}\n");
            }
        }
    }
}


