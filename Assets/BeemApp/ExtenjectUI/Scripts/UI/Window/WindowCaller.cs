using UnityEngine;
using Zenject;

namespace Beem.Extenject.UI {
    /// <summary>
    /// Call window from different sources
    /// </summary>
    public class WindowCaller : MonoBehaviour {
        [Header("Window Function")]
        [SerializeField]
        protected WindowSignal _windowSignals;

        protected WindowController _windowController;

        [Inject]
        public void Construct(WindowController windowController) {
            _windowController = windowController;
        }

        /// <summary>
        /// Call Window
        /// </summary>
        public void CallWindow() {
            _windowController.OnCalledSignal(_windowSignals);
        }
    }
}
