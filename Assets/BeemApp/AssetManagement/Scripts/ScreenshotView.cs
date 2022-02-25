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
    //[SerializeField]
    //private Material _customMaterial;

    //private Material _currentMaterial;
    private CancellationTokenSource _cancelTokenSource;
    private ARMsgJSON.Data _data;
    private Action _onSuccess;
    private Action _onFailed;

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
        if (_data.processing_status == ARMsgJSON.Data.COMPETED_STATUS) {
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

    private void Prepare(VideoPlayer video) {
        _onSuccess?.Invoke();
        UpdatePreview();
        _videoPlayer.prepareCompleted -= Prepare;
    }

    private async void UpdatePreview() {
        CancellationToken cancellationToken = _cancelTokenSource.Token;

        _image.texture = _videoPlayer.texture;

        //if (_currentMaterial == null) {
        //    _currentMaterial = Instantiate(_customMaterial);
        //}

        //_image.material = _currentMaterial;

        _videoPlayer.Play();
        if (!cancellationToken.IsCancellationRequested) {
            await Task.Delay(DELAY);
        }

        _videoPlayer.Pause();
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
