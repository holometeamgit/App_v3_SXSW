using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constructor for Record AR window
/// </summary>
public class RecordARConstructor : WindowConstructor {
    public static Action OnShow = delegate { };
    public static Action OnHide = delegate { };

    protected void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    protected void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    protected void Show() {
        _window.SetActive(true);
    }

    protected void Hide() {
        _window.SetActive(false);
    }
}
