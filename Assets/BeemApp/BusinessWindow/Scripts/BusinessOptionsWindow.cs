using Beem.UI;
using DynamicScrollRect;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Business View
/// </summary>
public class BusinessOptionsWindow : MonoBehaviour, IBlindView {

    [SerializeField]
    private CellView _cellView;
    [SerializeField]
    private GameObject _videoCell;

    private List<IARMsgDataView> _arMsgDataViews;

    private UserWebManager _userWebManager;
    private WebRequestHandler _webRequestHandler;

    private ARMsgJSON.Data _data = null;
    private bool _existPreview = true;

    /// <summary>
    /// Show data
    /// </summary>
    /// <param name="data"></param>
    public void Show(params object[] objects) {

        if (objects != null && objects.Length > 0) {
            foreach (var item in objects) {
                if (item is bool) {
                    _existPreview = Convert.ToBoolean(item);
                } else if (item is ARMsgJSON.Data) {
                    _data = item as ARMsgJSON.Data;
                } else if (item is UserWebManager) {
                    _userWebManager = item as UserWebManager;
                } else if (item is WebRequestHandler) {
                    _webRequestHandler = item as WebRequestHandler;
                }
            }
        }

        if (_data != null && _userWebManager != null && _webRequestHandler != null) {

            gameObject.SetActive(true);
            _videoCell.SetActive(_existPreview);
            _cellView?.Show(_data, _userWebManager, _webRequestHandler);
            _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

            _arMsgDataViews.ForEach(x => x.Init(_data));
        }
    }

    /// <summary>
    /// Hide
    /// </summary>

    public void Hide() {
        gameObject.SetActive(false);
    }
}
