using System;
using UnityEngine;
/// <summary>
/// Constructor for HomeScreen
/// </summary>
public class BottomMenuConstructor : WindowConstructor {
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
