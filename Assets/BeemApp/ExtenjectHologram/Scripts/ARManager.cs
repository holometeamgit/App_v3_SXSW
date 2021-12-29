using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Zenject;
namespace Beem.Extenject.Hologram {
    /// <summary>
    /// ARManagerForZenject
    /// </summary>
    public class ARManager : MonoBehaviour {

        private SignalBus _signalBus;

        private ARSession _arSession;
        private ARPlaneManager _arPlaneManager;

        [Inject]
        public void Construct(SignalBus signalBus) {
            _signalBus = signalBus;
        }

        private void OnEnable() {
            _arSession = FindObjectOfType<ARSession>();
            _arPlaneManager = FindObjectOfType<ARPlaneManager>();
            _signalBus.Subscribe<ARSessionActivateSignal>(ActivateARSession);
            _signalBus.Subscribe<ARPinchSignal>(ActivatePlanes);
        }

        private void OnDisable() {
            _signalBus.Unsubscribe<ARSessionActivateSignal>(ActivateARSession);
            _signalBus.Unsubscribe<ARPinchSignal>(ActivatePlanes);
        }

        private void ActivateARSession(ARSessionActivateSignal signal) {
            _arSession.enabled = signal.Active;

            if (_arSession.enabled) {
                _arSession.Reset();
            }
        }

        private void ActivatePlanes(ARPinchSignal signal) {
            _arPlaneManager.enabled = signal.Active;
            foreach (var plane in _arPlaneManager.trackables) {
                plane.gameObject.SetActive(signal.Active);
            }
        }
    }
}
