using UnityEngine;
using UnityEngine.EventSystems;
using Beem.Extenject.UI;

namespace Beem.Extenject.Record.SnapShot {

    /// <summary>
    /// Share snapshot
    /// </summary>
    public class ShareSnapShotBtn : MonoBehaviour, IPointerDownHandler, IShow {

        private Texture2D _snapshot;

        private void OnRecordComplete(Texture2D snapshot) {
            _snapshot = snapshot;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (_snapshot != null) {
                new NativeShare().AddFile(_snapshot).Share();
            } else {
                Debug.LogError("Screenshot was null");
            }
        }

        public void Show<T>(T parameter) {
            if (parameter is Texture2D) {
                OnRecordComplete(parameter as Texture2D);
            }
        }
    }
}
