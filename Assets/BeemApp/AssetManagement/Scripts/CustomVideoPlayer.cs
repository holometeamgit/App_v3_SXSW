using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Custom Video Player
/// </summary>
public class CustomVideoPlayer {

    public enum Status {
        Loading,
        Successed,
        Failed
    }

    private VideoPlayer _videoPlayer;
    private Action<Status> _onChangeStatus;
    private bool _thumbnailOk;

    public CustomVideoPlayer(VideoPlayer videoPlayer, Action<Status> onChangeStatus) {
        _videoPlayer = videoPlayer;
        _onChangeStatus = onChangeStatus;
    }


    /// <summary>
    /// Play Video
    /// </summary>
    /// <param name="url"></param>
    public void PlayVideoFromURL(string url) {
        _onChangeStatus?.Invoke(Status.Loading);
        _videoPlayer.url = url;
        _videoPlayer.Stop();
        _videoPlayer.renderMode = VideoRenderMode.APIOnly;
        _videoPlayer.sendFrameReadyEvents = true;
        _videoPlayer.frameReady += FrameReady;
        _videoPlayer.prepareCompleted += Prepare;
        _videoPlayer.Prepare();
    }

    /// <summary>
    /// Play video
    /// </summary>
    public void Play() {
        _videoPlayer.Play();
    }

    private void FrameReady(VideoPlayer vp, long frameIndex) {
        _videoPlayer.Pause();

        _videoPlayer.sendFrameReadyEvents = false; //To stop frameReady events

        _thumbnailOk = true;
        _videoPlayer.frameReady -= FrameReady;
    }

    private async void Prepare(VideoPlayer vp) {
        _videoPlayer.Play();

        _videoPlayer.isLooping = true;

        while (!_thumbnailOk) {
            await Task.Yield();
        }

        _onChangeStatus?.Invoke(Status.Successed);

        _videoPlayer.prepareCompleted -= Prepare;
        GC.Collect();
    }


    /// <summary>
    /// Load Video
    /// </summary>
    /// <param name="_url"></param>
    public async void LoadVideoFromURL(string _url) {
        string _pathToFile = Path.Combine(Application.persistentDataPath, _url.Split(Path.AltDirectorySeparatorChar).Last());
        if (!File.Exists(_pathToFile)) {
            UnityWebRequest webRequest = UnityWebRequest.Get(_url);
            _onChangeStatus?.Invoke(Status.Loading);
            await webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success) {
                _onChangeStatus?.Invoke(Status.Failed);
            } else {
                byte[] videoBytes = webRequest.downloadHandler.data;
                File.WriteAllBytes(_pathToFile, videoBytes);
                PlayVideoFromURL(_pathToFile);
            }
        } else {
            PlayVideoFromURL(_pathToFile);
        }
    }

    /// <summary>
    /// StopVideo
    /// </summary>
    public void Stop() {
        _videoPlayer.Stop();
    }

}
