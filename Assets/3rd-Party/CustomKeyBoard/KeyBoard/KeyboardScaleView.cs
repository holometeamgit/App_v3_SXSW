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
        private RectTransform _inputRectTranform;
        [Space]
        [SerializeField]
        private float _baseShift = 60;

        public override void RefreshData(InputField inputField) {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _inputRectTranform.sizeDelta.y + _baseShift);
        }

    }
}
