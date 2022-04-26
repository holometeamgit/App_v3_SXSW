using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Beem.UI;
using Beem.Permissions;
using Zenject;

/// <summary>
/// Bar for Prerecorded Video
/// </summary>
public class PnlARMessages : MonoBehaviour {
    [SerializeField]
    private GameObject _deleteBtn;

    private HologramHandler _hologramHandler;
    private UserWebManager _userWebManager;

    private List<IARMsgDataView> _arMsgDataViews;

    [Inject]
    public void Construct(HologramHandler hologramHandler, UserWebManager userWebManager) {
        _hologramHandler = hologramHandler;
        _userWebManager = userWebManager;
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamData">Stream Json data</param>
    public void Init(ARMsgJSON.Data arMsgJSON) {

        gameObject.SetActive(true);
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(arMsgJSON));

        _deleteBtn.SetActive(arMsgJSON.user == _userWebManager.GetUsername());

        _hologramHandler.SetOnPlacementUIHelperFinished(OnPlacementCompleted);
    }

    private void OnPlacementCompleted() {
        RecordARConstructor.OnActivated?.Invoke(true);
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        RecordARConstructor.OnActivated?.Invoke(false);
        gameObject.SetActive(false);
    }
}

