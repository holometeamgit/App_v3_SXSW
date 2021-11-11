using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlResetPasswordConstructor : MonoBehaviour {

    [SerializeField]
    private GameObject _pnlResetPassword;

    public static Action<bool> _onActivated = delegate { };

    private void OnEnable() {
        _onActivated += Activate;
    }

    private void OnDisable() {
        _onActivated -= Activate;
    }

    private void Activate(bool status) {
        _pnlResetPassword.SetActive(status);
    }
}
