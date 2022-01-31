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
        GetAllDatas,
        SaveDatas,
        RemoveData,
        RemoveUserDatas,
        CheckIfGssUrlValid,
        CheckIfGasUrlValid,
    }

    public class GssDbHub : MonoBehaviour
    {
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


        public void SaveDatas(string userName, List<SamplePayLoadDataStructure> datas, Action<string> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultPostFeedBack;

            string datasString = "";
            foreach (var d in datas)
            {
                datasString += $" {JsonUtility.ToJson(d)},";
            }
            datasString = datasString.Remove(datasString.Length - 1);

            StartCoroutine(
                GssPoster.SaveDatas(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, datasString, response => localDataFeedback((string)response)));
        }

        public void RemoveData(string userName, SamplePayLoadDataStructure data, Action<string> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultPostFeedBack;

            string dataString = $"{JsonUtility.ToJson(data)}";

            StartCoroutine(
                GssPoster.RemoveData(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, dataString, response => localDataFeedback((string)response)));
        }

        public void RemoveUserDatas(string userName, Action<string> localDataFeedback = null)
        {
            if (localDataFeedback == null) localDataFeedback = DefaultPostFeedBack;

            StartCoroutine(
                GssPoster.RemoveUserDatas(GasUrlManager.GetUrl(), GssUrlManager.GetUrl(), userName, response => localDataFeedback((string)response)));
        }



        private void DefaultGetFeedBack(PayloadData[] datas)
        {
            foreach (var d in datas) Debug.Log(d);
        }
        private void DefaultPostFeedBack(string response)
        {
            Debug.Log(response);
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


