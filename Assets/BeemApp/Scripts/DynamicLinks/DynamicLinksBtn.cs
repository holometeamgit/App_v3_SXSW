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
        private string _url = "https://beem.me";

        [SerializeField]
        private DynamicLinkParameters.Folder _folder;

        [SerializeField]
        private string _prefix = "https://beemrfc.page.link";

        public void OnPointerClick(PointerEventData eventData) {
            DynamicLinkParameters dynamicLinkParameters = new DynamicLinkParameters(_prefix, _url, _folder, _roomId);
            DynamicLinksCallBacks.onCreateShortLink?.Invoke(dynamicLinkParameters);
        }
    }
}
