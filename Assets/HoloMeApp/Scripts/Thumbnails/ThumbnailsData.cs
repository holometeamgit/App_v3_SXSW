using System;
using System.Collections.Generic;
using UnityEngine;

public class ThumbnailsData {

    

    private ThumbnailWebDownloadManager thumbnailWebDownloadManager;
    private ThumbnailsFilter thumbnailsFilter;
    private ThumbnailPriority thumbnailPriority;
    private int currentPriority;

    //    private int pagesCount;
    private int pageSize = 4;
    private int currentPage;

    private LoadingKey currentLoadingKey;

    public ThumbnailsData(ThumbnailPriority thumbnailPriority, ThumbnailWebDownloadManager thumbnailWebDownloadManager, ThumbnailsFilter thumbnailsFilter = null) {
        this.thumbnailPriority = thumbnailPriority;
        this.thumbnailWebDownloadManager = thumbnailWebDownloadManager;
        this.thumbnailsFilter = thumbnailsFilter;

        thumbnailWebDownloadManager.OnCountThumbnailsLoaded += PageCountCallBack;
        thumbnailWebDownloadManager.OnErrorCountThumbnailsLoaded += ErrorPageCountCallBack;

        thumbnailWebDownloadManager.OnStreamJsonDataLoaded += GetThumbnailsOnCurrentPageCallBack;
        thumbnailWebDownloadManager.OnErrorStreamJsonDataLoaded += ErrorGetThumbnailsOnCurrentPageCallBack;
    }

    public void RefreshData() {
        currentLoadingKey = new LoadingKey(this);
        currentPriority = 0;

        GetPageCount();
    }

    public void GetNextPage() {
        //проверить равна ли уже текущая страница нулю, если да,
        //то перейти к новому приоритету и попытаться запросить новое количество страниниц для текущего приортитета
        //если приоритеты закончились, то сообщить об окончании возможности грузить дальше
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
