using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Beem.Record.Video {

    /// <summary>
    /// Video Progress view
    /// </summary>
    public class VideoRecordView : MonoBehaviour {

        [SerializeField]
        private Image progressBar;

        private void OnEnable() {
            OnProgress();
            VideoRecordCallbacks.onRecordProgress += OnProgress;
        }

        private void OnDisable() {
            VideoRecordCallbacks.onRecordProgress -= OnProgress;
        }

        private void OnProgress(float value = 0f) {
            if (progressBar != null) {
                progressBar.fillAmount = value;
            }
        }
    }
}
