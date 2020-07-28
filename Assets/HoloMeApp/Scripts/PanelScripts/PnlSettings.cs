using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlSettings : MonoBehaviour
{
    [SerializeField] GameObject changePassword;
    [SerializeField] AccountManager accountManager;

    private void OnEnable() {
        changePassword.SetActive(accountManager.GetLoginType() == LogInType.Email);
    }
}
