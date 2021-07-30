using Beem.Extenject.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Record.SnapShot {
    /// <summary>
    /// SnapShot View
    /// </summary>

    [RequireComponent(typeof(RawImage))]
    public class SnapShotView : MonoBehaviour, IShow {

        private RawImage _rawImage;

        private void OnEnable() {
            _rawImage = GetComponent<RawImage>();
        }

        private void OnRecordComplete(Texture2D screenshot) {
            _rawImage.texture = screenshot;
        }

        public void Show<T>(T parameter) {
            Debug.Log(parameter.GetType());
            if (parameter is Texture2D) {
                OnRecordComplete(parameter as Texture2D);
            }
        }
    }
}
