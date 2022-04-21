using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Success Options Constructor
/// </summary>
public class SuccessOptionsConstructor : MonoBehaviour {

    [SerializeField]
    private SuccessOptionsWindow _successOptionsWindow;

    public static Action OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show() {
        _successOptionsWindow.Show();
    }

    private void Hide() {
        _successOptionsWindow.Hide();
    }
}
