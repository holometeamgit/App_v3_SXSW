using System;
using System.Collections.Generic;
using UnityEngine;

public class ThumbnailsData {

    public Action OnAllDataLoaded;
    public ThumbnailsDataContainer thumbnailsDataContainer;

    private ThumbnailWebDownloadManager thumbnailWebDownloadManager;
    private ThumbnailsFilter thumbnailsFilter;
    private ThumbnailPriority thumbnailPriority;
    private int currentPriority;

    private int pageSize = 8;
    private int currentPage;

    private LoadingKey currentLoadingKey;

    public ThumbnailsData(ThumbnailPriority thumbnailPriority,
        ThumbnailWebDownloadManager thumbnailWebDownloadManager,
        ThumbnailsFilter thumbnailsFilter = null, int pageSize = 10) {
        this.thumbnailPriority = thumbnailPriority;
        this.thumbnailWebDownloadManager = thumbnailWebDownloadManager;
        this.thumbnailsFilter = thumbnailsFilter;
        this.pageSize = pageSize;

        thumbnailsDataContainer = new ThumbnailsDataContainer();

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

        if (currentPage >= 0)
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
        if (loadingKey != currentLoadingKey)
            return;

        currentPage = Mathf.CeilToInt((float)count / pageSize);
        Debug.Log("currentPage = " + currentPage + " count = " + count);

        GetThumbnailsOnCurrentPage();
    }

    private void ErrorPageCountCallBack(long code, string body, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;
        Debug.Log(code + " " + body);
    }

    #endregion

    #region get new Thumbnails

    private void GetThumbnailsOnCurrentPage() {
        ThumbnailWebDownloadManager.ThumbnailWebRequestStruct thumbnailWebRequestStruct =
            new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(thumbnailPriority.Stages[currentPriority], currentPage, pageSize, thumbnailsFilter);

        thumbnailWebDownloadManager.DownloadThumbnails(thumbnailWebRequestStruct, currentLoadingKey);
    }

    private void GetThumbnailsOnCurrentPageCallBack(StreamJsonData streamJsonData, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;

        thumbnailsDataContainer.AddListStreamJsonData(streamJsonData);
        currentPage--;
    }

    private void ErrorGetThumbnailsOnCurrentPageCallBack(long code, string body, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;
        Debug.Log(code + " " + body);
    }

    #endregion

    ~ThumbnailsData() {
        thumbnailWebDownloadManager.OnCountThumbnailsLoaded -= PageCountCallBack;
        thumbnailWebDownloadManager.OnErrorCountThumbnailsLoaded -= ErrorPageCountCallBack;

        thumbnailWebDownloadManager.OnStreamJsonDataLoaded -= GetThumbnailsOnCurrentPageCallBack;
        thumbnailWebDownloadManager.OnErrorStreamJsonDataLoaded -= ErrorGetThumbnailsOnCurrentPageCallBack;
    }
}
