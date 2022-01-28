using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMover : MonoBehaviour
{
    [SerializeField] Camera _camera;
    private Transform _mainCameraTransform;
    [SerializeField]
    [Range(0f, 1f)]
    private float _speed = .04f;
    private bool _isSwiping;
    private Vector2 _startPos, _currentPos, _diffSwipeVec2;
    void Start()
    {
        if (_camera == null) _camera = Camera.main;
        _mainCameraTransform = _camera.transform;
    }

    void Update()
    {
        MovementControll();
    }

    void FixedUpdate()
    {
        MoveMap();
    }

    void MovementControll()
    {
        //移動
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //マウス左クリック時に始点座標を代入
            _startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _isSwiping = true;

            //押している最中に今の座標を代入
            _currentPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _diffSwipeVec2 = _currentPos - _startPos;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _isSwiping = false;
        }
    }

    void MoveMap()
    {
        if (_isSwiping == true)
        {
            _mainCameraTransform.position -= new Vector3(
                _diffSwipeVec2.x, 0, _diffSwipeVec2.y) * _speed;
        }
    }

}
