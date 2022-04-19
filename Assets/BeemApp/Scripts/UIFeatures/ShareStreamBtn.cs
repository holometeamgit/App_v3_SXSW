using Beem.Firebase.DynamicLink;
using Firebase.DynamicLinks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Beem.UI {

    /// <summary>
    /// Share Button for Room
    /// </summary>

    public class ShareStreamBtn : MonoBehaviour {

        [SerializeField]
        private VideoUploader _videoUploader;

        private AgoraController _agoraController = default;
        private GetRoomController _getRoomController;
        private GetStadiumController _getStadiumController;

        private ShareLinkController _shareController = new ShareLinkController();
        private const string TITLE = "You have been invited to {0}'s {1}";
        private const string DESCRIPTION = "Click the link below to join {0}'s {1}";
        private const string ROOM = "Room";
        private const string STADIUM = "Stadium";

        [Inject]
        public void Construct(AgoraController agoraController, WebRequestHandler webRequestHandler) {
            _agoraController = agoraController;
            _getRoomController = new GetRoomController(_videoUploader, webRequestHandler);
            _getStadiumController = new GetStadiumController(_videoUploader, webRequestHandler);
        }

        /// <summary>
        /// Share Stream
        /// </summary>
        public void Share() {
            if (_agoraController.IsRoom) {
                _getRoomController.GetRoomByUsername(_agoraController.ChannelName, Share);
            } else {
                _getStadiumController.GetStadiumByUsername(_agoraController.ChannelName, Share);
            }
        }

        private void Share(StreamJsonData.Data data) {
            if (!string.IsNullOrWhiteSpace(data.share_link.ToString())) {
                string title = string.Format(TITLE, data.user, STADIUM);
                string description = string.Format(DESCRIPTION, data.user, STADIUM);
                string msg = title + "\n" + description + "\n" + data.share_link;
                _shareController.ShareLink(msg);
            }
        }

        private void Share(RoomJsonData data) {
            if (!string.IsNullOrWhiteSpace(data.share_link.ToString())) {
                string title = string.Format(TITLE, data.user, ROOM);
                string description = string.Format(DESCRIPTION, data.user, ROOM);
                string msg = title + "\n" + description + "\n" + data.share_link;
                _shareController.ShareLink(msg);
            }
        }
    }
}
