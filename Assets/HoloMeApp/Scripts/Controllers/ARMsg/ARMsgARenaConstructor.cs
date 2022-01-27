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

    private ARMsgJSON.Data currentData;

    public static Action<ARMsgJSON.Data> OnActivatedARena = delegate { };
    public static Action OnDeactivatedARena = delegate { };

    private void ActivateARena(ARMsgJSON.Data data) {
        currentData = data;
        _pnlARMessages.Init(data);
    }

    private void DeactivateARena() {
        WarningConstructor.ActivateDoubleButton("Before you go...",
           "If you exit you could lose your AR message if you don't share the link.",
           "Copy link and exit", "Return",
           () => {
               GUIUtility.systemCopyBuffer = currentData.share_link;
               Close();
           },
           null,
           false);

    }

    private void Close() {
        ARenaConstructor.onDeactivate?.Invoke();
        _pnlARMessages.Deactivate();
        MenuConstructor.OnActivated?.Invoke(true);
        HomeScreenConstructor.OnActivated?.Invoke(true);
    }


    private void OnEnable() {
        OnActivatedARena += ActivateARena;
        OnDeactivatedARena += DeactivateARena;
    }

    private void OnDisable() {
        OnActivatedARena -= ActivateARena;
        OnDeactivatedARena -= DeactivateARena;
    }
}
