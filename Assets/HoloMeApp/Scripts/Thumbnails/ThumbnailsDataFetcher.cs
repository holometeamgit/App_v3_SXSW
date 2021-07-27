using System;
using System.Collections.Generic;
using UnityEngine;

public class ThumbnailsDataFetcher {

    public Action OnAllDataLoaded; //all data from the server has been downloaded
    public Action OnErrorGetCountThumbnails;
    public Action OnErrorGetThumbnails;

    private ThumbnailWebDownloadManager thumbnailWebDownloadManager;
    private ThumbnailsFilter thumbnailsFilter; //parame
    private ThumbnailPriority thumbnailPriority;
    private ThumbnailsDataContainer thumbnailsDataContainer;
    private int currentPriority;

    private int pageSize = 8;
    private int currentPage = 1;

    private bool isBusy;

    private LoadingKey currentLoadingKey;

    public ThumbnailsDataFetcher(ThumbnailPriority thumbnailPriority,
        ThumbnailWebDownloadManager downloadManager,
        ThumbnailsFilter thumbnailsFilter = null, int pageSize = 10) {
        this.thumbnailPriority = thumbnailPriority;
        thumbnailWebDownloadManager = downloadManager;
        this.thumbnailsFilter = thumbnailsFilter;
        this.pageSize = pageSize;

        thumbnailsDataContainer = new ThumbnailsDataContainer(this.thumbnailPriority);

        thumbnailWebDownloadManager.OnCountThumbnailsLoaded += PageCountCallBack;
        thumbnailWebDownloadManager.OnErrorCountThumbnailsLoaded += ErrorPageCountCallBack;

        thumbnailWebDownloadManager.OnStreamJsonDataLoaded += GetThumbnailsOnCurrentPageCallBack;
        thumbnailWebDownloadManager.OnErrorStreamJsonDataLoaded += ErrorGetThumbnailsOnCurrentPageCallBack;

        thumbnailWebDownloadManager.OnStreamByIdJsonDataLoaded += UpdateThumbnail;
    }

    public List<StreamJsonData.Data> GetDataList() {
        return thumbnailsDataContainer.GetDataList();
    }

    public void RefreshData() {
        if (isBusy)
            return;
        isBusy = true;
        currentLoadingKey = new LoadingKey(this);
        ClearData();
        GetPageCount();
    }

    public void ClearData() {
        currentPriority = 0;
        thumbnailsDataContainer.Clear();
    }

    public void GetNextPage() {
        if (isBusy)
            return;

        isBusy = true;
        currentLoadingKey = new LoadingKey(this);

        if (currentPage >= 1)
            GetThumbnailsOnCurrentPage();
        else {
            currentPriority++;
            if (thumbnailPriority.Priorities.Count <= currentPriority) {
                isBusy = false;
                OnAllDataLoaded?.Invoke();
                return;
            }
            GetPageCount();
        }
    }

    #region page count and continue get thumbnails 

    private void GetPageCount() {
        if (thumbnailPriority.Priorities.Count <= currentPriority)
            return;

        ThumbnailWebDownloadManager.ThumbnailWebRequestStruct thumbnailWebRequestStruct =
            new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(thumbnailPriority.Priorities[currentPriority], 1, pageSize, thumbnailsFilter);

        thumbnailWebDownloadManager.GetCountThumbnails(thumbnailWebRequestStruct, currentLoadingKey);

    }

    private void PageCountCallBack(int count, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;
        isBusy = false;

        currentPage = Mathf.Max(Mathf.CeilToInt((float)count / pageSize), 1);

        GetThumbnailsOnCurrentPage();
    }

    private void ErrorPageCountCallBack(long code, string body, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;
        isBusy = false;
        OnErrorGetCountThumbnails?.Invoke();
    }

    #endregion

    #region get new Thumbnails

    private void GetThumbnailsOnCurrentPage() {

        ThumbnailWebDownloadManager.ThumbnailWebRequestStruct thumbnailWebRequestStruct =
            new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(thumbnailPriority.Priorities[currentPriority], currentPage, pageSize, thumbnailsFilter);

        thumbnailWebDownloadManager.DownloadThumbnails(thumbnailWebRequestStruct, currentLoadingKey);
    }

    private void GetThumbnailsOnCurrentPageCallBack(StreamJsonData streamJsonData, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;
        isBusy = false;
        currentPage--;
        if (streamJsonData == null || streamJsonData.results.Count == 0)
            GetNextPage();
        else
            AddThumbnails(streamJsonData);
    }

    private void UpdateThumbnail(StreamJsonData.Data data) {
        if (!thumbnailsDataContainer.ContainStream(data.id))
            return;

        StreamJsonData streamJsonData = new StreamJsonData();
        streamJsonData.results.Add(data);
        AddThumbnails(streamJsonData);
    }

    private void AddThumbnails(StreamJsonData streamJsonData) {
        thumbnailsDataContainer.AddListStreamJsonData(streamJsonData);
    }

    private void ErrorGetThumbnailsOnCurrentPageCallBack(long code, string body, LoadingKey loadingKey) {
        if (loadingKey != currentLoadingKey)
            return;
        isBusy = false;
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
