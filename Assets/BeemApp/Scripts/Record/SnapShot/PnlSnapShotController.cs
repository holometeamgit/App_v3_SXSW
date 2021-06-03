using UnityEngine;
using UnityEngine.UI;

namespace Beem.Record.SnapShot {
    /// <summary>
    /// PnlSnapShotController
    /// </summary>
    public class PnlSnapShotController : MonoBehaviour {

        [Header("Snap shot Panel")]
        [SerializeField]
        private GameObject _pnl;

        private void OnEnable() {
            SnapShotCallBacks.onPostRecord += OnRecordComplete;
        }

        private void OnDisable() {
            SnapShotCallBacks.onPostRecord -= OnRecordComplete;
        }

        private void OnRecordComplete() {
            _pnl.SetActive(true);
        }
    }
}
