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
    private GameObject _playBtn;

    private List<IARMsgDataView> _arMsgDataViews;

    private UserWebManager _userWebManager;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    /// <summary>
    /// Show data
    /// </summary>
    /// <param name="data"></param>
    public void Show(ARMsgJSON.Data data) {
        gameObject.SetActive(true);
        _playBtn.SetActive(true);
        _cellView?.Show(data, _userWebManager);
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(data));
    }

    /// <summary>
    /// Close Business Options
    /// </summary>
    public void Close() {
        BusinessOptionsConstructor.OnHide?.Invoke();
    }


    /// <summary>
    /// Hide
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
    }
}
