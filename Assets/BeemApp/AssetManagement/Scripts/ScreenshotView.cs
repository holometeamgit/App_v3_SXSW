using Beem.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Screenshot View
/// </summary>
public class ScreenshotView : MonoBehaviour, IARMsgDataView {
    [SerializeField]
    private VideoPlayer _videoPlayer;
    [SerializeField]
    private RawImage _image;
    [SerializeField]
    private RectTransform _rectTransform;
    [SerializeField]
    private int _maxHeight = 345;

    private RenderTexture rt;
    private int videoWidth;
    private int videoHeight;

    private const int DELAY = 100;

    private void Init() {
        if (rt == null) {
            AgoraSharedVideoConfig.GetResolution(screenWidth: Screen.width, screenHeigh: Screen.height, out videoWidth, out videoHeight, _maxHeight);
            rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            rt.Create();
            _videoPlayer.targetTexture = rt;
            _image.texture = rt;
            _rectTransform.sizeDelta = new Vector2(videoWidth, videoHeight);
        }
    }

    /*/// <summary>
    /// Shot Preview
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    public void Show(ARMsgJSON.Data data, Action onSuccess, Action onFail) {

        Init();

        _videoPlayer.url = data.ar_message_s3_link;

        if (!_videoPlayer.isPrepared) {
            onFail?.Invoke();
            _videoPlayer.Prepare();
            _videoPlayer.prepareCompleted +=
                (player) => {
                    onSuccess?.Invoke();
                    UpdatePreview();
                };
        } else {
            onSuccess?.Invoke();
            UpdatePreview();
        }
    }*/

    private async void UpdatePreview() {
        _videoPlayer.Play();
        await Task.Delay(DELAY);
        _videoPlayer.Stop();
    }

    public void Init(ARMsgJSON.Data data) {
        Init();

        _videoPlayer.url = data.ar_message_s3_link;

        if (!_videoPlayer.isPrepared) {
            _videoPlayer.Prepare();
            _videoPlayer.prepareCompleted +=
                (player) => {
                    UpdatePreview();
                };
        } else {
            UpdatePreview();
        }
    }
}
