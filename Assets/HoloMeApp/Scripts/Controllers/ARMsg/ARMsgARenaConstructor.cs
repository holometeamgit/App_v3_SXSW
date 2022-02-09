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

    public static Action<ARMsgJSON.Data> OnActivatedARena = delegate { };
    public static Action OnDeactivatedARena = delegate { };

    private void ActivateARena(ARMsgJSON.Data data) {
        _pnlARMessages.Init(data);
    }

    private void DeactivateARena() {
        _pnlARMessages.Deactivate();
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
