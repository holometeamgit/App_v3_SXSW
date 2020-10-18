using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlHomeScreenV2 : MonoBehaviour
{

    [SerializeField] UIPullRefreshScrollController pullRefreshController;
    [SerializeField] UIThumbnailsController uiThumbnailsController;
    [SerializeField] ThumbnailPriorityScriptableObject thumbnailPriority;
    [SerializeField] ThumbnailWebDownloadManager thumbnailWebDownloadManager;
    [SerializeField] ThumbnailsPurchaser thumbnailsPurchaser;


    [Space]
    [SerializeField] int pageSize = 10;

    private ThumbnailsDataFetcher thumbnailsDataFetcher;

    bool dataLoaded;
    bool initialized;

    private void Awake() {
        pullRefreshController.OnRefresh += RefreshItems;
        pullRefreshController.OnReachedBottom += GetNextPage;

        thumbnailsDataFetcher =
            new ThumbnailsDataFetcher(thumbnailPriority.ThumbnailPriority,
            thumbnailWebDownloadManager, pageSize: pageSize );

        thumbnailsDataFetcher.OnAllDataLoaded += AllDataLoaded;
        thumbnailsDataFetcher.OnDataUpdated += DataUpdateCallBack;

        uiThumbnailsController.OnUpdated += UIUpdated;
        uiThumbnailsController.SetStreamJsonData(thumbnailsDataFetcher.GetDataList());

        thumbnailsPurchaser.SetStreamJsonData(thumbnailsDataFetcher.GetDataList());
    }

    private void DataUpdateCallBack() {
        initialized = true;
        pullRefreshController.StopBottomRefreshing = false;
        uiThumbnailsController.UpdateData();
    }

    private void UIUpdated() {
        StartCoroutine(EndingUIUpdate());
    }

    private void RefreshItems() {
        Debug.Log("RefreshItems");
        dataLoaded = false;
        //pullRefreshController.StopBottomRefreshing = false;
        thumbnailsDataFetcher.RefreshData();
    }

    private void GetNextPage() {
        if (!initialized)
            RefreshItems();
        else {
            if (!dataLoaded)
                thumbnailsDataFetcher.GetNextPage();
        }
    }

    private void AllDataLoaded() {
        Debug.Log(" AllDataLoaded Data loaded");
        dataLoaded = true;
        pullRefreshController.StopBottomRefreshing = true;
        pullRefreshController.EndRefreshing();
        uiThumbnailsController.RemoveUnnecessary();
    }

    private void OnDisable() {
        StopAllCoroutines();
        pullRefreshController.EndRefreshing();
    }

    IEnumerator EndingUIUpdate() {
        pullRefreshController.RefreshLayout();
        yield return new WaitForSeconds(2);
        Debug.Log("IEnumerator EndingUIUpdate");
        pullRefreshController.EndRefreshing();
    }
}
