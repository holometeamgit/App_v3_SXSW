using UnityEngine;
using Beem.ARMsg;
using System;
using Zenject;

/// <summary>
/// ARMsgConstructor. TODO convert it to DI in future 
/// </summary>
public class ARMsgRecordConstructor : WindowConstructor {
    public static Action<bool> OnActivated = delegate { };
    protected void OnEnable() {
        OnActivated += Activate;
    }

    protected void OnDisable() {
        OnActivated -= Activate;
    }

    private void Activate(bool status) {
        _window?.SetActive(status);
    }
}
