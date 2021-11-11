using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlSignInEmailConstructor : MonoBehaviour {

    [SerializeField]
    private GameObject _pnlSignInEmail;

    public static Action<bool> _onActivated = delegate { };

    private void OnEnable() {
        _onActivated += Activate;
    }

    private void OnDisable() {
        _onActivated -= Activate;
    }

    private void Activate(bool status) {
        _pnlSignInEmail.SetActive(status);
    }
}
