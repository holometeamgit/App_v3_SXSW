using Beem.UI;
using DynamicScrollRect;
using Firebase.Messaging;
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
public class BusinessOptionsWindow : MonoBehaviour {

    [SerializeField]
    private CellView _cellView;
    [SerializeField]
    private GameObject _videoCell;

    private List<IARMsgDataView> _arMsgDataViews;

    private UserWebManager _userWebManager;

    private ARMsgJSON.Data _data = null;
    private bool _existPreview = true;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    /// <summary>
    /// Show data
    /// </summary>
    /// <param name="data"></param>
    public void Show(ARMsgJSON.Data data, bool existPreview) {
        _data = data;
        _existPreview = existPreview;
        gameObject.SetActive(true);
        _videoCell.SetActive(existPreview);
        _cellView?.Show(data, _userWebManager);
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(data));
    }

    /// <summary>
    /// Show data
    /// </summary>
    /// <param name="data"></param>
    public void Show() {
        if (_data != null) {
            Show(_data, _existPreview);
        }
    }

    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }

}
