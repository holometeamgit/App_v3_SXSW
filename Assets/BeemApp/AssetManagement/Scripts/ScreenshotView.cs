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
    }

    private void OnEnable() {
        if (_data != null && _data.processing_status == ARMsgJSON.Data.COMPETED_STATUS) {
            if (_currentMat == null) {
                _currentMat = new Material(_greenScreenRemoverMat);
            }
            _videoPlayer.url = _data.ar_message_s3_link;
            _cancelTokenSource = new CancellationTokenSource();
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

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Debug.LogError($"source = {source.name}, destination = {destination.name}");
    }

    private void Prepare(VideoPlayer video) {
        _onSuccess?.Invoke();
        UpdatePreview();
        _videoPlayer.prepareCompleted -= Prepare;
    }

    private async void UpdatePreview() {
        CancellationToken cancellationToken = _cancelTokenSource.Token;

        _image.texture = _videoPlayer?.texture;
        _image.material = _currentMat;

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
