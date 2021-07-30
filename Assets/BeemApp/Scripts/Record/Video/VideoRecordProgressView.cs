using UnityEngine;
using UnityEngine.UI;

namespace Beem.Extenject.Record {

    /// <summary>
    /// Video Progress view
    /// </summary>
    public class VideoRecordProgressView : MonoBehaviour {

        [SerializeField]
        private Image progressBar;

        private void OnEnable() {
            OnProgress();
        }

        public void OnProgress(float value = 0f) {
            if (progressBar != null) {
                progressBar.fillAmount = value;
            }
        }
    }
}
