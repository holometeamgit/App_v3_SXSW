using System;
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

    private ARMsgJSON.Data _data;
    private Action _onSuccess;
    private Action<string> _onFailed;
    private Material _currentMat;
    private CustomVideoPlayer customVideoPlayer;

    private const string LOADING = "Loading...";
    private const string PROCESSING = "Processing...";
    private const string FAILED = "Failed...";


    /// <summary>
    /// Shot Preview
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    public void Show(ARMsgJSON.Data data, Action onSuccess, Action<string> onFail) {
        _data = data;
        _onSuccess = onSuccess;
        _onFailed = onFail;
        Play();
    }

    private void Play() {
        if (_data != null) {
            if (!string.IsNullOrEmpty(_data.ar_message_s3_link)) {
                if (customVideoPlayer == null) {
                    customVideoPlayer = new CustomVideoPlayer(_videoPlayer);
                }
                customVideoPlayer.LoadVideoFromURL(_data.ar_message_s3_link, OnChangeStatus);
            } else {
                if (_data.processing_status == ARMsgJSON.Data.FAILED_STATUS) {
                    _onFailed?.Invoke(FAILED);
                } else {
                    _onFailed?.Invoke(PROCESSING);
                }

            }
        } else {
            _onFailed?.Invoke(FAILED);
        }
    }

    private void OnChangeStatus(CustomVideoPlayer.Status status) {

        switch (status) {
            case CustomVideoPlayer.Status.ProcessLoading:
            case CustomVideoPlayer.Status.ProcessPreparing:
            case CustomVideoPlayer.Status.SuccessLoading:
                _onFailed?.Invoke(LOADING);
                break;
            case CustomVideoPlayer.Status.FailLoading:
                _onFailed?.Invoke(FAILED);
                break;
            case CustomVideoPlayer.Status.SuccessPreparing:
                _onSuccess?.Invoke();
                _image.texture = _videoPlayer?.texture;

                HelperFunctions.DevLogError($"width = {_videoPlayer.width}, height = {_videoPlayer.height},  _image.size = {_image.GetComponent<RectTransform>().sizeDelta}");
                if (_currentMat == null) {
                    _currentMat = new Material(_greenScreenRemoverMat);
                    _image.material = _currentMat;
                }
                break;
        }
    }

    private void OnDisable() {
        if (customVideoPlayer != null) {
            _videoPlayer.Stop();
        }
    }

}
