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

    public static event Action<string, object[]> OnShow = delegate { };
    public static event Action OnHide = delegate { };

    private void OnEnable() {
        OnShow += ShowView;
        OnHide += HideView;
    }

    private void OnDisable() {
        OnShow -= ShowView;
        OnHide -= HideView;
    }

    /// <summary>
    /// Show
    /// </summary>
    /// <param name="assetId"></param>
    /// <param name="objects"></param>
    public static void Show(string assetId, params object[] objects) {
        OnShow?.Invoke(assetId, objects);
    }

    private void ShowView(string assetId, params object[] objects) {
        _blindOptionsWindow.Show(assetId, objects);
    }

    /// <summary>
    /// Hide
    /// </summary>
    public static void Hide() {
        OnHide?.Invoke();
    }

    private void HideView() {
        _blindOptionsWindow.Hide();
    }
}
