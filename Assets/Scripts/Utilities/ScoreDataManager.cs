using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GssDbManageWrapper;
[RequireComponent(typeof(UserDataManager))]
public class ScoreDataManager : MonoBehaviour
{
    UserDataManager _userDataManager;
    PolyLineDataManager _polyLineDataManager;
    private Transform _textBGTransform;
    Dictionary<string, GameObject> _scoreObjects;
    // Start is called before the first frame update
    void Start()
    {
        if (_userDataManager == null)
        {
            _userDataManager = GetComponent<UserDataManager>();
        }
        if (_polyLineDataManager == null)
        {
            _polyLineDataManager = GetComponent<PolyLineDataManager>();
        }
        if (_textBGTransform == null)
        {
            _textBGTransform = GameObject.Find("TEXT_BG").transform;
        }
        _scoreObjects = new Dictionary<string, GameObject>();
    }
    // Update is called once per frame
    void Update()
    {
        var userNameSet = new HashSet<string>();
        foreach (var userName in _userDataManager._userNames)
        {
            userNameSet.Add(userName);
        }
        var removeUserNames = new List<string>();
        {
            foreach (var userName in _scoreObjects.Keys)
            {
                if (!userNameSet.Contains(userName))
                {
                    removeUserNames.Add(userName);
                }
            }
            foreach (var userName in removeUserNames)
            {
                var scoreObject = _scoreObjects[userName];
                _scoreObjects.Remove(userName);
                Destroy(scoreObject);
            }
        }
        foreach (var userName in _userDataManager._userNames)
        {
            if (!_scoreObjects.ContainsKey(userName))
            {
                var userData = _userDataManager.GetUserData(userName);
                var scoreObject = new GameObject();
                scoreObject.transform.SetParent(_textBGTransform);
                scoreObject.transform.name = "scoreBoard " + userName;
                scoreObject.transform.localScale = new Vector3(1, 1, 1);
                scoreObject.transform.localPosition = new Vector3(0, 0, 0);
                var scoreText = scoreObject.AddComponent<Text>();
                scoreText.text = userName + ": 0";
                scoreText.rectTransform.position = Vector3.zero;
                scoreText.rectTransform.offsetMin = new Vector2(0, 0);
                scoreText.rectTransform.offsetMax = new Vector2(0, 0);
                scoreText.rectTransform.anchorMin = new Vector3(0.01f, 0.01f, 0.01f);
                scoreText.rectTransform.anchorMax = new Vector3(0.99f, 0.99f, 0.99f);
                scoreText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                scoreText.fontSize = 60;
                scoreText.color = userData._color;
                _scoreObjects.Add(userName, scoreObject);
            }
        }
        {
            var allPolygonPositions = _polyLineDataManager.GetAllPolygonPositions();
            var offset = 0.0f;
            foreach (var item in _scoreObjects)
            {
                var score = 0.0f;
                if (allPolygonPositions.ContainsKey(item.Key))
                {
                    foreach (var (areaId, polygon) in allPolygonPositions[item.Key])
                    {
                        score += Mathf.Abs(CalcSignedArea(polygon));
                    }
                }
                item.Value.GetComponent<Text>().text = item.Key + ": " + score.ToString("0");
                item.Value.GetComponent<RectTransform>().offsetMax = new Vector2(0, -offset);
                offset += 60.0f;
            }
        }
    }
    static float CalcSignedArea(List<Vector3> points)
    {
        float signedArea = 0.0f;
        for (var i = 0; i < points.Count; ++i)
        {
            var p_i = points[i];
            var p_i_1 = (i == points.Count - 1) ? points[0] : points[i + 1];
            signedArea += (p_i.x * p_i_1.y - p_i.y * p_i_1.x);
        }
        return signedArea * 0.5F;
    }
}
