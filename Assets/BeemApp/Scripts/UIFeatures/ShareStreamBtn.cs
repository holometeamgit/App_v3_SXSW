using Beem.Firebase.DynamicLink;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Share Button for stream data
    /// </summary>

    public class ShareStreamBtn : MonoBehaviour, IStreamDataView {

        private StreamJsonData.Data _data = default;

        private ShareLinkController _shareController = new ShareLinkController();
        private const string TITLE = "You have been invited to {0}'s Stadium";
        private const string DESCRIPTION = "Click the link below to join {0}'s Stadium";

        public void Init(StreamJsonData.Data data) {
            _data = data;
        }

        /// <summary>
        /// Share prerecorded video
        /// </summary>
        public void Share() {

            if (!string.IsNullOrWhiteSpace(_data.share_link.ToString())) {
                string msg = string.Empty;

                if (_data.HasStreamUrl) {
                    msg = _data.share_link;
                } else if (_data.HasAgoraChannel) {
                    string title = string.Format(TITLE, _data.user);
                    string description = string.Format(DESCRIPTION, _data.user);
                    msg = title + "\n" + description + "\n" + _data.share_link;
                }

                _shareController.ShareLink(msg);
            }
        }
    }
}
