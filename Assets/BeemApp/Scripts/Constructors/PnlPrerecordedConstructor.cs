using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlPrerecordedConstructor : MonoBehaviour {

    [SerializeField]
    private PnlPrerecordedVideo _pnlPrerecordedVideo;

    [SerializeField]
    private PrerecordedVideoBar _prerecordedVideoBar;

    public static Action<StreamJsonData.Data> _onActivated = delegate { };

    public static Action _onDeactivated = delegate { };

    private void OnEnable() {
        _onActivated += Activate;
        _onDeactivated += Deactivate;
    }

    private void OnDisable() {
        _onActivated -= Activate;
        _onDeactivated -= Deactivate;
    }

    private void Activate(StreamJsonData.Data streamData) {
        _pnlPrerecordedVideo.Init(streamData);
        _prerecordedVideoBar.Init(streamData);
    }

    private void Deactivate() {
        _pnlPrerecordedVideo.Deactivate();
        _prerecordedVideoBar.Deactivate();
    }
}
