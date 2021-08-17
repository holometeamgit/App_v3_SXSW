using UnityEngine;
using UnityEngine.Events;

namespace Beem.Extenject.Video {

    /// <summary>
    /// Pause Action
    /// </summary>
    public class VideoPlayerPauseAction : MonoBehaviour {

        [SerializeField]
        private UnityEvent onPause;
        public void OnClick() {
            onPause?.Invoke();
        }
    }
}
