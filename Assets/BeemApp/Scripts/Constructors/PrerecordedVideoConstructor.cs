using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constructor For Prerecorded Video
/// </summary>
public class PrerecordedVideoConstructor : MonoBehaviour {

    [Header("Prerecorded Video Window")]
    [SerializeField]
    private PrerecordedVideoWindow _prerecordedVideoWindow;

    public static Action<StreamJsonData.Data> OnShow = delegate { };

    public static Action OnHide = delegate { };

    public static bool IsActive;

    private void OnEnable() {
        OnShow += Show;
        OnHide += Hide;
    }

    private void OnDisable() {
        OnShow -= Show;
        OnHide -= Hide;
    }

    private void Show(StreamJsonData.Data data) {
        IsActive = true;
        _prerecordedVideoWindow.Show(data);
    }

    private void Hide() {
        IsActive = false;
        _prerecordedVideoWindow.Hide();
    }
}
