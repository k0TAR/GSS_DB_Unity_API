using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GssDbManageWrapper
{
    [System.Serializable]
    public class UserData
    {
        public string _userName;
        public Color _color;

        public UserData(string userName, Color color)
        {
            _userName = userName;
            _color = color;
        }
    }

    public class UserDataManager : MonoBehaviour
    {
        public string LocalPlayerName
        {
            get => _localPlayer._userName;
            set => _localPlayer._userName = value;
        }

        [SerializeField]
        public UserData _localPlayer = null;

        private bool _isUpdating = false;

        public HashSet<string> _userNames = new HashSet<string>();
        public HashSet<UserData> _userDatas = new HashSet<UserData>();


        private void Start()
        {
            if (_localPlayer == null)
            {
                Debug.Log($"<color=red>[UserDataManager]</color> " +
                    $"{nameof(_localPlayer)} is null.");
            }
        }

        public Color RandomColor()
        {
            bool colorExists = true;
            Color randomColor = Color.white;
            while (colorExists)
            {
                colorExists = false;
                randomColor = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.3f, 1f);
                randomColor.a = 1.0f;

                foreach (var u in _userDatas)
                {
                    if (IsColorClose(u._color, randomColor))
                    {
                        colorExists = true;
                        continue;
                    }
                }
            }

            return randomColor;
        }

        private bool IsColorClose(Color aa, Color bb)
        {
            float r = Mathf.Abs(aa.r - bb.r);
            float g = Mathf.Abs(aa.g - bb.g);
            float b = Mathf.Abs(aa.b - bb.b);
            return (r + g + b) < .15f;
        }

        public void AddUserData(UserData userData)
        {
            _userDatas.Add(userData);
        }

        public UserData GetUserData(string userName)
        {
            foreach (var u in _userDatas)
            {
                if (u._userName == userName)
                {
                    return u;
                }
            }

            return null;
        }

        public void AddUserName(string userName)
        {
            _userNames.Add(userName);
        }

        public void UpdateDatas(PayloadData[] datas)
        {
            foreach (var d in datas)
            {
                var userName = d.userName;
                var color = JsonUtility.FromJson<Color>(d.data);

                if (!_userNames.Contains(userName))
                {
                    AddUserName(userName);
                    AddUserData(new UserData(d.userName, color));
                }
            }
        }

        public void UpdateAllUserNamesToGss(GssDbHub gssDbHub, System.Action feedback = null)
        {
            _isUpdating = true;
            gssDbHub.GetUserDatas((datas) => { GetUserNamesFeedBack(datas); feedback?.Invoke(); });
        }
        private void GetUserNamesFeedBack(PayloadData[] datas)
        {
            UpdateDatas(datas);
            _isUpdating = false;
        }

        public bool IsUpdating
        {
            get { return _isUpdating; }
        }

    }
}

