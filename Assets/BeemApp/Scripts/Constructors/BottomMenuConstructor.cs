using System;
using UnityEngine;
/// <summary>
/// Constructor for HomeScreen
/// </summary>
public class BottomMenuConstructor : WindowConstructor {

    public static Action<bool> OnActivated = delegate { };

    protected void OnEnable() {
        OnActivated += Activate;
    }

    protected void OnDisable() {
        OnActivated -= Activate;
    }

    protected void Activate(bool status) {
        _window.SetActive(status);
    }
}
