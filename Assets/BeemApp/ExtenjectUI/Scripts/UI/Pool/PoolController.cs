using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Beem.Extenject.UI {
    /// <summary>
    /// Controller for pool methods and properties
    /// </summary>
    public class PoolController {
        private List<WindowElement> _windowPool = new List<WindowElement>();
        private WindowElement _currentWindowElement;
        private WindowElement _previousWindowElement;
        private IShow[] _showWindows = null;
        private IEscape[] _escapeWindows = null;

        /// <summary>
        /// Add Window in Pool
        /// </summary>
        /// <param name="windowObject"></param>
        /// <param name="element"></param>
        public void AddInPool(WindowObject windowObject, GameObject element) {
            WindowElement tempPoolElement = new WindowElement {
                Id = windowObject.Id,
                Element = element
            };
            _windowPool.Add(tempPoolElement);
        }

        /// <summary>
        /// Get Window From Pool
        /// </summary>
        /// <param name="windowObject"></param>
        /// <returns></returns>
        public WindowElement GetWindowInPool(WindowObject windowObject) {
            return _windowPool.Find(x => x.Id == windowObject.Id);
        }

        /// <summary>
        /// Check Window in Pool
        /// </summary>
        /// <param name="windowObject"></param>
        /// <returns></returns>
        public bool ContainInPool(WindowObject windowObject) {
            return _windowPool.Find(x => x.Id == windowObject.Id) != null;
        }

        /// <summary>
        /// Show Pool Element
        /// </summary>
        public void Show<T>(WindowObject windowObject, T parameter) {
            if (ContainInPool(windowObject)) {
                WindowElement tempWindow = GetWindowInPool(windowObject);
                tempWindow.Element.SetActive(true);
                _previousWindowElement = _currentWindowElement;
                _currentWindowElement = tempWindow;
                _escapeWindows = _currentWindowElement.Element.GetComponents<IEscape>();
                _showWindows = _currentWindowElement.Element.GetComponents<IShow>();
                if (_showWindows != null && _showWindows.Length > 0) {
                    _showWindows.ToList().ForEach(x => x.Show(parameter));
                }
            }
        }

        /// <summary>
        /// Show Pool Element
        /// </summary>
        public void Show(WindowObject windowObject) {
            if (ContainInPool(windowObject)) {
                WindowElement tempWindow = GetWindowInPool(windowObject);
                tempWindow.Element.SetActive(true);
                _previousWindowElement = _currentWindowElement;
                _currentWindowElement = tempWindow;
                _escapeWindows = _currentWindowElement.Element.GetComponents<IEscape>();
            }
        }

        /// <summary>
        /// Activate Pool Element
        /// </summary>
        public void Hide(WindowObject windowObject) {
            if (ContainInPool(windowObject)) {
                WindowElement tempWindow = GetWindowInPool(windowObject);
                tempWindow.Element.SetActive(false);
                _currentWindowElement = _previousWindowElement;
                _escapeWindows = _currentWindowElement.Element.GetComponents<IEscape>();
            }
        }

        /// <summary>
        /// Deactivate all Windows
        /// </summary>
        public void DeactivateAllPoolElements() {
            foreach (WindowElement item in _windowPool) {
                item.Element.SetActive(false);
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
