using Beem.Firebase.DynamicLink;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Share Button for AR-Messages
    /// </summary>

    public class ShareARMessagesBtn : MonoBehaviour, IStreamDataView {

        private StreamJsonData.Data _streamData = default;

        public void Init(StreamJsonData.Data streamData) {
            _streamData = streamData;
        }

        /// <summary>
        /// Share AR Messages
        /// </summary>
        public void Share() {
            if (!string.IsNullOrWhiteSpace(_streamData.id.ToString())) {
                StreamCallBacks.onGetARMessagesLink?.Invoke(_streamData);
            } else {
                DynamicLinksCallBacks.onShareAppLink?.Invoke();
            }
        }
    }
}
