using UnityEngine;
using Zenject;

namespace WindowManager.Extenject {
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
        public void OnCalledSignal<T>(WindowSignal windowSignals, T parameter) {
            switch (windowSignals.WindowSignalsType) {
                case WindowSignalsType.OpenWindow:
                    OpenWindow(windowSignals.Id, parameter);
                    break;
                case WindowSignalsType.OpenPopup:
                    OpenPopup(windowSignals.Id, parameter);
                    break;
                case WindowSignalsType.CloseWindow:
                    CloseWindow(windowSignals.Id);
                    break;
            }
        }

        public void OnCalledSignal(WindowSignal windowSignals) {
            switch (windowSignals.WindowSignalsType) {
                case WindowSignalsType.OpenWindow:
                    OpenWindow(windowSignals.Id);
                    break;
                case WindowSignalsType.OpenPopup:
                    OpenPopup(windowSignals.Id);
                    break;
                case WindowSignalsType.CloseWindow:
                    CloseWindow(windowSignals.Id);
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
