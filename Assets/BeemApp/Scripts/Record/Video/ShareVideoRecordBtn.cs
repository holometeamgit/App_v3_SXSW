using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
namespace Beem.Record.Video {
    /// <summary>
    /// Share video record
    /// </summary>
    public class ShareVideoRecordBtn : MonoBehaviour, IPointerDownHandler {

        private string _outpath;

        private void OnEnable() {
            VideoRecordCallbacks.onRecordFinished += OnRecordComplete;
        }

        private void OnDisable() {
            VideoRecordCallbacks.onRecordFinished -= OnRecordComplete;
        }

        private void OnRecordComplete(string outpath) {
            _outpath = outpath;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (!string.IsNullOrEmpty(_outpath)) {
                new NativeShare().AddFile(_outpath).Share();
            } else {
                Debug.LogError("Record path was empty");
            }
        }
    }
}
