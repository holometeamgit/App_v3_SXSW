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

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _signalBus.Subscribe<PlaySignal>(OnPlay);
            _signalBus.Subscribe<PauseSignal>(OnPause);
            _signalBus.Subscribe<StopSignal>(OnPause);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<PlaySignal>(OnPlay);
            _signalBus.Unsubscribe<PauseSignal>(OnPause);
            _signalBus.Unsubscribe<StopSignal>(OnPause);
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
