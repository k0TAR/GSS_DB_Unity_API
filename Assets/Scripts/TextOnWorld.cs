using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOnWorld : MonoBehaviour
{
    [SerializeField]
    private GameObject _cameraObject;
    [SerializeField]
    private Vector3    _position;
    // Start is called before the first frame update
    void Start()
    {
        var rectTransform        = GetComponent<RectTransform>();
        var screenPos            = _cameraObject.GetComponent<Camera>().WorldToScreenPoint(_position);
        screenPos.z              = 1.0f;
        rectTransform.position   = screenPos;
    }
    // Update is called once per frame
    void Update()
    {
        var rectTransform        = GetComponent<RectTransform>();
        var screenPos            = _cameraObject.GetComponent<Camera>().WorldToScreenPoint(_position);
        screenPos.z              = 1.0f;
        rectTransform.position   = screenPos;
    }
}
