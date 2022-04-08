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
        Processing,
        Failed,
        Successed
    }

    private CancellationTokenSource _cancelTokenSource;
    private VideoPlayer _videoPlayer;
    private RawImage _rawImage;
    private Material _greenScreenRemoverMat;
    private Action<Status> _onChangeStatus;
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
    public void PlayVideoFromURL(string url) {
        _onChangeStatus?.Invoke(Status.Processing);
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

        while (!_thumbnailOk) {
            await Task.Yield();
        }

        _videoPlayer.Play();

        _onChangeStatus?.Invoke(Status.Successed);

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

        string previewFilePath = GetLocalPreviewPath(url);

        if (!File.Exists(previewFilePath)) {

            byte[] bytes = _thumbnail.EncodeToPNG();

            File.WriteAllBytes(previewFilePath, bytes);

        }


        return _thumbnail;
    }

    /// <summary>
    /// Load Video
    /// </summary>
    /// <param name="url"></param>
    public void LoadVideoFromURL(string url) {
        Cancel();

        string previewFilePath = GetLocalPreviewPath(url);

        if (!File.Exists(previewFilePath)) {
            string videoFilePath = GetLocalVideoPath(url);

            if (!File.Exists(videoFilePath)) {
                LoadVideo(url);
            } else {
                PlayVideoFromURL(videoFilePath);
            }
        } else {
            LoadTexture(previewFilePath);
        }
    }

    private string GetLocalVideoPath(string url) {
        string videoPath = Path.Combine(Application.persistentDataPath, "video");

        if (!Directory.Exists(videoPath)) {
            Directory.CreateDirectory(videoPath);
        }

        string filename = url.Split(Path.AltDirectorySeparatorChar).Last().Split(Path.DirectorySeparatorChar).Last();

        string path = Path.Combine(videoPath, filename);

        path = Path.ChangeExtension(path, ".mp4");

        return path;
    }

    private string GetLocalPreviewPath(string url) {
        string videoPath = Path.Combine(Application.persistentDataPath, "preview");

        if (!Directory.Exists(videoPath)) {
            Directory.CreateDirectory(videoPath);
        }

        string filename = url.Split(Path.AltDirectorySeparatorChar).Last().Split(Path.DirectorySeparatorChar).Last();

        string path = Path.Combine(videoPath, filename);

        path = Path.ChangeExtension(path, ".png");

        return path;
    }

    /// <summary>
    /// Load video
    /// </summary>
    /// <param name="url"></param>
    private async void LoadVideo(string url) {

        string videoFilePath = GetLocalVideoPath(url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        _onChangeStatus?.Invoke(Status.Processing);
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success) {
            _onChangeStatus?.Invoke(Status.Failed);
        } else {
            byte[] videoBytes = request.downloadHandler.data;
            File.WriteAllBytes(videoFilePath, videoBytes);
            PlayVideoFromURL(videoFilePath);
        }
    }

    /// <summary>
    /// Load Texture
    /// </summary>
    /// <param name="url"></param>
    private async void LoadTexture(string url) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        _onChangeStatus?.Invoke(Status.Processing);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            _onChangeStatus?.Invoke(Status.Failed);
        } else {
            _rawImage.texture = DownloadHandlerTexture.GetContent(request);
            _onChangeStatus?.Invoke(Status.Successed);
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
