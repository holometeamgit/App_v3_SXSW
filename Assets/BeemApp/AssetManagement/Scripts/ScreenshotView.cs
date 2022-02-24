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
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private int _maxHeight = 345;

    private CancellationTokenSource cancelTokenSource;
    //private RenderTexture rt;
    private int videoWidth;
    private int videoHeight;

    private const int DELAY = 1000;

    private Action _onSuccess;
    private Action _onFailed;

    /*private void Init() {
        if (rt == null) {
            rt = new RenderTexture(_maxHeight, _maxHeight, 16, RenderTextureFormat.ARGB32);
            rt.Create();
            _videoPlayer.targetTexture = rt;
            _image.texture = rt;
        }
    }*/

    /// <summary>
    /// Shot Preview
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    public void Show(ARMsgJSON.Data data, Action onSuccess, Action onFail) {
        //Init();

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

        HelperFunctions.DevLogError($"width = {_videoPlayer.width}, height = {_videoPlayer.height}");
        HelperFunctions.DevLogError($"pixelAspectRatioNumerator = {_videoPlayer.pixelAspectRatioNumerator}, pixelAspectRatioDenominator = {_videoPlayer.pixelAspectRatioDenominator}");

        AgoraSharedVideoConfig.GetResolution(screenWidth: (int)_videoPlayer.pixelAspectRatioNumerator, screenHeigh: (int)_videoPlayer.pixelAspectRatioDenominator, out videoWidth, out videoHeight, _maxHeight);
        _rectTransform.sizeDelta = new Vector2(videoWidth, videoHeight);
        _videoPlayer.Play();
        if (!cancellationToken.IsCancellationRequested) {
            await Task.Delay(DELAY);
        }

        HelperFunctions.DevLogError($"2width = {_videoPlayer.width}, height = {_videoPlayer.height}");
        HelperFunctions.DevLogError($"2pixelAspectRatioNumerator = {_videoPlayer.pixelAspectRatioNumerator}, pixelAspectRatioDenominator = {_videoPlayer.pixelAspectRatioDenominator}");

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
