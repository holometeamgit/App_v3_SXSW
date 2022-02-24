using Beem.UI;
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

    private CancellationTokenSource cancelTokenSource;

    private const int DELAY = 1000;

    private Action _onSuccess;
    private Action _onFailed;

    /// <summary>
    /// Shot Preview
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    public void Show(ARMsgJSON.Data data, Action onSuccess, Action onFail) {
        _videoPlayer.url = data.ar_message_s3_link;
        _onSuccess = onSuccess;
        _onFailed = onFail;
    }

    private void OnEnable() {
        cancelTokenSource = new CancellationTokenSource();
        if (!_videoPlayer.isPrepared) {
            _onFailed?.Invoke();
            _videoPlayer.prepareCompleted += Prepare;
            _videoPlayer.Prepare();
        } else {
            _onSuccess?.Invoke();
            UpdatePreview();
        }
    }

    private void Prepare(VideoPlayer video) {
        _onSuccess?.Invoke();
        UpdatePreview();
        _videoPlayer.prepareCompleted -= Prepare;
    }

    private async void UpdatePreview() {
        CancellationToken cancellationToken = cancelTokenSource.Token;

        _image.texture = _videoPlayer.texture;

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
        if (cancelTokenSource != null) {
            cancelTokenSource.Cancel();
            cancelTokenSource = null;
        }
    }

    private void OnDisable() {
        Cancel();
        _videoPlayer.Stop();
    }

}
