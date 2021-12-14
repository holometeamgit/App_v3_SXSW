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

    [Header("Prerecorded Video Bar")]
    [SerializeField]
    private PrerecordedVideoBar _prerecordedVideoBar;

    public static Action<StreamJsonData.Data> OnActivated = delegate { };

    public static Action OnDeactivated = delegate { };

    private void OnEnable() {
        OnActivated += Activate;
        OnDeactivated += Deactivate;
    }

    private void OnDisable() {
        OnActivated -= Activate;
        OnDeactivated -= Deactivate;
    }

    private void Activate(StreamJsonData.Data streamData) {
        _prerecordedVideoWindow.Init(streamData);
        _prerecordedVideoBar.Init(streamData);
    }

    private void Deactivate() {
        _prerecordedVideoWindow.Deactivate();
        _prerecordedVideoBar.Deactivate();
    }
}
