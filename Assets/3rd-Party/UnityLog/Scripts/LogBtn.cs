using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Open/Close LogScreen
    /// </summary>
    public class LogBtn : MonoBehaviour, IPointerClickHandler {

        [Header("Open")]
        [SerializeField]
        private bool _open;

        private static GameObject _logObject;

        private const string LOGGER = "Logger";

        public void OnPointerClick(PointerEventData eventData) {
            if (_open) {
                _logObject = Instantiate(Resources.Load(LOGGER) as GameObject);
            } else {
                if (_logObject != null) {
                    Destroy(_logObject);
                }
            }
        }
    }
}
