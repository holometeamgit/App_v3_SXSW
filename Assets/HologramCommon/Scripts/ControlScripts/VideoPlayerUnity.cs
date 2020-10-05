using HoloMeSDK;
using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerUnity : IVideoPlayer {

    VideoPlayer videoPlayer;
    VideoAudioOutputMode audioOutputModeRef;
    AudioSource targetAudioSource;
    string url;

    public void AddToPlaybackQuad(GameObject ARQuad) {
        videoPlayer = ARQuad.AddComponent<VideoPlayer>();
    }

    public void SetDefaults(AudioSource targetAudioSource = null) {
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;

        if (targetAudioSource) {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, targetAudioSource);
        }
    }

    public bool IsPaused() {
        return videoPlayer.isPaused;
    }

    public bool IsPlaying() {
        return videoPlayer.isPlaying;
    }

    public bool IsPrepared() {
        return videoPlayer.isPrepared;
    }

    public void MuteClip() {
        audioOutputModeRef = videoPlayer.audioOutputMode;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
    }

    public void UnMuteClip() {
        videoPlayer.audioOutputMode = audioOutputModeRef;
    }

    public void Pause() {
        videoPlayer.Pause();
    }

    public void Play() {
        videoPlayer.Play();
    }

    public void Prepare() {
        videoPlayer.url = url;
        videoPlayer.Prepare();
    }

    public void SetAutoPlay(bool autoPlay) {
        videoPlayer.playOnAwake = autoPlay;
    }

    public void SetLooping(bool loop) {
        videoPlayer.isLooping = loop;
    }

    public void SetOnErrorEvent(Action OnError) {
        videoPlayer.errorReceived += (x, y) => OnError();
    }

    public void SetOnReadyEvent(Action OnReady) {
        videoPlayer.prepareCompleted += x => OnReady();
    }

    public void SetVideoURL(string url) {
        this.url = url;
        videoPlayer.url = url;
    }

    public void Stop() {
        videoPlayer.Stop();
    }
}
