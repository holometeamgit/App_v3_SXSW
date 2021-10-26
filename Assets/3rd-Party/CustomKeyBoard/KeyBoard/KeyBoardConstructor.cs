using Mopsicus.Plugins;
using NiceJson;
using System;
using UnityEngine;

namespace Beem.KeyBoard {
    /// <summary>
    /// KeyBoard Constructor
    /// </summary>
    public class KeyBoardConstructor : MonoBehaviour {
        [SerializeField]
        private GameObject _keyboardField;
        [SerializeField]
        private MobileInputField _mobileInputField;
        [SerializeField]
        private KeyBoardPositionView _positionSettingsView;

        public static Action<bool> onShow = delegate { };

        private const string DATA = "data";
        private const string HEIGHT = "height";
        private const string SHOW = "show";


        private void Awake() {
            Construct();
        }

        private void Construct() {
            Plugins.onJsonInit += Init;
            onShow += Show;
        }

        private void Show(bool isShown) {
            _mobileInputField.SetFocus(isShown);
            _keyboardField.SetActive(isShown);
            _positionSettingsView.UpdatePosition();
        }

        private void OnDestroy() {
            Plugins.onJsonInit -= Init;
            onShow -= Show;
        }

        private void Init(JsonObject data) {
            string dataParse = data[DATA].ToString();
            /*if (dataParse.Contains(HEIGHT)) {
                JsonObject info = (JsonObject)JsonNode.ParseJsonString(dataParse);
                _positionSettingsView.UpdatePosition(int.Parse(info[HEIGHT].ToString()));
            }
            if (dataParse.Contains(SHOW)) {
                JsonObject info = (JsonObject)JsonNode.ParseJsonString(dataParse);
                Show((bool)info[SHOW]);
            }*/
        }
    }
}
