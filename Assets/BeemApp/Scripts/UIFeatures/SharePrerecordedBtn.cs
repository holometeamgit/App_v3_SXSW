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
            if (!string.IsNullOrWhiteSpace(_streamData.share_link.ToString())) {
                StreamCallBacks.onShareStadiumLink?.Invoke(_streamData.share_link);
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            Share();
        }
    }
}
