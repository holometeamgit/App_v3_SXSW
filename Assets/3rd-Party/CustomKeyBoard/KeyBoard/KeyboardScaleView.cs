using Mopsicus.Plugins;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.KeyBoard {
    /// <summary>
    /// Scale View
    /// </summary>
    public class KeyboardScaleView : AbstractKeyBoardSettings {

        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private float _baseHeight = 165;
        [SerializeField]
        private float _baseShift = 50;

        [SerializeField]
        private float _speed = 10;

        private CancellationTokenSource cancelTokenSource;

        public override void RefreshData(InputField inputField) {
            Cancel();
            if (inputField.textComponent.text.Length > 0) {
                ChangeInputHeight(_baseHeight + inputField.textComponent.preferredHeight - _baseShift);
            } else {
                ChangeInputHeight(_baseHeight);
            }
        }


        private async void ChangeInputHeight(float height) {

            cancelTokenSource = new CancellationTokenSource();
            try {
                while (Mathf.Abs(_rectTransform.sizeDelta.y - height) > 0.1f && !cancelTokenSource.IsCancellationRequested) {
                    _rectTransform.sizeDelta = Vector2.Lerp(_rectTransform.sizeDelta, new Vector2(_rectTransform.sizeDelta.x, height), Time.deltaTime * _speed);
                    await Task.Yield();
                }
            } finally {
                if (cancelTokenSource != null) {
                    cancelTokenSource.Dispose();
                    cancelTokenSource = null;
                }
            }
        }

        private void OnDisable() {
            Cancel();
        }

        /// <summary>
        /// Clear Info
        /// </summary>
        private void Cancel() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
            }
        }

    }
}
