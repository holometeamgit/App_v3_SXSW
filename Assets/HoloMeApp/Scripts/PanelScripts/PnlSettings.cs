using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using TMPro;

public class PnlSettings : MonoBehaviour
{
    [SerializeField] GameObject changePassword;
    [SerializeField] AccountManager accountManager;
    [SerializeField] UserWebManager userWebManager;
    [SerializeField] TMP_Text txtNickname;

    private void OnEnable() {
        changePassword.SetActive(accountManager.GetLogInType() == LogInType.Email);
        txtNickname.text = userWebManager.GetUsername();
    }
}
