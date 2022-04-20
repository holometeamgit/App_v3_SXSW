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
    private SwipePopUp _swipePopUp;
    [SerializeField]
    private GameObject _videoCell;

    private List<IARMsgDataView> _arMsgDataViews;

    private UserWebManager _userWebManager;

    private ARMsgJSON.Data _data = null;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    /// <summary>
    /// Show data
    /// </summary>
    /// <param name="data"></param>
    public void Show(ARMsgJSON.Data data, bool videoCell = true) {
        _data = data;
        gameObject.SetActive(true);
        _videoCell.SetActive(videoCell);
        _cellView?.Show(data, _userWebManager);
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(data));
        _swipePopUp.Show();
        _swipePopUp.onHid += OnClose;
    }

    /// <summary>
    /// Show data
    /// </summary>
    /// <param name="data"></param>
    public void Show(bool videoCell = true) {
        if (_data != null) {
            Show(_data, videoCell);
        }
    }

    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        _swipePopUp.Hide();
    }

    private void OnClose() {
        _swipePopUp.onHid -= OnClose;
        gameObject.SetActive(false);
    }
}
