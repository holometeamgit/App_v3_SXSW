using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Beem.UI;

/// <summary>
/// Bar for Prerecorded Video
/// </summary>
public class PnlARMessages : MonoBehaviour {

    [Header("Hologram manager")]
    [SerializeField]
    private HologramHandler _hologramHandler;

    private List<IARMsgDataView> _arMsgDataViews;

    private ARMsgJSON.Data _arMsgJSON = default;

    private bool newDataAssigned = false;

    private bool isPinned = false;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamData">Stream Json data</param>
    public void Init(ARMsgJSON.Data arMsgJSON) {

        newDataAssigned = true;
        _arMsgJSON = arMsgJSON;
        if (isPinned) {
            Refresh();
        } else {
#if !UNITY_EDITOR
            Deactivate();
#else
            Refresh();
#endif
        }

        _hologramHandler.SetOnPlacementUIHelperFinished(Refresh);

    }

    private void Refresh() {
        if (!newDataAssigned) {
            return;
        }

        isPinned = true;

        gameObject.SetActive(true);

        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(_arMsgJSON));
    }



    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }
}

