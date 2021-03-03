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

        public void SetRoomId(string roomId) {
            _roomId = roomId;
        }

        [SerializeField]
        private string _prefix = "https://beemrfc.page.link";

        public void OnPointerClick(PointerEventData eventData) {
            DynamicLinksCallBacks.onCreateShortLink?.Invoke(_prefix, _roomId);
        }
    }
}
