using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.UI {

    /// <summary>
    /// Share Button for Room
    /// </summary>

    public class ShareRoomBtn : MonoBehaviour, IRoomDataView {

        private RoomJsonData _data = default;

        private ShareLinkController _shareController = new ShareLinkController();
        private const string TITLE = "You have been invited to {0}'s Room";
        private const string DESCRIPTION = "Click the link below to join {0}'s Room";

        public void Init(RoomJsonData data) {
            _data = data;
        }

        /// <summary>
        /// Share Room
        /// </summary>
        public void Share() {
            if (!string.IsNullOrWhiteSpace(_data.share_link.ToString())) {
                string title = string.Format(TITLE, _data.user);
                string description = string.Format(DESCRIPTION, _data.user);
                string msg = title + "\n" + description + "\n" + _data.share_link;
                _shareController.ShareLink(msg);
            }
        }
    }
}
