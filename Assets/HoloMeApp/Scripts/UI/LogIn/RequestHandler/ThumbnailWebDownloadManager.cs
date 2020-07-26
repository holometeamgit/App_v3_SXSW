using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ThumbnailWebDownloadManager : MonoBehaviour
{
    public enum Stage {
        All,
        Live,
        Announced,
        Finished
    }

    public struct ThumbnailWebRequestStruct {

        public Stage Stage;
        public string AccessToken;
        public int PageNumber;
        public int MaxPageSize;
        public string UserName;

        public ThumbnailWebRequestStruct(Stage Stage, string AccessToken, int PageNumber, int MaxPageSize, string UserName) {
            this.Stage = Stage;
            this.AccessToken = AccessToken;
            this.PageNumber = PageNumber;
            this.MaxPageSize = MaxPageSize;
            this.UserName = UserName;
        }
    }

    public delegate void StreamJsonDataDelegate(List<StreamJsonData.Data> streamJsonData, Stage stage);

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

    private void LoadThumbnailsCallBack(string data, DateTime startLoadingDate, StreamJsonDataDelegate streamJsonDataDelegate, Stage stage) {
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
        (thumbnailWebRequestStruct.Stage != Stage.All ? ("&" + statusStreamParameter + GetStatusValue(thumbnailWebRequestStruct.Stage)) : "") +
        (string.IsNullOrEmpty(thumbnailWebRequestStruct.UserName) ? "" : ("&" + this.userName + thumbnailWebRequestStruct.UserName));
    }

    private string GetStatusValue(Stage stage) {
        switch(stage) {
        case Stage.Announced:
            return "announced";
        case Stage.Finished:
            return "finished";
        case Stage.Live:
            return "live";
        case Stage.All:
        default:
            return "";
        }
    }
}
