using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility {

    /// <summary>
    /// Hide Interface in Screenshot
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class HideUI : MonoBehaviour {
        public static Action<bool> onActivate = delegate { };

        private CanvasGroup canvasGroup = default;

        private void OnEnable() {
            canvasGroup = GetComponent<CanvasGroup>();
            onActivate += Activate;
        }

        private void OnDisable() {
            onActivate -= Activate;
        }

        private void Activate(bool value) {
            canvasGroup.alpha = value ? 1 : 0;
        }
    }
}
