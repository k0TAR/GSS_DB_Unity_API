using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GssDbManageWrapper;

[RequireComponent(typeof(GssDbHub))]
[RequireComponent(typeof(UserDataManager))]
[RequireComponent(typeof(LonLatGetter))]
public class PlayerSettingUIManager : MonoBehaviour
{
    [SerializeField]
    public InputField _playerNameField;
    [SerializeField]
    private Text _playerNameVisualizeText;

    [SerializeField]
    private Text _playerValidation;
    [SerializeField]
    private GameObject _playerValidationBG;

    [SerializeField]
    private GameObject _nextSceneButton;
    [SerializeField]
    private GameObject _checkUserButton;

    [SerializeField]
    private Color _plusColor;
    [SerializeField]
    private Color _minusColor;

    private GssDbHub _gssDbHub;
    private UserDataManager _userDataManager;
    private LonLatGetter _lonLatGetter;
    private bool _playerNameValidanceCheck = false;
    private bool _runValidation = false;
    private bool _checkButtonPressed = false;

    private void Awake()
    {
        if (_gssDbHub == null) _gssDbHub = GetComponent<GssDbHub>();
        if (_userDataManager == null) _userDataManager = GetComponent<UserDataManager>();
        if (_lonLatGetter == null) _lonLatGetter = GetComponent<LonLatGetter>();

        _playerNameField.text = "";
        _playerValidationBG.SetActive(false);

        _nextSceneButton.SetActive(false);
        _playerNameField.readOnly = false;
        _checkUserButton.SetActive(false);

        UpdateUserDataList();
    }

    private void Update()
    {
        /*
        if(_runValidation)
        {
            _gssDbHub.CheckIfPlayerNameValid(
                _playerNameField.text, UpdatePlayerNameRelatedUI);
            _runValidation = false;
        }


        if(_playerNameValidanceCheck)
        {
            StartCoroutine(PlayerNameValidationUIEffect());
            _playerNameValidanceCheck = false;
        }*/

        if (!_userDataManager.IsUpdating && !_checkButtonPressed && !string.IsNullOrWhiteSpace(_playerNameField.text))
        {
            _checkUserButton.SetActive(true);
        }

        if (_lonLatGetter.CanGetLonLat() && _checkButtonPressed)
        {
            _nextSceneButton.SetActive(true);
        }
    }

    public void CheckUser()
    {
        bool localPlayerExists = false;
        foreach (var d in _userDataManager._userDatas)
        {
            if (d._userName == _playerNameField.text)
            {
                localPlayerExists = true;
                _userDataManager._localPlayer = d;
            }
        }

        if (!localPlayerExists)
        {
            _userDataManager._localPlayer = new UserData(_playerNameField.text, _userDataManager.RandomColor());
            _gssDbHub.SetUserData(_userDataManager._localPlayer, data => { _checkButtonPressed = true; });

        }
        else
        {
            _checkButtonPressed = true;
        }

        _playerNameField.readOnly = true;
        _checkUserButton.SetActive(false);
    }

    private void UpdateUserDataList()
    {
        _userDataManager.UpdateAllUserNamesToGss(_gssDbHub);

    }

    IEnumerator PlayerNameValidationUIEffect()
    {
        yield return new WaitForSeconds(3.0f);
        _playerValidation.text = "";
        _playerValidationBG.SetActive(false);
    }

    private void UpdatePlayerNameRelatedUI(bool isPlayerNameValid, string playerName)
    {
        if (isPlayerNameValid)
        {
            _playerNameVisualizeText.text = "\n" + playerName;
            _nextSceneButton.SetActive(true);

            _playerValidationBG.SetActive(true);
            _playerValidation.text = $"The Player Name: {playerName} is valid";
            _playerValidation.color = _plusColor;
            _playerNameValidanceCheck = true;
        }
        else
        {
            if (_playerNameField.text == "")
            {
                _playerNameVisualizeText.text = "\nNo Player Name is set.";
            }

            _playerValidationBG.SetActive(true);
            _playerValidation.text = $"The Player Name: {playerName} is inValid";
            _playerValidation.color = _minusColor;
            _playerNameValidanceCheck = true;
        }
    }

    public void RunPlayerNameValidation()
    {
        if (!string.IsNullOrEmpty(_playerNameField.text))
        {
            _runValidation = true;
        }
    }
}
