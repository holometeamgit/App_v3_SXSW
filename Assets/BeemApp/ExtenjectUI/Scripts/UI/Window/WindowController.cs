using UnityEngine;
using Zenject;

namespace Beem.Extenject.UI {
    /// <summary>
    /// Controller for windows
    /// </summary>
    public class WindowController : ITickable {
        private PoolController _poolController;
        private DiContainer _diContainer;
        private Transform _parent;

        [Inject]
        public void Construct(PoolController poolController, DiContainer diContainer, Transform parent) {
            _poolController = poolController;
            _diContainer = diContainer;
            _parent = parent;
        }


        /// <summary>
        /// Show/Hide window
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="windowSignals"></param>
        /// <param name="parameter"></param>
        public void OnCalledSignal<T>(WindowSignal windowSignals, T parameter) {
            switch (windowSignals.WindowSignalsType) {
                case WindowSignalsType.OpenWindow:
                    OpenWindow(windowSignals.WindowObject, parameter);
                    break;
                case WindowSignalsType.OpenPopup:
                    OpenPopup(windowSignals.WindowObject, parameter);
                    break;
                case WindowSignalsType.CloseWindow:
                    CloseWindow(windowSignals.WindowObject);
                    break;
            }
        }

        public void OnCalledSignal(WindowSignal windowSignals) {
            switch (windowSignals.WindowSignalsType) {
                case WindowSignalsType.OpenWindow:
                    OpenWindow(windowSignals.WindowObject);
                    break;
                case WindowSignalsType.OpenPopup:
                    OpenPopup(windowSignals.WindowObject);
                    break;
                case WindowSignalsType.CloseWindow:
                    CloseWindow(windowSignals.WindowObject);
                    break;
            }
        }

        private void OpenWindow<T>(WindowObject windowObject, T parameter) {
            if (windowObject == null) {
                Debug.LogError("windowObject is absent");
                return;
            }

            _poolController.DeactivateAllPoolElements();

            OpenPopup(windowObject, parameter);
        }

        private void OpenWindow(WindowObject windowObject) {
            if (windowObject == null) {
                Debug.LogError("windowObject is absent");
                return;
            }

            _poolController.DeactivateAllPoolElements();

            OpenPopup(windowObject);
        }

        private void OpenPopup<T>(WindowObject windowObject, T parameter) {
            if (windowObject == null) {
                Debug.LogError("windowObject is absent");
                return;
            }

            CreateWindow(windowObject);

            _poolController.Show(windowObject, parameter);
        }

        private void OpenPopup(WindowObject windowObject) {
            if (windowObject == null) {
                Debug.LogError("windowObject is absent");
                return;
            }

            CreateWindow(windowObject);

            _poolController.Show(windowObject);
        }

        private void CloseWindow(WindowObject windowObject) {
            if (windowObject == null) {
                Debug.LogError("windowObject is absent");
                return;
            }

            _poolController.Hide(windowObject);
        }

        private void CreateWindow(WindowObject windowObject) {
            if (!_poolController.ContainInPool(windowObject)) {
                GameObject tempWindow = _diContainer.InstantiatePrefab(windowObject.WindowPrefab);
                tempWindow.transform.SetParent(_parent);
                tempWindow.name = windowObject.Id;
                _poolController.AddInPool(windowObject, tempWindow);
            }
        }

        /// <summary>
        /// Check Escape
        /// </summary>
        public void Tick() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                _poolController.Back();
            }
        }
    }
}
