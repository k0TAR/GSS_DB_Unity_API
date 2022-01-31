using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GssDbManageWrapper
{
    public enum MethodNames
    {
        GetUserNames,
        SetUserData,
        GetUserDatas,
        GetAllDatas,
        SaveData,
        UpdateDatas,
        RemoveData,
        RemoveArea,
        CheckIfGssUrlValid,
        CheckIfGasUrlValid,
        CheckIfPlayerNameValid,
    }

    public class GssDbHub : MonoBehaviour
    {

        private void DefaultGetFeedBack(PayloadData[] datas)
        {
            foreach (var d in datas) Debug.Log(d);
        }
        private void DefaultPostFeedBack(string response)
        {
            Debug.Log(response);
        }



        public void GetAllDatas(Action<PayloadData[]> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultGetFeedBack;
            StartCoroutine(
                GssGetter.GetAllDatas(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), response => localDataFeedback((PayloadData[])response)));
        }

        public void GetUserNames(Action<PayloadData[]> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultGetFeedBack;
            StartCoroutine(
                GssGetter.GetUserNames(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), response => localDataFeedback((PayloadData[])response)));
        }

        public void GetUserDatas(Action<PayloadData[]> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultGetFeedBack;
            StartCoroutine(
                GssGetter.GetUserDatas(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), response => localDataFeedback((PayloadData[])response)));
        }
        public void SetUserData(UserData userData, Action<string> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultPostFeedBack;
            var userName = userData._userName;
            var colorJsonString = JsonUtility.ToJson(userData._color);
            StartCoroutine(
                GssPoster.SetUserData(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, colorJsonString, response => localDataFeedback((string)response)));
        }


        public void SaveData(
            string userName, SamplePayLoadDataStructure data, Action<string> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultPostFeedBack;

            string message = JsonUtility.ToJson(data);
            StartCoroutine(
                GssPoster.SaveUserData(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, message, response => localDataFeedback((string)response)));
        }

        public void UpdateDatas(string userName, List<SamplePayLoadDataStructure> datas, Action<string> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultPostFeedBack;

            string message = "[ ";
            foreach (var d in datas)
            {
                message += $" {JsonUtility.ToJson(d)},";
            }
            message = message.Remove(message.Length - 1);
            message += " ]";

            StartCoroutine(
                GssPoster.UpdateDatas(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, message, response => localDataFeedback((string)response)));
        }

        public void RemoveData(string userName, int areaId, int vertexId, Action<string> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultPostFeedBack;

            string message = $"{{" +
                        $"\"areaId\" : {areaId}, " +
                        $"\"vertexId\" : {vertexId} " +
                        $"}}";
            StartCoroutine(
                GssPoster.RemoveData(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, message, response => localDataFeedback((string)response)));
        }

        public void RemoveArea(string userName, int areaId, Action<string> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultPostFeedBack;

            string message = $"{{" +
                        $"\"areaId\" : {areaId} " +
                        $"}}";
            StartCoroutine(
                GssPoster.RemoveArea(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, message, response => localDataFeedback((string)response)));
        }


        public void CheckIfPlayerNameValid(
            string playerName, Action<bool, string> updatePlayerNameRelatedUI)
        {
            StartCoroutine(
                GssGetter.CheckIfPlayerNameValid(
                    GasUrlManager.GetUrl(),
                    GssUrlManager.GetUrl(),
                    playerName,
                    response => PlayerNameValidFeedBack(
                        (string)response, playerName, updatePlayerNameRelatedUI)
                    ));
        }

        private void PlayerNameValidFeedBack(
            string response,
            string playerName,
            Action<bool, string> updatePlayerNameRelatedUI = null)
        {
            bool isValid = !response.Contains("Invalid");
            updatePlayerNameRelatedUI?.Invoke(isValid, playerName);
        }

        public void CheckIfGssUrlValid
        (
            string gssUrl,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            StartCoroutine
            (
                GssGetter.CheckIfGssUrlValid
                (
                    GasUrlManager.GetUrl(),
                    gssUrl,
                    response => GssUrlValidFeedBack
                    (
                        (string)response,
                        saveKeyFeedBack,
                        updateKeyRelatedUiFeedBack,
                        validFeedback,
                        invalidFeedback
                    )
                )
            );
        }

        private void GssUrlValidFeedBack
        (
            string response,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            if (!response.Contains("Error"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
                validFeedback?.Invoke();
            }
            else
            {
                invalidFeedback?.Invoke();
            }
        }


        public void CheckIfGasUrlValid
        (
            string gasUrl,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            StartCoroutine
            (
                GssGetter.CheckIfGasUrlValid
                (
                    gasUrl,
                    response => GasUrlValidFeedBack
                    (
                        (string)response,
                        saveKeyFeedBack,
                        updateKeyRelatedUiFeedBack,
                        validFeedback,
                        invalidFeedback
                    )
                )
            );
        }

        private void GasUrlValidFeedBack
        (
            string response,
            Action saveKeyFeedBack = null,
            Action updateKeyRelatedUiFeedBack = null,
            Action validFeedback = null,
            Action invalidFeedback = null
        )
        {
            if (!response.Contains("Cannot"))
            {
                saveKeyFeedBack?.Invoke();
                updateKeyRelatedUiFeedBack?.Invoke();
                validFeedback?.Invoke();
            }
            else
            {
                invalidFeedback?.Invoke();
            }
        }
    }
}


