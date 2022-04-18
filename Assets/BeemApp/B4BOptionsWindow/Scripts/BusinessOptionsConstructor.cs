using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Business Options Constructor
/// </summary>
public class BusinessOptionsConstructor : MonoBehaviour {

    [SerializeField]
    private BusinessOptionsWindow _businessOptionsView;

    public static Action<ARMsgJSON.Data> OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show(ARMsgJSON.Data data) {
        _businessOptionsView.Show(data);
    }

    private void Hide() {
        _businessOptionsView.Hide();
    }
}
