using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlEmailVerificationConstructor : MonoBehaviour {

    [SerializeField]
    private GameObject _pnlEmailVerification;

    public static Action<bool> _onActivated = delegate { };

    private void OnEnable() {
        _onActivated += Activate;
    }

    private void OnDisable() {
        _onActivated -= Activate;
    }

    private void Activate(bool status) {
        _pnlEmailVerification.SetActive(status);
    }
}
