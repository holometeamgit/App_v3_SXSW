using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Beem.UI;

/// <summary>
/// Bar for Prerecorded Video
/// </summary>
public class PrerecordedVideoBar : MonoBehaviour {

    private List<IStreamDataView> _streamDataViews;

    [SerializeField]
    private StreamLikesRefresherView streamLikesRefresherView;
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamData">Stream Json data</param>
    public void Init(StreamJsonData.Data streamData) {

        _streamDataViews = GetComponentsInChildren<IStreamDataView>().ToList();

        _streamDataViews.ForEach(x => x.Init(streamData));

        gameObject.SetActive(true);
        streamLikesRefresherView?.StartCountAsync(streamData.id.ToString());
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }
}

