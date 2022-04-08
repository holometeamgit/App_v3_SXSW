using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

//TODO: Class in development
/// <summary>
/// Save First frame from video as texture
/// </summary>
public class CustomVideoPlayerController {
    private VideoPlayer _videoPlayer;
    private RawImage _rawImage;
    private Texture2D _thumbnail;
    private bool _thumbnailOk;
    private int _vidHeight;
    private int _vidWidth;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="videoPlayer"></param>
    /// <param name="rawImage"></param>
    public CustomVideoPlayerController(VideoPlayer videoPlayer, RawImage rawImage) {
        _rawImage = rawImage;
        _videoPlayer = videoPlayer;
    }

    /// <summary>
    /// Prepare video
    /// </summary>
    /// <param name="path_"></param>
    public void VideoPreparation(string path_) {
        _videoPlayer.url = path_;
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

        _rawImage.texture = Get2DTexture();

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

        GC.Collect();
    }

    private Texture2D Get2DTexture() {
        _thumbnail = new Texture2D(_rawImage.texture.width, _rawImage.texture.height, TextureFormat.RGBA32, false);
        RenderTexture cTexture = RenderTexture.active;
        RenderTexture rTexture = new RenderTexture(_rawImage.texture.width, _rawImage.texture.height, 32);
        Graphics.Blit(_rawImage.texture, rTexture);

        RenderTexture.active = rTexture;
        _thumbnail.ReadPixels(new Rect(0, 0, rTexture.width, rTexture.height), 0, 0);
        _thumbnail.Apply();

        Color[] pixels = _thumbnail.GetPixels();

        RenderTexture.active = cTexture;

        rTexture.Release();

        return _thumbnail;
    }

}
