using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Video {

    /// <summary>
    /// Pause Action
    /// </summary>
    public class VideoPlayerPauseAction : MonoBehaviour {
        public void OnClick() {
            VideoPlayerCallBacks.onPause?.Invoke();
        }
    }
}
