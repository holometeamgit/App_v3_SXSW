using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlHomeScreenV2 : MonoBehaviour
{

    [SerializeField] UIPullRefreshScrollController pullRefreshController;
    [SerializeField] ThumbnailPriorityScriptableObject thumbnailPriority;
    [SerializeField] ThumbnailWebDownloadManager thumbnailWebDownloadManager;
    [SerializeField] UIThumbnailsController uiThumbnailsController;

    [Space]
    [SerializeField] int pageSize = 10;

    private ThumbnailsDataFetcher thumbnailsDataFetcher;
    private ThumbnailsDataContainer thumbnailsDataContainer;

    bool dataLoaded;
    bool initialized;

    private void Awake() {
        pullRefreshController.OnRefresh += RefreshItems;
        pullRefreshController.OnReachedBottom += GetNextPage;

        thumbnailsDataContainer = new ThumbnailsDataContainer();

        thumbnailsDataContainer.OnDataUpdated += DataUpdateCallBack;

        thumbnailsDataFetcher = new ThumbnailsDataFetcher(thumbnailPriority.ThumbnailPriority, thumbnailWebDownloadManager, thumbnailsDataContainer, pageSize: pageSize );
        uiThumbnailsController.SetStreamJsonData(thumbnailsDataContainer.GetDataList());

        thumbnailsDataFetcher.OnAllDataLoaded += AllDataLoaded;

        uiThumbnailsController.OnUpdated += UIUpdated;
    }

    private void DataUpdateCallBack() {
        initialized = true;
        uiThumbnailsController.UpdateData();
    }

    private void UIUpdated() {
        pullRefreshController.EndRefreshing();
    }

    private void RefreshItems() {
        Debug.Log("RefreshItems");
        dataLoaded = false;
        pullRefreshController.StopBottomRefreshing = false;
        thumbnailsDataFetcher.RefreshData();
    }

    private void GetNextPage() {
        if (!initialized)
            RefreshItems();
        else
            if (!dataLoaded)
                thumbnailsDataFetcher.GetNextPage();
    }

    private void AllDataLoaded() {
        Debug.Log(" AllDataLoaded Data loaded");
        dataLoaded = true;
        pullRefreshController.StopBottomRefreshing = true;
    }
}
