using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace WindowManager.Extenject {
    /// <summary>
    /// Controller for pool methods and properties
    /// </summary>
    public class PoolController {
        private GameObject _currentWindowElement;
        private GameObject _previousWindowElement;
        private IEscape[] _escapeWindows = null;

        private Transform _parent;
        private DiContainer _diContainer;

        private List<GameObject> _cachedObject = new List<GameObject>();

        [Inject]
        public void Construct(Transform parent, DiContainer diContainer) {
            _parent = parent;
            _diContainer = diContainer;
        }

        private bool Contain(string assetId) {
            return _cachedObject.Find(x => x.name == assetId) != null;
        }

        public bool IsActive(string assetId) {
            if (Contain(assetId)) {
                return Element(assetId).activeInHierarchy;
            }
            return false;
        }

        private GameObject Element(string assetId) {
            return _cachedObject.Find(x => x.name == assetId);
        }

        private async Task<GameObject> Load(string assetId) {
            if (!Contain(assetId)) {
                var handle = Addressables.LoadAssetAsync<GameObject>(assetId);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded) {
                    GameObject tempWindow = _diContainer.InstantiatePrefab(handle.Task.Result);
                    tempWindow.name = assetId;
                    _cachedObject.Add(tempWindow);
                }
            }

            GameObject tempAsset = Element(assetId);

            if (Contain(assetId)) {
                Addressables.ReleaseInstance(tempAsset);

            }
            return tempAsset;
        }

        public void DeactivateAllPoolElements() {
            _cachedObject.ForEach(x => {
                IHide[] _hideWindows = x.GetComponentsInChildren<IHide>();
                if (_hideWindows != null && _hideWindows.Length > 0) {
                    _hideWindows.ToList().ForEach(x => x.Hide());
                }
            }
            );
        }

        /// <summary>
        /// Show Pool Element
        /// </summary>
        public async void Show(string id) {

            GameObject tempWindow = await Load(id);
            tempWindow.transform.SetParent(_parent);

            _previousWindowElement = _currentWindowElement;
            _currentWindowElement = tempWindow;
            _escapeWindows = _currentWindowElement.GetComponentsInChildren<IEscape>();

            IShow[] _showWindows = _currentWindowElement.GetComponentsInChildren<IShow>();
            if (_showWindows != null && _showWindows.Length > 0) {
                _showWindows.ToList().ForEach(x => x.Show());
            }
        }


        /// <summary>
        /// Show Pool Element
        /// </summary>
        public void Show<T>(string id, T parameter) {

            Show(id);

            IShowWithParam[] _showWithParamWindows = _currentWindowElement.GetComponentsInChildren<IShowWithParam>();
            if (_showWithParamWindows != null && _showWithParamWindows.Length > 0) {
                _showWithParamWindows.ToList().ForEach(x => x.Show(parameter));
            }
        }

        /// <summary>
        /// Hide Pool Element
        /// </summary>
        public void Hide(string id) {
            if (Contain(id)) {
                _currentWindowElement = _previousWindowElement;

                _escapeWindows = _currentWindowElement.GetComponents<IEscape>();

                GameObject tempAsset = Element(id);
                IHide[] _hideWindows = tempAsset.GetComponentsInChildren<IHide>();
                if (_hideWindows != null && _hideWindows.Length > 0) {
                    _hideWindows.ToList().ForEach(x => x.Hide());
                }

            }

        }

        /// <summary>
        /// Hide Pool Element
        /// </summary>
        public void Hide<T>(string id, T parameter) {
            Hide(id);

            if (Contain(id)) {
                GameObject tempAsset = Element(id);
                IHideWithParam[] _hideWindows = tempAsset.GetComponentsInChildren<IHideWithParam>();
                if (_hideWindows != null && _hideWindows.Length > 0) {
                    _hideWindows.ToList().ForEach(x => x.Hide(parameter));
                }

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
