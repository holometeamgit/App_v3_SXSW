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
using static CustomVideoPlayer;

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
    private Action<string> _onFailed;
    private Material _currentMat;

    private CustomVideoPlayer customVideoPlayer;

    //private const int DELAY = 1000;
    private const string LOADING = "Loading...";
    private const string LOADING_COMPLETED = "Loading completed";
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
    }

    private void OnEnable() {
        if (_data != null) {
            if (!string.IsNullOrEmpty(_data.ar_message_s3_link)) {
                if (customVideoPlayer == null) {
                    customVideoPlayer = new CustomVideoPlayer(_videoPlayer);
                }
                customVideoPlayer.PlayVideoFromURL(_data.ar_message_s3_link, OnChangeStatus);
                /*_videoPlayer.url = _data.ar_message_s3_link;
                if (!_videoPlayer.isPrepared) {
                    _onFailed?.Invoke(LOADING);
                    _videoPlayer.prepareCompleted += Prepare;
                    _videoPlayer.Prepare();
                } else {
                    _onSuccess?.Invoke();
                    UpdatePreview();
                }*/
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

    private void OnChangeStatus(Status status) {
        Debug.LogError($"status = {status}");

        switch (status) {
            case Status.ProcessLoading:
            case Status.ProcessPreparing:
                _onFailed?.Invoke(LOADING);
                break;
            case Status.FailLoading:
                _onFailed?.Invoke(FAILED);
                break;
            case Status.SuccessLoading:
                _onFailed?.Invoke(LOADING_COMPLETED);
                break;
            case Status.SuccessPreparing:
                _onSuccess?.Invoke();
                _image.texture = _videoPlayer?.texture;

                if (_currentMat == null) {
                    _currentMat = new Material(_greenScreenRemoverMat);
                    _image.material = _currentMat;
                }
                break;
        }
    }

    private void Prepare(VideoPlayer video) {
        _onSuccess?.Invoke();
        UpdatePreview();
        _videoPlayer.prepareCompleted -= Prepare;
    }

    private /*async*/ void UpdatePreview() {
        //_cancelTokenSource = new CancellationTokenSource();
        //CancellationToken cancellationToken = _cancelTokenSource.Token;

        _image.texture = _videoPlayer?.texture;

        if (_currentMat == null) {
            _currentMat = new Material(_greenScreenRemoverMat);
            _image.material = _currentMat;
        }

        _videoPlayer?.Play();
        //if (!cancellationToken.IsCancellationRequested) {
        //    await Task.Delay(DELAY);
        //    _videoPlayer?.Pause();
        //}
    }

    /// <summary>
    /// Clear Info
    /// </summary>
    /*public void Cancel() {
        if (_cancelTokenSource != null) {
            _cancelTokenSource.Cancel();
            _cancelTokenSource = null;
        }
    }*/

    private void OnDisable() {
        //Cancel();
        _videoPlayer.Stop();
    }

}
