using System;
using UnityEngine;

namespace HoloMeSDK {
    public interface IVideoPlayer {
        void AddToPlaybackQuad(GameObject ARQuad);
        void SetDefaults(AudioSource targetAudioSource = null);
        void Stop();
        bool IsPlaying();
        void Play();
        bool IsPrepared();
        void Prepare();
        void SetVideoURL(string url);
        bool IsPaused();
        void Pause();
        void MuteClip();
        void UnMuteClip();
        void SetLooping(bool loop);
        void SetAutoPlay(bool autoPlay);
        void SetOnReadyEvent(Action OnReady);
        void SetOnErrorEvent(Action OnError);
        double GetClipLength();
    }
}
