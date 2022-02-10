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
    public static bool IsActive;

    protected void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    protected void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    protected void Show() {
        IsActive = true;
        _window.SetActive(true);
    }

    protected void Hide() {
        IsActive = false;
        _window.SetActive(false);
    }
}
