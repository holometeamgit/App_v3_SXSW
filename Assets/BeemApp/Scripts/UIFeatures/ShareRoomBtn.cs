using Beem.Firebase.DynamicLink;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Share Button for prerecorded video
    /// </summary>

    public class ShareRoomBtn : MonoBehaviour, IRoomDataView, IPointerDownHandler {

        private RoomJsonData _data = default;

        public void Init(RoomJsonData data) {
            _data = data;
        }

        /// <summary>
        /// Share prerecorded video
        /// </summary>
        public void Share() {
            if (!string.IsNullOrWhiteSpace(_data.share_link.ToString())) {
                StreamCallBacks.onShareRoomLink?.Invoke(_data.share_link);
            }
        }

        public void OnPointerDown(PointerEventData eventData) {
            Share();
        }
    }
}
