using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// CurrentPlayer for video
    /// </summary>
    public class VideoPlayerController : IInitializable, ILateDisposable {

        [Header("Video Player")]
        [SerializeField]
        private VideoPlayer _videoPlayer;

        private double currentTime = default;
        private CancellationTokenSource cancelTokenSource;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        /// <summary>
        /// Play Video
        /// </summary>

        private void OnPlay() {
            if (_videoPlayer == null) {
                return;
            }

            if (!_videoPlayer.isPrepared) {
                _videoPlayer.prepareCompleted += OnPrepare;
                _videoPlayer.Prepare();
            } else {
                OnPrepare(_videoPlayer);
            }

        }

        /// <summary>
        /// Prepare Video
        /// </summary>
        /// <param name="videoPlayer"></param>

        private void OnPrepare(VideoPlayer videoPlayer) {

            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.prepareCompleted -= OnPrepare;
            _videoPlayer.seekCompleted += OnSeekCompleted;
            _videoPlayer.Play();
        }

        /// <summary>
        /// Pause Video
        /// </summary>

        private void OnPause() {

            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.Pause();
        }

        /// <summary>
        /// Rewind Start
        /// </summary>

        private void OnRewindStarted() {
            OnPause();
        }

        private void OnResume() {

            if (_videoPlayer == null) {
                return;
            }
            if (_videoPlayer.isPaused) {
                OnPlay();
            }
        }

        private void OnApplicationPause(bool pause) {
            if (pause) {
                OnPause();
            }
        }

        private void OnApplicationFocus(bool focus) {
            if (focus) {
                OnResume();
            }
        }

        /// <summary>
        /// Rewind Video
        /// </summary>
        /// <param name="pct"></param>
        private void OnRewind(RewindSignal rewindSignal) {

            if (_videoPlayer == null) {
                return;
            }

            if (!_videoPlayer.canSetTime) return;
            if (!_videoPlayer.isPrepared) return;
            currentTime = rewindSignal.Percent * (_videoPlayer.frameCount / _videoPlayer.frameRate);
            _videoPlayer.time = currentTime;
        }

        /// <summary>
        /// Seek Completed
        /// </summary>
        /// <param name="videoPlayer"></param>

        private void OnSeekCompleted(VideoPlayer videoPlayer) {
            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.sendFrameReadyEvents = true;
            _videoPlayer.seekCompleted -= OnSeekCompleted;
            _videoPlayer.frameReady += OnFrameReady;

        }

        /// <summary>
        /// Frame Ready
        /// </summary>
        /// <param name="videoPlayer"></param>
        /// <param name="frame"></param>

        private void OnFrameReady(VideoPlayer videoPlayer, long frame) {
            if (_videoPlayer == null) {
                return;
            }

            _videoPlayer.sendFrameReadyEvents = false;
            _videoPlayer.frameReady -= OnFrameReady;
        }

        private async void LoadVideoFrameComletedAsync() {

            if (_videoPlayer == null) {
                return;
            }

            Cancel();

            cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancelTokenSource.Token;

            try {
                while (!cancellationToken.IsCancellationRequested && Mathf.Abs((float)(currentTime - _videoPlayer.time)) > 1f) {
                    await Task.Yield();
                }

                if (!cancellationToken.IsCancellationRequested) {
                    OnResume();
                }
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        /// <summary>
        /// Clear Info
        /// </summary>
        private void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }
        }

        /// <summary>
        /// Rewind Finished
        /// </summary>
        /// <param name="pct"></param>

        private void OnRewindFinished(FinishRewindSignal finishRewindSignal) {
            RewindSignal rewindSignal = new RewindSignal {
                Percent = finishRewindSignal.Percent
            };
            OnRewind(rewindSignal);
            LoadVideoFrameComletedAsync();
        }

        /// <summary>
        /// Stop Video
        /// </summary>

        private void OnStop() {
            if (_videoPlayer == null) {
                return;
            }

            if (_videoPlayer.isPlaying) {
                _videoPlayer.Stop();
            }
            Cancel();
        }

        private void OnInit(InitSignal videoSignal) {

            if (videoSignal.Player == null) {
                return;
            }

            OnStop();
            _videoPlayer = videoSignal.Player;
            OnPlay();
        }

        public void Initialize() {
            _signalBus.Subscribe<InitSignal>(OnInit);
            _signalBus.Subscribe<PlaySignal>(OnPlay);
            _signalBus.Subscribe<PauseSignal>(OnPause);
            _signalBus.Subscribe<StopSignal>(OnStop);
            _signalBus.Subscribe<StartRewindSignal>(OnRewindStarted);
            _signalBus.Subscribe<RewindSignal>(OnRewind);
            _signalBus.Subscribe<FinishRewindSignal>(OnRewindFinished);
        }

        public void LateDispose() {
            _signalBus.Unsubscribe<InitSignal>(OnInit);
            _signalBus.Unsubscribe<PlaySignal>(OnPlay);
            _signalBus.Unsubscribe<PauseSignal>(OnPause);
            _signalBus.Unsubscribe<StopSignal>(OnStop);
            _signalBus.Unsubscribe<StartRewindSignal>(OnRewindStarted);
            _signalBus.Unsubscribe<RewindSignal>(OnRewind);
            _signalBus.Unsubscribe<FinishRewindSignal>(OnRewindFinished);
            OnStop();
        }
    }
}
