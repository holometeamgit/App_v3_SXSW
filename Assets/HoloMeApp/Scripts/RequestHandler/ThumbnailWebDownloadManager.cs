using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime;

public class ThumbnailWebDownloadManager : MonoBehaviour {

    public struct ThumbnailWebRequestStruct {

        public StreamJsonData.Data.Stage Stage;
        public int PageNumber;
        public int MaxPageSize;
        public ThumbnailsFilter Filter;

        public ThumbnailWebRequestStruct(StreamJsonData.Data.Stage stage, int pageNumber, int maxPageSize, ThumbnailsFilter filter) {
            Stage = stage;
            PageNumber = pageNumber;
            MaxPageSize = maxPageSize;
            Filter = filter;
        }
    }

    public AccountManager accountManager;

    public Action<StreamJsonData, LoadingKey> OnStreamJsonDataLoaded;
    public Action<long, string, LoadingKey> OnErrorStreamJsonDataLoaded;

    public Action<int, LoadingKey> OnCountThumbnailsLoaded;
    public Action<long, string, LoadingKey> OnErrorCountThumbnailsLoaded;

    [SerializeField]
    WebRequestHandler webRequestHandler;

    [SerializeField]
    VideoUploader videoUploader;

    private string pageStreamParameter = "page";

    private string statusStreamParameter = "status";

    private string pageSize = "page_size";

    public void DownloadThumbnails(ThumbnailWebRequestStruct thumbnailWebRequestStruct, LoadingKey loadingKey) {
        webRequestHandler.GetRequest(GetRequestRefreshTokenURL(thumbnailWebRequestStruct),
        (code, body) => { DownloadThumbnailsCallBack(body, loadingKey); },
        (code, body) => { DownloadErrorThumbnailsCallBack(code, body, loadingKey); },
        accountManager.GetAccessToken().access);
    }

    public void GetCountThumbnails(ThumbnailWebRequestStruct thumbnailWebRequestStruct, LoadingKey loadingKey) {
        webRequestHandler.GetRequest(GetRequestRefreshTokenURL(thumbnailWebRequestStruct),
        (code, body) => { GetCountThumbnailsCallBack(body, loadingKey); },
        (code, body) => { ErrorGetCountThumbnailsCallBack(code, body, loadingKey); },
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
        } catch (Exception e) { return null; }
    }

    private string GetRequestRefreshTokenURL(ThumbnailWebRequestStruct thumbnailWebRequestStruct) {
        /*     string result = webRequestHandler.ServerURLMediaAPI + videoUploader.Stream + "?"
                 + pageStreamParameter + "=" + thumbnailWebRequestStruct.PageNumber +
                 "&" + pageSize + "=" + thumbnailWebRequestStruct.MaxPageSize +
             (thumbnailWebRequestStruct.Stage != StreamJsonData.Data.Stage.All ? ("&" + statusStreamParameter + "=" + StreamJsonData.Data.GetStatusValue(thumbnailWebRequestStruct.Stage)) : "") +
             ((thumbnailWebRequestStruct.Filter == null || thumbnailWebRequestStruct.Filter.IsEmpty()) ? "" : ("&" + thumbnailWebRequestStruct.Filter.GetParametersString()));
             Debug.Log(result);*/

        var builder = new UriBuilder(webRequestHandler.ServerURLMediaAPI + videoUploader.Stream);
        builder.Port = -1;
        var query = HttpUtility.ParseQueryString(builder.Query);

        //page number
        query[pageStreamParameter] = thumbnailWebRequestStruct.PageNumber.ToString();
        //page size
        query[pageSize] = thumbnailWebRequestStruct.MaxPageSize.ToString();
        //status
        if (thumbnailWebRequestStruct.Stage != StreamJsonData.Data.Stage.All)
            query[statusStreamParameter] = StreamJsonData.Data.GetStatusValue(thumbnailWebRequestStruct.Stage);
        //user name
        if (thumbnailWebRequestStruct.Filter != null && !thumbnailWebRequestStruct.Filter.IsEmpty()) {
            foreach (var param in thumbnailWebRequestStruct.Filter.GetParameters())
                query[param.Key] = param.Value;
        }

        builder.Query = query.ToString();
        Debug.Log(builder.ToString());
        return builder.ToString();
    }


}
