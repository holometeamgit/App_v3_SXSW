using UnityEngine;
using UnityEngine.Events;

namespace Beem.Extenject.UI {

    /// <summary>
    /// Escape Action
    /// </summary>
    public class EscapeAction : MonoBehaviour, IEscape {

        [Header("Event on Escape")]
        [SerializeField]
        private UnityEvent onEscape;

        /// <summary>
        /// Escape
        /// </summary>
        public void Escape() {
            onEscape?.Invoke();
        }

    }
}
