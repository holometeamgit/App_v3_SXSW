using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Beem.Video {

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
