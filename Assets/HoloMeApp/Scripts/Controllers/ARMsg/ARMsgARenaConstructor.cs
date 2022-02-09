using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.ARMsg;
using Beem.Permissions;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// Constructor for PnlARMessage
/// </summary>
public class ARMsgARenaConstructor : MonoBehaviour {
    [SerializeField]
    private PnlARMessages _pnlARMessages;

    public static Action<ARMsgJSON.Data> OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void Show(ARMsgJSON.Data data) {
        _pnlARMessages.Init(data);
    }

    private void Hide() {
        _pnlARMessages.Deactivate();
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
