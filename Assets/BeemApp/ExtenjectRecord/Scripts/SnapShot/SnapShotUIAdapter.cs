using Beem.Extenject.UI;
using UnityEngine;
using Zenject;

namespace Beem.Extenject.Record {

    /// <summary>
    /// Adapter for snapshots
    /// </summary>
    public class SnapShotUIAdapter : MonoBehaviour {

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
            _signalBus.Subscribe<SnapShotFinishSignal>(Refresh);
        }

        public void OnDisable() {
            _signalBus.Unsubscribe<SnapShotFinishSignal>(Refresh);
        }

        private void Refresh(SnapShotFinishSignal snapShotFinishSignal) {
            _windowController.OnCalledSignal(_snapShotWindow, snapShotFinishSignal.SnapShot);
        }
    }
}
