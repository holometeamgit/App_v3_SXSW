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

    public Action<StreamJsonData, LoadingKey> OnStreamJsonDataLoaded;
    public Action<long, string, LoadingKey> OnErrorStreamJsonDataLoaded;

    public Action<StreamJsonData.Data> OnStreamByIdJsonDataLoaded;
    public Action<long> OnErrorStreamByIdJsonDataLoaded;

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
        webRequestHandler.Get(GetRequestStreamURL(thumbnailWebRequestStruct),
        (code, body) => { DownloadThumbnailsCallBack(body, loadingKey); },
        (code, body) => { DownloadErrorThumbnailsCallBack(code, body, loadingKey); },
        needHeaderAccessToken: true);
    }

    public void GetCountThumbnails(ThumbnailWebRequestStruct thumbnailWebRequestStruct, LoadingKey loadingKey) {
        webRequestHandler.Get(GetRequestStreamURL(thumbnailWebRequestStruct),
        (code, body) => { GetCountThumbnailsCallBack(body, loadingKey); },
        (code, body) => { ErrorGetCountThumbnailsCallBack(code, body, loadingKey); },
        needHeaderAccessToken: true);
    }

    private void Awake() {
        CallBacks.onDownloadStreamById += DownloadStreamById;
    }

    private void DownloadStreamById(long id) {
        webRequestHandler.Get(GetRequestStreamByIdURL(id),
            (code, body) => {
                HelperFunctions.DevLog("DownloadStreamById " + id + " " + body);

                StreamJsonData.Data streamJsonData = JsonUtility.FromJson<StreamJsonData.Data>(body);
                if (streamJsonData != null)
                    OnStreamByIdJsonDataLoaded?.Invoke(streamJsonData);
            },
        (code, body) => {
            HelperFunctions.DevLog("Error DownloadStreamById " + id);
            OnErrorStreamByIdJsonDataLoaded?.Invoke(id);
        },
        needHeaderAccessToken: true);
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
        CallBacks.onStreamPurchasedAndUpdateOnServer += DownloadStreamById;
    }

    private void OnDisable() {
        CallBacks.onStreamPurchasedAndUpdateOnServer -= DownloadStreamById;
    }

    private void OnDestroy() {
        CallBacks.onDownloadStreamById -= DownloadStreamById;
    }
}
