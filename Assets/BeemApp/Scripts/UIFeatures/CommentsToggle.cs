using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.UI {

    /// <summary>
    /// Toggle for comments
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class CommentsToggle : MonoBehaviour, IStreamDataView {

        private StreamJsonData.Data _streamData = default;

        private Toggle _toggle;

        private void Awake() {
            _toggle = GetComponent<Toggle>();
        }

        public void Init(StreamJsonData.Data streamData) {
            _streamData = streamData;
        }

        /// <summary>
        /// Toggle comments
        /// </summary>
        public void ToggleComments(bool enable) {
            if (enable) {
                StreamCallBacks.onOpenComment?.Invoke((int)_streamData.id);
            } else {
                StreamCallBacks.onCloseComments?.Invoke();
            }
        }

        private void ForceCommentsToggleOff() {
            _toggle.isOn = false;
        }

        private void OnEnable() {
            StreamCallBacks.onCommentsClosed += ForceCommentsToggleOff;
            _toggle.onValueChanged.AddListener(ToggleComments);
        }

        private void OnDisable() {
            StreamCallBacks.onCommentsClosed -= ForceCommentsToggleOff;
            _toggle.onValueChanged.RemoveListener(ToggleComments);
        }
    }
}
