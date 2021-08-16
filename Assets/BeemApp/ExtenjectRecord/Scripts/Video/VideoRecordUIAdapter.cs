using Beem.Extenject.UI;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {

    /// <summary>
    /// Adapter for video record
    /// </summary>
    public class VideoRecordUIAdapter : MonoBehaviour {

        [SerializeField]
        private WindowSignal _snapShotWindow;

        private SignalBus _signalBus;
        private WindowController _windowController;

        [Inject]
        public void Construct(SignalBus signalbus, WindowController windowController) {
            _signalBus = signalbus;
            _windowController = windowController;
        }


        private void OnEnable() {
            _signalBus.Subscribe<VideoRecordFinishSignal>(Refresh);
        }

        public void OnDisable() {
            _signalBus.Unsubscribe<VideoRecordFinishSignal>(Refresh);
        }

        private void Refresh(VideoRecordFinishSignal videoRecordFinishSignal) {
            _windowController.OnCalledSignal(_snapShotWindow, videoRecordFinishSignal.Path);
        }
    }
}
