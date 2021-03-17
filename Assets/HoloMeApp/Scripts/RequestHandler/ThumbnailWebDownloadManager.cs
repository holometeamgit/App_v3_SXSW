using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime;
using Beem.SSO;
using System.Threading.Tasks;

public class ThumbnailWebDownloadManager : MonoBehaviour {

    public struct ThumbnailWebRequestStruct {

        public ThumbnailsDataContainer.Priority Priority;
        public int PageNumber;
        public int MaxPageSize;
        public ThumbnailsFilter Filter;

        public ThumbnailWebRequestStruct(ThumbnailsDataContainer.Priority priority, int pageNumber, int maxPageSize, ThumbnailsFilter filter) {
            Priority = priority;
            PageNumber = pageNumber;
            MaxPageSize = maxPageSize;
            Filter = filter;
        }
    }

    public AccountManager accountManager;

    public Action<StreamJsonData, LoadingKey> OnStreamJsonDataLoaded;
    public Action<StreamJsonData> OnStreamByIdJsonDataLoaded;
    public Action<long, string, LoadingKey> OnErrorStreamJsonDataLoaded;

    public Action<int, LoadingKey> OnCountThumbnailsLoaded;
    public Action<long, string, LoadingKey> OnErrorCountThumbnailsLoaded;

    [SerializeField]
    WebRequestHandler webRequestHandler;

    [SerializeField]
    VideoUploader videoUploader;

    private string pageStreamParameter = "page";

    private string statusStreamParameter = "status";
    private const string IS_PIN = "is_pin";

    private string pageSize = "page_size";

    private const int DOWNLOAD_STREAM_DELAY_TIME = 1500;

    public void DownloadThumbnails(ThumbnailWebRequestStruct thumbnailWebRequestStruct, LoadingKey loadingKey) {
        webRequestHandler.GetRequest(GetRequestStreamURL(thumbnailWebRequestStruct),
        (code, body) => { DownloadThumbnailsCallBack(body, loadingKey); },
        (code, body) => { DownloadErrorThumbnailsCallBack(code, body, loadingKey); },
        accountManager.GetAccessToken().access);
    }

    public void GetCountThumbnails(ThumbnailWebRequestStruct thumbnailWebRequestStruct, LoadingKey loadingKey) {
        Debug.Log("GetCountThumbnails" + GetRequestStreamURL(thumbnailWebRequestStruct));
        webRequestHandler.GetRequest(GetRequestStreamURL(thumbnailWebRequestStruct),
        (code, body) => { GetCountThumbnailsCallBack(body, loadingKey); },
        (code, body) => { ErrorGetCountThumbnailsCallBack(code, body, loadingKey); },
        accountManager.GetAccessToken().access);
    }

    private void DownloadStreamByIdWithDelay(long id) { //TODO need wait server confirmation
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(DOWNLOAD_STREAM_DELAY_TIME).ContinueWith((_) => DownloadStreamById(id), taskScheduler);
    }

    private void DownloadStreamById(long id) {
        webRequestHandler.GetRequest(GetRequestStreamByIdURL(id),
            (code, body) => {
                HelperFunctions.DevLog("DownloadStreamById " + body);
                StreamJsonData streams = new StreamJsonData();

                StreamJsonData.Data streamJsonData = JsonUtility.FromJson<StreamJsonData.Data>(body);
                if (streamJsonData != null) {
                    streams.results.Add(streamJsonData);
                }


                OnStreamByIdJsonDataLoaded?.Invoke(streams);
            },
        (code, body) => { HelperFunctions.DevLog("Error DownloadStreamById " + id); },
        accountManager.GetAccessToken().access);
    }

    #region DownloadThumbnailsCallBack
    private void DownloadThumbnailsCallBack(string data, LoadingKey loadingKey) {
        StreamJsonData streamJsonData = GetStreamJsonData(data);
        if (streamJsonData == null)
            return;

        OnStreamJsonDataLoaded?.Invoke(streamJsonData, loadingKey);
    }

    private void DownloadErrorThumbnailsCallBack(long code, string body, LoadingKey loadingKey) {
        OnErrorStreamJsonDataLoaded?.Invoke(code, body, loadingKey);
    }
    #endregion

    #region GetCountThumbnailsCallBack
    private void GetCountThumbnailsCallBack(string data, LoadingKey loadingKey) {

        StreamJsonData streamJsonData = GetStreamJsonData(data);
        if (streamJsonData == null)
            return;

        OnCountThumbnailsLoaded?.Invoke(streamJsonData.count, loadingKey);
    }

    private void ErrorGetCountThumbnailsCallBack(long code, string body, LoadingKey loadingKey) {
        OnErrorCountThumbnailsLoaded.Invoke(code, body, loadingKey);
    }
    #endregion

    private StreamJsonData GetStreamJsonData(string data) {
        try {
            StreamJsonData streamJsonData = JsonUtility.FromJson<StreamJsonData>(data);
            return streamJsonData;
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
            return null;
        }
    }

    private string GetRequestStreamURL(ThumbnailWebRequestStruct thumbnailWebRequestStruct) {

        var builder = new UriBuilder(webRequestHandler.ServerURLMediaAPI + videoUploader.Stream);
        builder.Port = -1;
        var query = HttpUtility.ParseQueryString(builder.Query);

        //page number
        query[pageStreamParameter] = thumbnailWebRequestStruct.PageNumber.ToString();
        //page size
        query[pageSize] = thumbnailWebRequestStruct.MaxPageSize.ToString();
        //status
        if (thumbnailWebRequestStruct.Priority.Stage != StreamJsonData.Data.Stage.All)
            query[statusStreamParameter] = StreamJsonData.Data.GetStatusValue(thumbnailWebRequestStruct.Priority.Stage);

        query[IS_PIN] = thumbnailWebRequestStruct.Priority.IsPin.ToString();
        //user name
        if (thumbnailWebRequestStruct.Filter != null && !thumbnailWebRequestStruct.Filter.IsEmpty()) {
            foreach (var param in thumbnailWebRequestStruct.Filter.GetParameters())
                query[param.Key] = param.Value;
        }

        builder.Query = query.ToString();
        return builder.ToString();
    }

    private string GetRequestStreamByIdURL(long id) {
        return webRequestHandler.ServerURLMediaAPI + videoUploader.StreamById.Replace("{id}", id.ToString());
    }

    private void OnEnable() {
        CallBacks.onStreamPurchasedAndUpdateOnServer += DownloadStreamByIdWithDelay;
    }

    private void OnDisable() {
        CallBacks.onStreamPurchasedAndUpdateOnServer -= DownloadStreamByIdWithDelay;
    }
}
