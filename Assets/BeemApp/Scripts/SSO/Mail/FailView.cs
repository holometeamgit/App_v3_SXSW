using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.SSO {
    /// <summary>
    /// Fail View
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class FailView : MonoBehaviour {

        private Text text;

        private void Awake() {
            text = GetComponent<Text>();
        }

        private void OnEnable() {
            CallBacks.onFail += Fail;
        }

        private void OnDisable() {
            CallBacks.onFail -= Fail;
        }

        private void Fail(string authError) {
            text.text = authError;
        }
    }
}
