using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Beem.Extenject.UI {
    /// <summary>
    /// Controller for pool methods and properties
    /// </summary>
    public class PoolController {
        private Dictionary<string, GameObject> _windowPool = new Dictionary<string, GameObject>();
        private GameObject _currentWindowElement;
        private GameObject _previousWindowElement;
        private IShow[] _showWindows = null;
        private IEscape[] _escapeWindows = null;

        /// <summary>
        /// Add Window in Pool
        /// </summary>
        /// <param name="windowObject"></param>
        /// <param name="element"></param>
        public void AddInPool(WindowObject windowObject, GameObject element) {
            _windowPool.Add(windowObject.Id, element);
        }

        /// <summary>
        /// Get Window From Pool
        /// </summary>
        /// <param name="windowObject"></param>
        /// <returns></returns>
        public GameObject GetWindowInPool(WindowObject windowObject) {
            return _windowPool[windowObject.Id];
        }

        /// <summary>
        /// Check Window in Pool
        /// </summary>
        /// <param name="windowObject"></param>
        /// <returns></returns>
        public bool ContainInPool(WindowObject windowObject) {
            return _windowPool.ContainsKey(windowObject.Id);
        }

        /// <summary>
        /// Show Pool Element
        /// </summary>
        public void Show<T>(WindowObject windowObject, T parameter) {
            Show(windowObject);
            _showWindows = _currentWindowElement.GetComponentsInChildren<IShow>();
            if (_showWindows != null && _showWindows.Length > 0) {
                _showWindows.ToList().ForEach(x => x.Show(parameter));
            }
        }

        /// <summary>
        /// Show Pool Element
        /// </summary>
        public void Show(WindowObject windowObject) {
            if (ContainInPool(windowObject)) {
                GameObject tempWindow = GetWindowInPool(windowObject);
                tempWindow.SetActive(true);
                _previousWindowElement = _currentWindowElement;
                _currentWindowElement = tempWindow;
                _escapeWindows = _currentWindowElement.GetComponentsInChildren<IEscape>();
            }
        }

        /// <summary>
        /// Activate Pool Element
        /// </summary>
        public void Hide(WindowObject windowObject) {
            if (ContainInPool(windowObject)) {
                GameObject tempWindow = GetWindowInPool(windowObject);
                tempWindow.SetActive(false);
                _currentWindowElement = _previousWindowElement;
                _escapeWindows = _currentWindowElement.GetComponentsInChildren<IEscape>();
            }
        }

        /// <summary>
        /// Deactivate all Windows
        /// </summary>
        public void DeactivateAllPoolElements() {
            foreach (KeyValuePair<string, GameObject> item in _windowPool) {
                item.Value.SetActive(false);
            }
        }

        /// <summary>
        /// Escape Last Element
        /// </summary>
        public void Back() {
            if (_escapeWindows != null && _escapeWindows.Length > 0) {
                _escapeWindows.ToList().ForEach(x => x.Escape());
            }
        }
    }
}
