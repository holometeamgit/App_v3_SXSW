using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlHomeScreenV2 : MonoBehaviour
{

    [SerializeField] UIPullRefreshScrollController pullRefreshController;
    [SerializeField] ThumbnailPriorityScriptableObject thumbnailPriority;
    [SerializeField] ThumbnailWebDownloadManager thumbnailWebDownloadManager;

    [Space]
    [SerializeField] int pageSize = 10;

    private ThumbnailsData thumbnailsData;

    private void Awake() {
        thumbnailsData = new ThumbnailsData(thumbnailPriority.ThumbnailPriority, thumbnailWebDownloadManager, pageSize: 10);
        pullRefreshController.OnRefresh += RefreshItems;
    }

    private void RefreshItems() {
        FetchData();
    }

    public void FetchData() {
        thumbnailsData.RefreshData();
        //ClearData();
        //FetchEventStreamData();
    }
}
