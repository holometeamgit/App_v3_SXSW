using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

/// <summary>
/// Custom Video Player
/// </summary>
public class CustomVideoPlayer {

    public enum Status {
        ProcessLoading,
        FailLoading,
        SuccessLoading,
        ProcessPreparing,
        SuccessPreparing
    }

    private CancellationTokenSource _cancelTokenSource;
    private VideoPlayer _videoPlayer;
    private UnityWebRequest _videoRequest;
    public CustomVideoPlayer(VideoPlayer videoPlayer) {
        _videoPlayer = videoPlayer;
    }


    /// <summary>
    /// Play Video
    /// </summary>
    /// <param name="_url"></param>
    public async void PlayVideoFromURL(string _url, Action<Status> onChangeStatus) {
        Cancel();
        _cancelTokenSource = new CancellationTokenSource();
        CancellationToken cancelTokenSource = _cancelTokenSource.Token;
        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = _url;
        _videoPlayer.Prepare();
        onChangeStatus?.Invoke(Status.ProcessPreparing);
        while (_videoPlayer.isPrepared == false && !cancelTokenSource.IsCancellationRequested) {
            await Task.Yield();
        }
        if (cancelTokenSource.IsCancellationRequested) {
            return;
        }
        onChangeStatus?.Invoke(Status.SuccessPreparing);
        _videoPlayer.Play();
    }

    /// <summary>
    /// Load Video
    /// </summary>
    /// <param name="_url"></param>
    public async void LoadVideoFromURL(string _url, Action<Status> onChangeStatus) {
        Cancel();
        string _pathToFile = Path.Combine(Application.persistentDataPath, _url.Split(Path.AltDirectorySeparatorChar).Last());
        if (!File.Exists(_pathToFile)) {
            if (_videoRequest != null && !_videoRequest.isDone) {
                _videoRequest.Dispose();
            }
            _videoRequest = UnityWebRequest.Get(_url);
            onChangeStatus?.Invoke(Status.ProcessLoading);
            await _videoRequest.SendWebRequest();
            if (_videoRequest.result != UnityWebRequest.Result.Success) {
                onChangeStatus?.Invoke(Status.FailLoading);
            } else {
                byte[] videoBytes = _videoRequest.downloadHandler.data;
                File.WriteAllBytes(_pathToFile, videoBytes);
                onChangeStatus?.Invoke(Status.SuccessLoading);
                PlayVideoFromURL(_pathToFile, onChangeStatus);
            }
        } else {
            onChangeStatus?.Invoke(Status.SuccessLoading);
            PlayVideoFromURL(_pathToFile, onChangeStatus);
        }
    }

    private void Cancel() {
        if (_cancelTokenSource != null) {
            _cancelTokenSource.Cancel();
            _cancelTokenSource = null;
        }
    }

    /// <summary>
    /// StopVideo
    /// </summary>
    public void StopVideo() {
        Cancel();
        _videoPlayer.Stop();
    }

}
