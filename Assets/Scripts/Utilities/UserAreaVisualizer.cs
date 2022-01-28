using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GssDbManageWrapper;

[RequireComponent(typeof(AreaDataManager))]
public class UserAreaVisualizer : MonoBehaviour
{
    AreaDataManager _areaDataManager;
    Transform _visualizedObjRoot;

    int _lastCount = 0;

    private void Start()
    {
        _visualizedObjRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        if (_areaDataManager == null) _areaDataManager = GetComponent<AreaDataManager>();
    }

    private void Update()
    {
        if (_areaDataManager != null && _lastCount != _areaDataManager._userCurrentArea.Count)
        {
            var transforms = _visualizedObjRoot.GetComponentsInChildren<Transform>();
            if (transforms != null)
            {
                foreach (var g in transforms)
                {
                    if (g == _visualizedObjRoot) continue;
                    Destroy(g.gameObject);
                }
            }


            foreach (MessageJson d in _areaDataManager._userCurrentArea)
            {
                GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
                a.transform.position = new Vector3(d.position.x, d.position.z, d.position.y);
                a.transform.localScale *= 10f;
                a.transform.SetParent(_visualizedObjRoot.transform);
            }
            _lastCount = _areaDataManager._userCurrentArea.Count;
        }

    }
}
