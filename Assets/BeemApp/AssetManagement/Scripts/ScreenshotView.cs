using System;
using UnityEngine;
using UnityEngine.Events;
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
    private AspectRatioFitter _aspectRationFitter;
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

    [SerializeField]
    private bool _startState = true;

    private bool _currentState = default;


    /// <summary>
    /// Shot Preview
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFail"></param>
    public void Show(ARMsgJSON.Data data, Action onSuccess = null, Action<string> onFail = null) {
        _data = data;
        _onSuccess = onSuccess;
        _onFailed = onFail;
        Load();
    }

    private void Load() {
        if (_data != null) {
            if (!string.IsNullOrEmpty(_data.ar_message_s3_link)) {
                if (customVideoPlayer == null) {
                    customVideoPlayer = new CustomVideoPlayer(_videoPlayer, OnChangeStatus);
                }
                customVideoPlayer.LoadVideoFromURL(_data.ar_message_s3_link);
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

    /// <summary>
    /// Play
    /// </summary>
    public void Play() {
        if (customVideoPlayer != null) {
            customVideoPlayer.Play();
        }
    }

    /// <summary>
    /// Pause
    /// </summary>
    public void Pause() {
        if (customVideoPlayer != null) {
            customVideoPlayer.Pause();
        }
    }

    /// <summary>
    /// Play or Pause
    /// </summary>
    public void PlayOrPause() {
        _currentState = !_currentState;
        if (!_currentState) {
            Pause();
        } else {
            Play();
        }
    }

    private void OnChangeStatus(CustomVideoPlayer.Status status) {

        switch (status) {
            case CustomVideoPlayer.Status.Loading:
                _onFailed?.Invoke(LOADING);
                break;
            case CustomVideoPlayer.Status.Failed:
                _onFailed?.Invoke(FAILED);
                break;
            case CustomVideoPlayer.Status.Successed:
                _onSuccess?.Invoke();
                _image.texture = _videoPlayer.texture;
                _aspectRationFitter.aspectRatio = (float)_videoPlayer?.width / (float)_videoPlayer?.height;
                if (_currentMat == null) {
                    _currentMat = new Material(_greenScreenRemoverMat);
                    _image.material = _currentMat;
                }
                _currentState = _startState;
                if (_currentState) {
                    Play();
                }
                break;
        }
    }

    private void OnDisable() {
        if (customVideoPlayer != null) {
            customVideoPlayer.Stop();
        }
    }

}
