using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BlindOptionsWindow : MonoBehaviour {
    [SerializeField]
    private Mover _mover;

    [SerializeField]
    private Transform _parent;

    private List<GameObject> _blindViews = new List<GameObject>();
    private GameObject _currentView;

    public async void Show(string assetId, params object[] objects) {

        _currentView = await ShowView(assetId);

        _currentView.GetComponent<IBlindView>().Show(objects);


        gameObject.SetActive(true);
        _mover.ChangeState(true);
        _mover.onEndMoving += OnClose;
    }

    private async Task<GameObject> ShowView(string assetId) {
        if (!Contain(assetId)) {
            var handle = Addressables.LoadAssetAsync<GameObject>(assetId);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded) {
                GameObject tempView = Instantiate(handle.Task.Result);
                tempView.name = assetId;
                tempView.transform.SetParent(_parent);
                tempView.transform.localScale = Vector3.one;
                tempView.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                tempView.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                _blindViews.Add(tempView);
            }
        }

        foreach (var item in _blindViews) {
            item.GetComponent<IBlindView>().Hide();
        }

        GameObject tempAsset = Element(assetId);

        if (Contain(assetId)) {
            Addressables.ReleaseInstance(tempAsset);
        }

        return tempAsset;
    }

    private bool Contain(string assetId) {
        return _blindViews.Find(x => x.name == assetId) != null;
    }

    private GameObject Element(string assetId) {
        return _blindViews.Find(x => x.name == assetId);
    }

    private void HideView() {
        if (_currentView != null) {
            _currentView.GetComponent<IBlindView>().Hide();
        }
    }

    /// <summary>
    /// Hide Window
    /// </summary>
    public void Hide() {
        _mover.ChangeState(false);
    }

    private void OnClose(bool status) {
        if (!status) {
            _mover.onEndMoving -= OnClose;
            gameObject.SetActive(false);
            HideView();
        }
    }
}
