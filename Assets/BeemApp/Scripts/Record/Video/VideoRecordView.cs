using Beem.Extenject.UI;
using UnityEngine;
using UnityEngine.Video;

namespace Beem.Record.Video {


    /// <summary>
    /// VideoRecord View
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoRecordView : MonoBehaviour, IShow {
        private VideoPlayer _videoPlayer;

        private void OnEnable() {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        private void OnRecordComplete(string outpath) {
            _videoPlayer.url = outpath;
            _videoPlayer.Play();
        }

        public void Show<T>(T parameter) {
            Debug.Log(parameter.GetType());
            if (parameter is string) {
                OnRecordComplete(parameter as string);
            }
        }
    }
}
