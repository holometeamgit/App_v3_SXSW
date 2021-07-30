using Beem.Extenject.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Extenject.Record.Video {
    /// <summary>
    /// Share video record
    /// </summary>
    public class ShareVideoRecordBtn : MonoBehaviour, IPointerDownHandler, IShow {

        private string _outpath;

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

        public void Show<T>(T parameter) {
            if (parameter is string) {
                OnRecordComplete(parameter as string);
            }
        }
    }
}
