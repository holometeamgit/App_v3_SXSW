using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Constructor for PnlARMessage
/// </summary>
public class ARMsgARenaConstructor : MonoBehaviour {
    [SerializeField]
    private PnlARMessages _pnlARMessages;

    public static Action<ARMsgJSON.Data> OnShow = delegate { };
    public static Action OnHide = delegate { };

    public static bool IsActive;

    private void Show(ARMsgJSON.Data data) {
        IsActive = true;
        _pnlARMessages.Show(data);
    }

    private void Hide() {
        IsActive = false;
        _pnlARMessages.Hide();
    }

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }
}
