using Beem.Firebase.DynamicLink;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Share Button for prerecorded video
    /// </summary>

    public class SharePrerecordedBtn : MonoBehaviour, IStreamDataView, IPointerDownHandler {

        private StreamJsonData.Data _streamData = default;

        public void Init(StreamJsonData.Data streamData) {
            _streamData = streamData;
        }

        /// <summary>
        /// Share prerecorded video
        /// </summary>
        public void Share() {
            if (!string.IsNullOrWhiteSpace(_streamData.id.ToString())) {
                StreamCallBacks.onGetStreamLink?.Invoke(_streamData.id.ToString(), _streamData.user);
            } else {
                DynamicLinksCallBacks.onShareAppLink?.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            Share();
        }
    }
}
