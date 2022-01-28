using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

[RequireComponent(typeof(UserDataManager))]
[RequireComponent(typeof(AreaDataManager))]
public class PolyLineDataManager : MonoBehaviour
{
    AreaDataManager _areaDataManager;
    UserDataManager _userDataManager;

    private List<PolyLineData> _polyLineDatas = new List<PolyLineData>();

    private void Awake()
    {
        if (_areaDataManager == null) _areaDataManager = GetComponent<AreaDataManager>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();

    }
    void Update()
    {
        foreach (var polyLineData in _polyLineDatas)
        {
            polyLineData.ResetScorePosition();
        }
    }

    public Dictionary<string, HashSet<int>> GetAllAreaIdMap()
    {
        var allDatas = _areaDataManager._allDatas;
        var areaIdMap = new Dictionary<string, HashSet<int>>();
        foreach (var pair in allDatas)
        {
            if (!areaIdMap.ContainsKey(pair.Key))
            {
                areaIdMap[pair.Key] = new HashSet<int>();
            }
            foreach (var data in pair.Value)
            {
                areaIdMap[pair.Key].Add(data.areaId);
            }
        }
        return areaIdMap;
    }
    public Dictionary<string, List<(int, List<Vector3>)>> GetAllPolygonPositions()
    {
        var allAreaIdMap = GetAllAreaIdMap();
        var allPolygonVertices = new Dictionary<string, List<(int, List<Vector3>)>>();
        foreach (var pair in allAreaIdMap)
        {
            if (!allPolygonVertices.ContainsKey(pair.Key))
            {
                allPolygonVertices[pair.Key] = new List<(int, List<Vector3>)>();
            }
            foreach (var areaId in pair.Value)
            {
                var polygonVertices = _areaDataManager.GetAreaVerticies(pair.Key, areaId);
                allPolygonVertices[pair.Key].Add((areaId, polygonVertices));
            }
        }
        return allPolygonVertices;
    }
    public void UpdatePolyLineDatas()
    {
        var allPolygonPositions = GetAllPolygonPositions();
        foreach (var data in allPolygonPositions)
        {
            var userName = data.Key;
            var userData = _userDataManager.GetUserData(userName);
            foreach (var (areaId, polygonPositions) in data.Value)
            {
                bool sameData = false;
                foreach (var polyLineData in _polyLineDatas)
                {
                    if (polyLineData._userData == userData && polyLineData._areaId == areaId)
                    {
                        sameData = true;
                        break;
                    }
                }

                if (!sameData)
                {
                    List<Vector3> positions = new List<Vector3>();
                    foreach (var d in polygonPositions)
                    {
                        var position = new Vector3(d.x, d.z, d.y);
                        position = new Vector3(position.x, 10.0f, position.z);
                        positions.Add(position);
                    }
                    positions.Add(positions[0]);

                    _polyLineDatas.Add(new PolyLineData(userData, areaId, positions));
                }
            }
        }
        int areaIdx = 0;
        List<int> removeIndices = new List<int>();
        foreach (var polyLineData in _polyLineDatas)
        {
            bool sameData = false;
            foreach (var data in allPolygonPositions)
            {
                var userName = data.Key;
                var userData = _userDataManager.GetUserData(userName);
                foreach (var (areaId, polygonPositions) in data.Value)
                {
                    if (polyLineData._userData == userData && polyLineData._areaId == areaId)
                    {
                        sameData = true;
                        break;
                    }
                }
            }
            if (!sameData)
            {
                Debug.Log("Remove Local " + areaIdx.ToString());
                removeIndices.Add(areaIdx);
            }
            ++areaIdx;
        }
        {
            int removeOffset = 0;
            foreach (var removeIndex in removeIndices)
            {
                var polyLineData = _polyLineDatas[removeIndex - removeOffset];
                _polyLineDatas.Remove(polyLineData);
                polyLineData.RefreshPolyLine();
                ++removeOffset;
            }
        }
    }
}
