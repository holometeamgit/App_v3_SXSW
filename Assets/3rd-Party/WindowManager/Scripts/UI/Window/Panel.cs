using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindowManager.Extenject {

    /// <summary>
    /// Basic Panel
    /// </summary>
    public class Panel : MonoBehaviour, IShow, IHide {
        /// <summary>
        /// Hide Window
        /// </summary>
        public void Hide() {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Show Window
        /// </summary>
        public void Show() {
            gameObject.SetActive(true);
        }
    }
}
