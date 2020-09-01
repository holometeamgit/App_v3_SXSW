using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ThumbnailWebDownloadManager : MonoBehaviour
{
    public struct ThumbnailWebRequestStruct {

        public StreamJsonData.Data.Stage Stage;
        public string AccessToken;
        public int PageNumber;
        public int MaxPageSize;
        public string UserName;

        public ThumbnailWebRequestStruct(StreamJsonData.Data.Stage Stage, string AccessToken, int PageNumber, int MaxPageSize, string UserName) {
            this.Stage = Stage;
            this.AccessToken = AccessToken;
            this.PageNumber = PageNumber;
            this.MaxPageSize = MaxPageSize;
            this.UserName = UserName;
        }
    }

    public delegate void StreamJsonDataDelegate(List<StreamJsonData.Data> streamJsonData, StreamJsonData.Data.Stage stage);

    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField] string getStreamAccessTokenAPI;

    private string pageStreamParameter = "page=";

    private string statusStreamParameter = "status=";

    private string pageSize = "page_size=";

    private string userName = "username=";
    //private int

    //the key to load data from one request time
    private DateTime startLoadingThubnailsDateTime;

    public void LoadThubnails(ThumbnailWebRequestStruct thumbnailWebRequestStruct, DateTime startLoadingDateTime, StreamJsonDataDelegate streamJsonData) {

        startLoadingThubnailsDateTime = startLoadingDateTime;

        webRequestHandler.GetRequest(GetRequestRefreshTokenURL(thumbnailWebRequestStruct),
        (code, body) => { LoadThumbnailsCallBack(body, startLoadingDateTime, streamJsonData, thumbnailWebRequestStruct.Stage); },
        (code, body) => { Debug.Log(code + " " + body); /*TODO show warning somewhere */},
    thumbnailWebRequestStruct.AccessToken);
    }

    private void LoadThumbnailsCallBack(string data, DateTime startLoadingDate, StreamJsonDataDelegate streamJsonDataDelegate, StreamJsonData.Data.Stage stage) {
        try {
            if (startLoadingDate != startLoadingThubnailsDateTime)
                return;
            StreamJsonData streamJsonData = JsonUtility.FromJson<StreamJsonData>(data);

            if (streamJsonData == null)
                return;

            streamJsonDataDelegate.Invoke(streamJsonData.results, stage);
        } catch (Exception e) { }
    }

    void Start() {}

    #region Data loading
    private void LoadData(StreamJsonData streamJsonData) {
        foreach(var data in streamJsonData.results) {

        }
    }

    #endregion

    private string GetRequestRefreshTokenURL(ThumbnailWebRequestStruct thumbnailWebRequestStruct) {

        //Debug.Log("GetRequestRefreshTokenURL " + thumbnailWebRequestStruct.UserName);
        return webRequestHandler.serverURLMediaAPI + getStreamAccessTokenAPI + "?"
            + pageStreamParameter + thumbnailWebRequestStruct.PageNumber +
            "&" + pageSize + thumbnailWebRequestStruct.MaxPageSize +
        (thumbnailWebRequestStruct.Stage != StreamJsonData.Data.Stage.All ? ("&" + statusStreamParameter + StreamJsonData.Data.GetStatusValue(thumbnailWebRequestStruct.Stage)) : "") +
        (string.IsNullOrEmpty(thumbnailWebRequestStruct.UserName) ? "" : ("&" + this.userName + thumbnailWebRequestStruct.UserName));
    }


}
