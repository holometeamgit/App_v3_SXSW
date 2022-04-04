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
    private HologramHandler _hologramHandler;

    private List<IARMsgDataView> _arMsgDataViews;

    [Inject]
    public void Construct(HologramHandler hologramHandler) {
        _hologramHandler = hologramHandler;
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamData">Stream Json data</param>
    public void Init(ARMsgJSON.Data arMsgJSON) {

        gameObject.SetActive(true);
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(arMsgJSON));

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

