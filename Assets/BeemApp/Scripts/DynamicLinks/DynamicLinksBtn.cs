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

            string _baseLink = _prefix + "/" + _roomId;

            var components = new DynamicLinkComponents(
         // The base Link.
         new System.Uri(_baseLink),
         // The dynamic link URI prefix.
         _prefix) {
                IOSParameters = new IOSParameters(Application.identifier),
                AndroidParameters = new AndroidParameters(Application.identifier),
            };

            var options = new DynamicLinkOptions {
                PathLength = DynamicLinkPathLength.Short
            };

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            DynamicLinks.GetShortLinkAsync(components, options).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("GetShortLinkAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("GetShortLinkAsync encountered an error: " + task.Exception);
                    return;
                }

                // Short Link has been created.
                ShortDynamicLink link = task.Result;
                Debug.LogFormat("Generated short link: {0}", link.Url);
                DynamicLinksCallBacks.onGetShortLink?.Invoke(link.Url);
            }, taskScheduler);

        }
    }
}
