using Beem.Extenject.Permissions;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Beem.Extenject.Record {
    /// <summary>
    /// Record Button
    /// </summary>
    public class RecordBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [Header("Recording Time")]
        [SerializeField]
        private Vector2 recordingTime = new Vector2(2, 15);

        private SignalBus _signalbus;
        private PermissionController _permissionController;

        [Inject]
        public void Construct(SignalBus signalBus, PermissionController permissionController) {
            _signalbus = signalBus;
            _permissionController = permissionController;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (!_permissionController.CheckMicAccess()) {
                return;
            }
            _signalbus.Fire(new VideoRecordStartSignal(recordingTime));
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (!_permissionController.CheckMicAccess()) {
                return;
            }
            _signalbus.Fire(new VideoRecordStopSignal());
        }

    }
}