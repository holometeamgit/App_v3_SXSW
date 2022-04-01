using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    private VideoPlayer _videoPlayer;
    public CustomVideoPlayer(VideoPlayer videoPlayer) {
        _videoPlayer = videoPlayer;
    }


    /// <summary>
    /// Play Video
    /// </summary>
    /// <param name="_url"></param>
    public async void PlayVideoFromURL(string _url, Action<Status> onChangeStatus) {
        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = _url;
        _videoPlayer.Prepare();
        onChangeStatus?.Invoke(Status.ProcessPreparing);
        while (_videoPlayer.isPrepared == false) {
            await Task.Yield();
        }
        onChangeStatus?.Invoke(Status.SuccessPreparing);
        _videoPlayer.Play();
    }

    /// <summary>
    /// Load Video
    /// </summary>
    /// <param name="_url"></param>
    public async void LoadVideoFromURL(string _url, Action<Status> onChangeStatus) {

        string _pathToFile = Path.Combine(Application.streamingAssetsPath, _url.Split(Path.AltDirectorySeparatorChar).Last());

        HelperFunctions.DevLogError(_pathToFile);

        if (!File.Exists(_pathToFile)) {
            UnityWebRequest _videoRequest = UnityWebRequest.Get(_url);
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

}
