using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Application Version
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class ApplicationVersionView : MonoBehaviour {

        private Text _txt;

        private void Awake() {
            _txt = GetComponent<Text>();
        }

        private void Start() {
            _txt.text = "Version " + Application.version;
        }
    }
}
