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

    private List<IStreamDataView> _streamDataViews;

    private StreamJsonData.Data _streamData = default;

    private bool newDataAssigned = false;

    private bool isPinned = false;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamData">Stream Json data</param>
    public void Init(StreamJsonData.Data streamData) {

        newDataAssigned = true;
        _streamData = streamData;
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

        _streamDataViews = GetComponentsInChildren<IStreamDataView>().ToList();

        _streamDataViews.ForEach(x => x.Init(_streamData));
    }



    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }
}

