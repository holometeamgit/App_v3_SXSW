using Beem.UI;
using HoloMeSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Screenshot View
/// </summary>
public class ScreenshotView : MonoBehaviour {
    [SerializeField]
    private VideoPlayer _videoPlayer;
    [SerializeField]
    private RawImage _image;
    [SerializeField]
    private Material _greenScreenRemoverMat;

    private CancellationTokenSource _cancelTokenSource;
    private ARMsgJSON.Data _data;
    private Action _onSuccess;
    private Action _onFailed;
    private Material _currentMat;

    private const int DELAY = 1000;


    /// <summary>
    /// Shot Preview
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    public void Show(ARMsgJSON.Data data, Action onSuccess, Action onFail) {
        _data = data;
        _onSuccess = onSuccess;
        _onFailed = onFail;
        if (gameObject.activeInHierarchy) {
            Activate();
        }
    }

    private void Activate() {
        if (_data != null && !string.IsNullOrEmpty(_data.ar_message_s3_link)) {
            _videoPlayer.url = _data.ar_message_s3_link;
            if (!_videoPlayer.isPrepared) {
                _onFailed?.Invoke();
                _videoPlayer.prepareCompleted += Prepare;
                _videoPlayer.Prepare();
            } else {
                _onSuccess?.Invoke();
                UpdatePreview();
            }
        } else {
            _onFailed?.Invoke();
        }
    }

    private void OnEnable() {
        Activate();
    }

    private void Prepare(VideoPlayer video) {
        _onSuccess?.Invoke();
        UpdatePreview();
        _videoPlayer.prepareCompleted -= Prepare;
    }

    private async void UpdatePreview() {
        _cancelTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = _cancelTokenSource.Token;

        _image.texture = _videoPlayer?.texture;

        if (_currentMat == null) {
            _currentMat = new Material(_greenScreenRemoverMat);
            _image.material = _currentMat;
        }

        _videoPlayer?.Play();
        if (!cancellationToken.IsCancellationRequested) {
            await Task.Delay(DELAY);
            _videoPlayer?.Pause();
        }
    }

    /// <summary>
    /// Clear Info
    /// </summary>
    public void Cancel() {
        if (_cancelTokenSource != null) {
            _cancelTokenSource.Cancel();
            _cancelTokenSource = null;
        }
    }

    private void OnDisable() {
        Cancel();
        _videoPlayer.Stop();
    }

}
