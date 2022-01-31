using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GssDbManageWrapper
{
    public class AreaDataManager : MonoBehaviour
    {
        public Dictionary<string, List<SamplePayLoadDataStructure>> _allDatas = new Dictionary<string, List<SamplePayLoadDataStructure>>();
        public List<SamplePayLoadDataStructure> _userCurrentArea = new List<SamplePayLoadDataStructure>();

        private bool _isUpdating = false;

        public int GetCurrentAreaId(string userName)
        {
            var userDatas = GetUserDatas(userName);
            int maxAreaId = 0;
            if (userDatas != null)
            {
                maxAreaId = userDatas.Max(x => x.areaId);
            }

            bool isAreaClosed = IsAreaIdClosed(userName, maxAreaId);
            return isAreaClosed ? maxAreaId + 1 : maxAreaId;
        }


        public bool IsAreaIdClosed(string userName, int areaId)
        {
            var areaIdData = GetAreaMessages(userName, areaId);
            bool isClosed = false;
            if (areaIdData != null)
            {
                isClosed = areaIdData[0].isClosed;
            }
            return isClosed;
        }


        public List<SamplePayLoadDataStructure> GetAreaMessages(string userName, int areaId)
        {
            var userDatas = GetUserDatas(userName);
            if (userDatas != null)
            {
                var areaIdData = userDatas.Where(x => x.areaId == areaId);
                return areaIdData.ToList();
            }

            return null;
        }

        public void RefreshUserAreaData()
        {
            _userCurrentArea.Clear();
        }

        //閉曲線まで単純に追加していく
        public void AddPositinToCurrentAreaDatas(string userName, Vector3 position)
        {
            var currentAreaId = GetCurrentAreaId(userName);
            var nextVertexId = _userCurrentArea.Count;
            _userCurrentArea.Add(new SamplePayLoadDataStructure(false, currentAreaId, nextVertexId, position));
        }

        public List<Vector3> GetCurrentAreaDatasAsVector()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (var m in _userCurrentArea)
            {
                positions.Add(m.position);
            }
            return positions;
        }

        //閉曲線のときに全てを更新
        public void UpdateCurrentAreaDatas(string userName, List<Vector3> vertices)
        {
            RefreshUserAreaData();
            var currentAreaId = GetCurrentAreaId(userName);
            List<SamplePayLoadDataStructure> messageData = new List<SamplePayLoadDataStructure>();
            for (int i = 0; i < vertices.Count; ++i)
            {
                messageData.Add(new SamplePayLoadDataStructure(true, currentAreaId, i, vertices[i]));
            }
            _userCurrentArea = messageData;
        }

        public List<Vector3> GetAreaVerticies(string userName, int areaId)
        {
            var userDatas = GetUserDatas(userName);
            var areaIdData = userDatas.Where(x => x.areaId == areaId);
            List<Vector3> verticies = new List<Vector3>();
            foreach (SamplePayLoadDataStructure m in areaIdData)
            {
                verticies.Add(m.position);
            }
            return verticies;
        }


        public List<SamplePayLoadDataStructure> GetUserDatas(string userName)
        {
            if (_allDatas.ContainsKey(userName))
            {
                var filteredUserDatas = _allDatas[userName];
                return filteredUserDatas;
            }
            else
            {
                return null;
            }
        }

        public List<SamplePayLoadDataStructure> GetAllDatas()
        {
            List<SamplePayLoadDataStructure> datas = new List<SamplePayLoadDataStructure>();
            foreach (List<SamplePayLoadDataStructure> list in _allDatas.Values)
            {
                foreach (SamplePayLoadDataStructure m in list)
                {
                    datas.Add(m);
                }
            }
            return datas;
        }


        private void AddData(ref Dictionary<string, List<SamplePayLoadDataStructure>> dataList, string userName, SamplePayLoadDataStructure data)
        {
            if (dataList.ContainsKey(userName))
            {
                dataList[userName].Add(data);
            }
            else
            {
                var newData = new List<SamplePayLoadDataStructure>();
                newData.Add(data);
                dataList.Add(userName, newData);
            }
        }

        private void RefreshDatas(ref Dictionary<string, List<SamplePayLoadDataStructure>> dataList, PayloadData[] datas)
        {
            dataList.Clear();
            foreach (var (userName, messageJson) in from d in datas
                                                    let userName = d.userName
                                                    let messageJson = d.ExtractData<SamplePayLoadDataStructure>()
                                                    select (userName, messageJson))
            {
                AddData(ref dataList, userName, messageJson);
            }
        }

        public void RefreshAllDatas(PayloadData[] datas)
        {
            RefreshDatas(ref _allDatas, datas);
        }

        public void UpdateAllDatasToGss(GssDbHub gssDbHub, Action feedback = null)
        {
            _isUpdating = true;
            gssDbHub.GetAllDatas((datas) => { GetAllDatasFeedBack(datas); feedback?.Invoke(); });
        }
        private void GetAllDatasFeedBack(PayloadData[] datas)
        {
            RefreshAllDatas(datas);
            _isUpdating = false;
        }
        private Dictionary<string, List<SamplePayLoadDataStructure>> GetNearPositionDatas(
            Vector3 targetPos, Func<Vector3, Vector3, bool> nearConditionFunc = null)
        {
            if (nearConditionFunc == null)
            {
                nearConditionFunc = IsTwoPositionsCloseEnough;
            }

            var nearPositionDatas = new Dictionary<string, List<SamplePayLoadDataStructure>>();
            foreach (var key in _allDatas.Keys)
            {
                nearPositionDatas.Add(key, _allDatas[key].Where(v => nearConditionFunc(v.position, targetPos)).ToList());
                if (nearPositionDatas[key].Count == 0)
                {
                    nearPositionDatas.Remove(key);
                }
            }
            return nearPositionDatas;
        }

        public Dictionary<string, List<SamplePayLoadDataStructure>> GetAllNearPositionDatas(
            Vector3 targetPos, Func<Vector3, Vector3, bool> nearConditionFunc = null)
        {
            return GetNearPositionDatas(targetPos, nearConditionFunc);
        }


        private bool IsTwoPositionsCloseEnough(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude < .8f;
        }

        public bool IsUpdating
        {
            get { return _isUpdating; }
        }

    }
}