using UnityEngine;
using UnityEngine.Video;
using Zenject;

namespace Beem.Extenject.Video {

    /// <summary>
    /// Video Player view
    /// </summary>
    public class VideoPlayerBtnView : MonoBehaviour {

        [Header("Action On Play")]
        [SerializeField]
        private GameObject playBtn;
        [Header("Action On Pause")]
        [SerializeField]
        private GameObject pauseBtn;

        private VideoPlayerController _videoPlayerController;

        [Inject]
        public void Construct(VideoPlayerController videoPlayerController) {
            _videoPlayerController = videoPlayerController;
        }

        private void OnEnable() {
            _videoPlayerController.onPlay += OnPlay;
            _videoPlayerController.onPause += OnPause;
            _videoPlayerController.onStop += OnPause;
            if (_videoPlayerController.Player.isPlaying) {
                OnPlay();
            } else {
                OnPause();
            }
        }

        private void OnDisable() {
            _videoPlayerController.onPlay -= OnPlay;
            _videoPlayerController.onPause -= OnPause;
            _videoPlayerController.onStop -= OnPause;
        }

        private void OnPlay() {
            Refresh(true);
        }

        private void OnPause() {
            Refresh(false);
        }

        private void Refresh(bool isPlaying) {
            playBtn.SetActive(!isPlaying);
            pauseBtn.SetActive(isPlaying);
        }
    }
}
