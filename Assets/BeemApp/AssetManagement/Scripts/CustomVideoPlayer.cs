using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        UnityWebRequest _videoRequest = UnityWebRequest.Get(_url);
        onChangeStatus?.Invoke(Status.ProcessLoading);
        await _videoRequest.SendWebRequest();

        if (_videoRequest.result != UnityWebRequest.Result.Success) {
            onChangeStatus?.Invoke(Status.FailLoading);
        } else {
            byte[] _videoBytes = _videoRequest.downloadHandler.data;
            string _pathToFile = Path.Combine(Application.persistentDataPath, _url);
            Debug.LogError(_pathToFile);
            File.WriteAllBytes(_pathToFile, _videoBytes);
            onChangeStatus?.Invoke(Status.SuccessLoading);
            PlayVideoFromURL(_pathToFile, onChangeStatus);
        }
    }
}
