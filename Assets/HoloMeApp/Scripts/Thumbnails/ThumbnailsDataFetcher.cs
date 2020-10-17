using System;
using System.Collections.Generic;
using UnityEngine;

public class ThumbnailsDataFetcher {

    public Action OnAllDataLoaded; //all data from the server has been downloaded
    public Action OnErrorGetCountThumbnails;
    public Action OnErrorGetThumbnails;

    private ThumbnailWebDownloadManager thumbnailWebDownloadManager;
    private ThumbnailsFilter thumbnailsFilter;
    private ThumbnailPriority thumbnailPriority;
    private ThumbnailsDataContainer thumbnailsDataContainer;
    private int currentPriority;

    private int pageSize = 8;
    private int currentPage = 1;

    private LoadingKey currentLoadingKey;

    public ThumbnailsDataFetcher(ThumbnailPriority thumbnailPriority,
        ThumbnailWebDownloadManager downloadManager,
        ThumbnailsDataContainer thumbnailsDataContainer,
        ThumbnailsFilter thumbnailsFilter = null, int pageSize = 10) {
        this.thumbnailPriority = thumbnailPriority;
        thumbnailWebDownloadManager = downloadManager;
        this.thumbnailsFilter = thumbnailsFilter;
        this.pageSize = pageSize;

        this.thumbnailsDataContainer = thumbnailsDataContainer;

        thumbnailWebDownloadManager.OnCountThumbnailsLoaded += PageCountCallBack;
        thumbnailWebDownloadManager.OnErrorCountThumbnailsLoaded += ErrorPageCountCallBack;

        thumbnailWebDownloadManager.OnStreamJsonDataLoaded += GetThumbnailsOnCurrentPageCallBack;
        thumbnailWebDownloadManager.OnErrorStreamJsonDataLoaded += ErrorGetThumbnailsOnCurrentPageCallBack;
    }

    public void RefreshData() {
        currentLoadingKey = new LoadingKey(this);
        currentPriority = 0;
        thumbnailsDataContainer.Refresh();
        GetPageCount();
    }

    public void GetNextPage() {
        currentLoadingKey = new LoadingKey(this);

        if (currentPage >= 1)
            GetThumbnailsOnCurrentPage();
        else {
            currentPriority++;
            if (thumbnailPriority.Stages.Count <= currentPriority) {
                OnAllDataLoaded?.Invoke();
                return;
            }
            GetPageCount();
        }
    }

    #region page count and continue get thumbnails 

    private void GetPageCount() {
        if (thumbnailPriority.Stages.Count <= currentPriority)
            return;

        ThumbnailWebDownloadManager.ThumbnailWebRequestStruct thumbnailWebRequestStruct =
            new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(thumbnailPriority.Stages[currentPriority], 1, pageSize, thumbnailsFilter);

        thumbnailWebDownloadManager.GetCountThumbnails(thumbnailWebRequestStruct, currentLoadingKey);

    }

    private void PageCountCallBack(int count, LoadingKey loadingKey) {
//        Debug.Log("PageCountCallBack " + loadingKey.ToString());
        if (loadingKey != currentLoadingKey)
            return;

        currentPage = Mathf.Max(Mathf.CeilToInt((float)count / pageSize), 1);
        Debug.Log("currentPage = " + currentPage + " count = " + count);

        GetThumbnailsOnCurrentPage();
    }

    private void ErrorPageCountCallBack(long code, string body, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;
        Debug.Log(code + " " + body);
        OnErrorGetCountThumbnails?.Invoke();
    }

    #endregion

    #region get new Thumbnails

    private void GetThumbnailsOnCurrentPage() {
        ThumbnailWebDownloadManager.ThumbnailWebRequestStruct thumbnailWebRequestStruct =
            new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(thumbnailPriority.Stages[currentPriority], currentPage, pageSize, thumbnailsFilter);

        thumbnailWebDownloadManager.DownloadThumbnails(thumbnailWebRequestStruct, currentLoadingKey);
    }

    private void GetThumbnailsOnCurrentPageCallBack(StreamJsonData streamJsonData, LoadingKey loadingKey) {
//        Debug.Log("GetThumbnailsOnCurrentPageCallBack " + loadingKey.ToString());
        if (loadingKey != currentLoadingKey)
            return;

        thumbnailsDataContainer.AddListStreamJsonData(streamJsonData);
        currentPage--;
    }

    private void ErrorGetThumbnailsOnCurrentPageCallBack(long code, string body, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;
        Debug.Log(code + " " + body);
        OnErrorGetThumbnails?.Invoke();
    }

    #endregion

    ~ThumbnailsDataFetcher() {
        thumbnailWebDownloadManager.OnCountThumbnailsLoaded -= PageCountCallBack;
        thumbnailWebDownloadManager.OnErrorCountThumbnailsLoaded -= ErrorPageCountCallBack;

        thumbnailWebDownloadManager.OnStreamJsonDataLoaded -= GetThumbnailsOnCurrentPageCallBack;
        thumbnailWebDownloadManager.OnErrorStreamJsonDataLoaded -= ErrorGetThumbnailsOnCurrentPageCallBack;
    }
}
