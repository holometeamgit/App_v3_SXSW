using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

public class PnlSettings : MonoBehaviour
{
    [SerializeField] GameObject changePassword;
    [SerializeField] AccountManager accountManager;

    private void OnEnable() {
        changePassword.SetActive(accountManager.GetLogInType() == LogInType.Email);
    }
}
