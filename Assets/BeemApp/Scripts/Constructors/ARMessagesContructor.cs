using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constructor for AR messages panel
/// </summary>
public class ARMessagesContructor : MonoBehaviour {
    [SerializeField]
    private PnlARMessages pnlARMessages;

    public static Action<StreamJsonData.Data> onActivated = delegate { };

    public static Action onDeactivated = delegate { };

    private void OnEnable() {
        onActivated += Activate;
        onDeactivated += Deactivate;
    }

    private void OnDisable() {
        onActivated -= Activate;
        onDeactivated -= Deactivate;
    }

    private void Activate(StreamJsonData.Data data) {
        pnlARMessages.Init(data);
    }

    private void Deactivate() {
        pnlARMessages.Deactivate();
    }

}
