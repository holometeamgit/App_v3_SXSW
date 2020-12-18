﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PnlHomeScreenV2 : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    //Pull refresh
    [SerializeField] UIPullRefreshScrollController pullRefreshController;
    //controller uithumbnails 
    [SerializeField] UIThumbnailsController uiThumbnailsController;
    //filter for downloadable thumbnails 
    [SerializeField] ThumbnailPriorityScriptableObject thumbnailPriority;
    //ThumbnailWebDownloadManager need for data fetcher  
    [SerializeField] ThumbnailWebDownloadManager thumbnailWebDownloadManager;
    //puchase pnl
    [SerializeField] PnlEventPurchaser pnlEventPurchaser;

    [Space]
    [SerializeField] int pageSize = 10;

    //ThumbnailsDataFetcher take json pages with thumbnails 
    private ThumbnailsDataFetcher thumbnailsDataFetcher;

    bool dataLoaded;
    bool initialized;

    public UnityEvent OnPlay;

    public UnityEvent OnRefresh;
    public UnityEvent OnAllDataLoaded;

    public void SetDefaultState() {
        scrollRect.verticalNormalizedPosition = 1;
    }

    private void Awake() {
        pullRefreshController.OnRefresh += RefreshItems;
        pullRefreshController.OnReachedBottom += GetNextPage;

        thumbnailsDataFetcher =
            new ThumbnailsDataFetcher(thumbnailPriority.ThumbnailPriority,
            thumbnailWebDownloadManager, pageSize: pageSize );

        thumbnailsDataFetcher.OnAllDataLoaded += AllDataLoaded;
        thumbnailsDataFetcher.OnDataUpdated += DataUpdateCallBack;

        uiThumbnailsController.OnUpdated += UIUpdated;
        //add ref to data list from fetcher for ui Thumbnails Controller
        uiThumbnailsController.SetStreamJsonData(thumbnailsDataFetcher.GetDataList());
        uiThumbnailsController.OnPlay += OnPlayCallBack;
        uiThumbnailsController.OnNeedPurchase += OnNeedPurchaseCallBack;

        pnlEventPurchaser.OnServerPurchasedDataUpdate += RefreshItems;
    }

    private void DataUpdateCallBack() {
        initialized = true;
        pullRefreshController.StopBottomRefreshing = false;
        uiThumbnailsController.UpdateData();
    }

    private void UIUpdated() {
        pullRefreshController.RefreshLayout();
        Invoke("EndingUIUpdate", 0.05f);
    }

    private void RefreshItems() {
        Resources.UnloadUnusedAssets();
//        Debug.Log("RefreshItems");
        dataLoaded = false;
        //pullRefreshController.StopBottomRefreshing = false;
        thumbnailsDataFetcher.RefreshData();
        OnRefresh.Invoke();
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
        OnAllDataLoaded.Invoke();//temp
    }

    private void OnDisable() {
        StopAllCoroutines();
        pullRefreshController.EndRefreshing();
    }

    private void OnNeedPurchaseCallBack(StreamJsonData.Data data) {
        Debug.Log("Home page OnClickCallBack");
        pnlEventPurchaser.Show(data);
    }

    private void OnPlayCallBack(StreamJsonData.Data data) {
        Debug.Log("Home page OnPlayCallBack");
        OnPlay.Invoke();
    }

    private void EndingUIUpdate() {
//        Debug.Log("IEnumerator EndingUIUpdate");
        pullRefreshController.EndRefreshing();
        pullRefreshController.RefreshLayout();
        pullRefreshController.RefreshLayout();
    }
}
