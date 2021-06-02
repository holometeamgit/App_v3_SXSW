using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
namespace Beem.Record.Video {
    /// <summary>
    /// Share video record
    /// </summary>
    public class ShareVideoRecordBtn : MonoBehaviour, IPointerDownHandler {
        [SerializeField]
        private VideoPlayer _videoPlayer;

        public void OnPointerDown(PointerEventData eventData) {
            if (!string.IsNullOrEmpty(_videoPlayer.url)) {
                new NativeShare().AddFile(_videoPlayer.url).Share();
            } else {
                Debug.LogError("Record path was empty");
            }
        }
    }
}
