using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.DynamicLinks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// Create New Link
    /// </summary>
    public class DynamicLinksBtn : MonoBehaviour, IPointerClickHandler {

        [SerializeField]
        private string _roomId = "abc";

        [SerializeField]
        private string _source = "Source";

        [SerializeField]
        private string _url = "https://beem.me";

        private const string ROOM = "room";

        public void SetRoomId(string roomId) {
            _roomId = roomId;
        }

        public void SetSource(string source) {
            _source = source;
        }

        [SerializeField]
        private string _prefix = "https://beemrfc.page.link";

        public void OnPointerClick(PointerEventData eventData) {
            DynamicLinksCallBacks.onCreateShortLink?.Invoke(_prefix, ROOM, _roomId, _url, _source);
        }
    }
}
