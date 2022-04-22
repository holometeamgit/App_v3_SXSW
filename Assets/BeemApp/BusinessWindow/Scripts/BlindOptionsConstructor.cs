using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Blind Options Constructor
/// </summary>
public class BlindOptionsConstructor : MonoBehaviour {

    [SerializeField]
    private BlindOptionsWindow _blindOptionsWindow;

    public static Action<string, object[]> OnShow = delegate { };
    public static Action OnHide = delegate { };

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show(string assetId, params object[] objects) {
        _blindOptionsWindow.Show(assetId, objects);
    }

    private void Hide() {
        _blindOptionsWindow.Hide();
    }
}
