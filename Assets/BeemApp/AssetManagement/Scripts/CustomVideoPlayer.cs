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
        ProcessLoading,
        FailLoading,
        SuccessLoading,
        ProcessPreparing,
        SuccessPreparing
    }

    private CancellationTokenSource _cancelTokenSource;
    private VideoPlayer _videoPlayer;
    private RawImage _rawImage;
    private Material _greenScreenRemoverMat;
    private Action<Status> _onChangeStatus;
    private UnityWebRequest _videoRequest;
    private Texture2D _thumbnail;
    private bool _thumbnailOk;
    private int _vidHeight;
    private int _vidWidth;
    public CustomVideoPlayer(VideoPlayer videoPlayer, RawImage rawImage, Material greenScreenRemoverMat, Action<Status> onChangeStatus) {
        _videoPlayer = videoPlayer;
        _rawImage = rawImage;
        _onChangeStatus = onChangeStatus;
        if (_greenScreenRemoverMat == null) {
            _greenScreenRemoverMat = new Material(greenScreenRemoverMat);
        }

    }


    /// <summary>
    /// Play Video
    /// </summary>
    /// <param name="url"></param>
    public /*async*/ void PlayVideoFromURL(string url) {
        /*Cancel();
        _cancelTokenSource = new CancellationTokenSource();
        CancellationToken cancelTokenSource = _cancelTokenSource.Token;
        _videoPlayer.source = VideoSource.Url;
        _videoPlayer.url = url;
        _videoPlayer.Prepare();
        _onChangeStatus?.Invoke(Status.ProcessPreparing);
        while (_videoPlayer.isPrepared == false && !cancelTokenSource.IsCancellationRequested) {
            await Task.Yield();
        }
        if (cancelTokenSource.IsCancellationRequested) {
            return;
        }
        _onChangeStatus?.Invoke(Status.SuccessPreparing);
        _videoPlayer.Play();*/
        _onChangeStatus?.Invoke(Status.ProcessPreparing);
        _videoPlayer.url = url;
        _videoPlayer.Stop();
        _videoPlayer.renderMode = VideoRenderMode.APIOnly;
        _videoPlayer.sendFrameReadyEvents = true;
        _videoPlayer.frameReady += FrameReady;
        _videoPlayer.Prepare();
        PrepareVideo();
    }

    private void FrameReady(VideoPlayer vp, long frameIndex) {
        _videoPlayer.Pause();
        _rawImage.texture = vp.texture;

        _rawImage.texture = Get2DTexture(vp.url);

        _videoPlayer.sendFrameReadyEvents = false; //To stop frameReady events
        vp = null;

        _thumbnailOk = true;
    }

    private async void PrepareVideo() {
        while (!_videoPlayer.isPrepared) {
            await Task.Yield();
        }

        _videoPlayer.Play();

        _vidWidth = Convert.ToInt32(_videoPlayer.width);
        _vidHeight = Convert.ToInt32(_videoPlayer.height);

        _videoPlayer.isLooping = true;
        _videoPlayer.renderMode = VideoRenderMode.MaterialOverride;

        while (!_thumbnailOk) {
            await Task.Yield();
        }

        _videoPlayer.Play();

        _onChangeStatus?.Invoke(Status.SuccessPreparing);

        GC.Collect();
    }

    private Texture2D Get2DTexture(string url) {

        _thumbnail = new Texture2D(_rawImage.texture.width, _rawImage.texture.height, TextureFormat.RGBA32, false);
        RenderTexture cTexture = RenderTexture.active;
        RenderTexture rTexture = new RenderTexture(_rawImage.texture.width, _rawImage.texture.height, 32);
        Graphics.Blit(_rawImage.texture, rTexture, _greenScreenRemoverMat);

        RenderTexture.active = rTexture;
        _thumbnail.ReadPixels(new Rect(0, 0, rTexture.width, rTexture.height), 0, 0);
        _thumbnail.Apply();

        Color[] pixels = _thumbnail.GetPixels();

        RenderTexture.active = cTexture;

        rTexture.Release();

        string dirPath = Path.Combine(Application.persistentDataPath, "preview");

        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }

        string fileName = url.Split(Path.AltDirectorySeparatorChar).Last().Split(Path.DirectorySeparatorChar).Last().Split('.').First() + ".png";

        string _pathToFile = Path.Combine(dirPath, fileName);

        if (!File.Exists(_pathToFile)) {

            byte[] bytes = _thumbnail.EncodeToPNG();

            File.WriteAllBytes(_pathToFile, bytes);
        }


        return _thumbnail;
    }

    /// <summary>
    /// Load Video
    /// </summary>
    /// <param name="url"></param>
    public async void LoadVideoFromURL(string url) {
        Cancel();
        string dirPath = Path.Combine(Application.persistentDataPath, "video");

        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }

        string _pathToFile = Path.Combine(dirPath, url.Split(Path.AltDirectorySeparatorChar).Last());

        if (!File.Exists(_pathToFile)) {
            if (_videoRequest != null && !_videoRequest.isDone) {
                _videoRequest.Dispose();
            }
            _videoRequest = UnityWebRequest.Get(url);
            _onChangeStatus?.Invoke(Status.ProcessLoading);
            await _videoRequest.SendWebRequest();
            if (_videoRequest.result != UnityWebRequest.Result.Success) {
                _onChangeStatus?.Invoke(Status.FailLoading);
            } else {
                byte[] videoBytes = _videoRequest.downloadHandler.data;
                File.WriteAllBytes(_pathToFile, videoBytes);
                _onChangeStatus?.Invoke(Status.SuccessLoading);
                PlayVideoFromURL(_pathToFile);
            }
        } else {
            _onChangeStatus?.Invoke(Status.SuccessLoading);
            PlayVideoFromURL(_pathToFile);
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
