using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;

public class PnlSettings : MonoBehaviour
{
    [SerializeField] GameObject _changePassword;
    [SerializeField] GameObject _btnStadium;
    [SerializeField] AccountManager _accountManager;
    [SerializeField] UserWebManager _userWebManager;
    [SerializeField] TMP_Text _txtNickname;

    private void OnEnable() {
        _changePassword.SetActive(_accountManager.GetLogInType() == LogInType.Email);
        _txtNickname.text = _userWebManager.GetUsername();

        _btnStadium.SetActive(_userWebManager.CanGoLive());
    }
}
