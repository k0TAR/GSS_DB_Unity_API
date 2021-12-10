using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GssDbManageWrapper
{
    public enum MethodNames
    {
        GetUserNames,
        GetUserDatas,
        SaveUserData,
    }

    public class GssDbHub : MonoBehaviour
    {
        private const string _gasURL = "https://script.google.com/macros/s/AKfycbybwPGWrYarv9B6MFL0mW2iozcIVcqvTf6Aa3268uaPn0svEMTRw8D6QSAaZ5W3Ex0B/exec";
        [SerializeField]
        private string _userName = "tester";
        [SerializeField]
        private string _message = "tester Unity Post";
        [SerializeField]
        private MethodNames _requestMethod = MethodNames.GetUserNames;
        [SerializeField]
        private bool _sendRequest = false;

        

        void Update()
        {
            if (_sendRequest)
            {
                if (_requestMethod == MethodNames.GetUserDatas)
                {
                    StartCoroutine(GssGetter.GetUserDatas(_gasURL, _userName));
                }
                else if (_requestMethod == MethodNames.GetUserNames)
                {
                    StartCoroutine(GssGetter.GetUserNames(_gasURL));
                }
                else if (_requestMethod == MethodNames.SaveUserData)
                {
                    StartCoroutine(GssPoster.SaveUserData(_gasURL, _userName, _message));
                }
                _sendRequest = false;
            }
        }
    }
}


