using UnityEngine;
using Zenject;

namespace WindowManager.Extenject {

    public enum WindowSignalsType {
        OpenWindow,
        OpenPopup,
        CloseWindow
    }

    /// <summary>
    /// Controller for windows
    /// </summary>
    public class WindowController : ITickable {
        private PoolController _poolController;

        [Inject]
        public void Construct(PoolController poolController) {
            _poolController = poolController;
        }


        /// <summary>
        /// Show/Hide window
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="windowSignals"></param>
        /// <param name="parameter"></param>
        public void CallWindow<T>(string id, WindowSignalsType windowSignalsType, T parameter) {
            switch (windowSignalsType) {
                case WindowSignalsType.OpenWindow:
                    OpenWindow(id, parameter);
                    break;
                case WindowSignalsType.OpenPopup:
                    OpenPopup(id, parameter);
                    break;
                case WindowSignalsType.CloseWindow:
                    CloseWindow(id, parameter);
                    break;
            }
        }

        public void CallWindow(string id, WindowSignalsType windowSignalsType) {
            switch (windowSignalsType) {
                case WindowSignalsType.OpenWindow:
                    OpenWindow(id);
                    break;
                case WindowSignalsType.OpenPopup:
                    OpenPopup(id);
                    break;
                case WindowSignalsType.CloseWindow:
                    CloseWindow(id);
                    break;
            }
        }

        private void OpenWindow<T>(string id, T parameter) {
            if (string.IsNullOrEmpty(id)) {
                Debug.LogError("id is null or empty");
                return;
            }

            _poolController.DeactivateAllPoolElements();

            OpenPopup(id, parameter);
        }

        private void OpenWindow(string id) {
            if (string.IsNullOrEmpty(id)) {
                Debug.LogError("id is null or empty");
                return;
            }

            _poolController.DeactivateAllPoolElements();

            OpenPopup(id);
        }

        private void OpenPopup<T>(string id, T parameter) {
            if (string.IsNullOrEmpty(id)) {
                Debug.LogError("id is null or empty");
                return;
            }

            _poolController.Show(id, parameter);
        }

        private void OpenPopup(string id) {
            if (string.IsNullOrEmpty(id)) {
                Debug.LogError("id is null or empty");
                return;
            }

            _poolController.Show(id);
        }

        private void CloseWindow(string id) {
            if (string.IsNullOrEmpty(id)) {
                Debug.LogError("id is null or empty");
                return;
            }

            _poolController.Hide(id);
        }

        private void CloseWindow<T>(string id, T parameter) {
            if (string.IsNullOrEmpty(id)) {
                Debug.LogError("id is null or empty");
                return;
            }

            _poolController.Hide(id, parameter);
        }

        public bool IsActive(string id) {
            return _poolController.IsActive(id);
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
