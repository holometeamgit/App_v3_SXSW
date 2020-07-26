/* needed to download data for the home page
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class HomeScreenLoader : MonoBehaviour
{

    public class HomeScreenDataElement {
        public StreamJsonData.Data streamJsonData;
        public Texture texture;
    }

    public List<HomeScreenDataElement> eventHomeScreenDataElement;
    public List<HomeScreenDataElement> liveHomeScreenDataElement;
    public List<HomeScreenDataElement> streamHomeScreenDataElement;

    [SerializeField]
    ThumbnailWebDownloadManager thumbnailWebDownloadManager;

    [SerializeField]
    MediaFileDataHandler mediaFileDataHandler;

    [SerializeField]
    AccountManager accountManager;

    [SerializeField]
    int maxEventPageSize = 1;
    [SerializeField]
    int maxLivePageSize = 2;
    [SerializeField]
    int maxFinishedPageSize = 6;

    //timestamp of the request
    DateTime fetchStartDateTime;

    private List<StreamJsonData.Data> eventStreamJsonData;
    private List<StreamJsonData.Data> liveStreamJsonData;
    private List<StreamJsonData.Data> finishedStreamJsonData;
    int currentEventPageNumber;
    int currentFinishedPageNumber;
    int currentLivePageNumber;

    int countLoadedStreamData;
    int compliteCountLoadedStreamData = 3;
    int countLoadedTextures;

    public UnityEvent OnDataFetched;

    public void FetchData() {
        ClearData();
        FetchEventStreamData();
    }

    private void ClearData() {
        eventStreamJsonData = new List<StreamJsonData.Data>();
        liveStreamJsonData = new List<StreamJsonData.Data>();
        finishedStreamJsonData = new List<StreamJsonData.Data>();

        currentEventPageNumber = 1;
        currentLivePageNumber = 1;
        currentFinishedPageNumber = 1;

        countLoadedStreamData = 0;
        countLoadedTextures = 0;

        eventHomeScreenDataElement = new List<HomeScreenDataElement>();
        liveHomeScreenDataElement = new List<HomeScreenDataElement>();
        streamHomeScreenDataElement = new List<HomeScreenDataElement>();
    }

    private void FetchEventStreamData() {

        fetchStartDateTime = DateTime.Now;

        var thumbnailEventDataRequest = new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(
            ThumbnailWebDownloadManager.Stage.Announced,
            accountManager.GetAccessToken().access,
            currentEventPageNumber,
            maxEventPageSize,
            "");

        var thumbnailLiveDataRequest = new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(
            ThumbnailWebDownloadManager.Stage.Live,
            accountManager.GetAccessToken().access,
            currentLivePageNumber,
            maxLivePageSize,
            "");

        var thumbnailFinishedDataRequest = new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(
            ThumbnailWebDownloadManager.Stage.Finished,
            accountManager.GetAccessToken().access,
            currentFinishedPageNumber,
            maxFinishedPageSize,
            "");

        //request Event thumbnails
        thumbnailWebDownloadManager.LoadThubnails(thumbnailEventDataRequest, fetchStartDateTime, (streamJsonData, stage) => FetchStreamDataCallBack(streamJsonData, stage, fetchStartDateTime));
        //request Live thumbnails
        thumbnailWebDownloadManager.LoadThubnails(thumbnailLiveDataRequest, fetchStartDateTime, (streamJsonData, stage) => FetchStreamDataCallBack(streamJsonData, stage, fetchStartDateTime));
        //request Finished thumbnails
        thumbnailWebDownloadManager.LoadThubnails(thumbnailFinishedDataRequest, fetchStartDateTime, (streamJsonData, stage) => FetchStreamDataCallBack(streamJsonData, stage, fetchStartDateTime));

    }

    private void FetchStreamDataCallBack(List<StreamJsonData.Data> streamJsonData, ThumbnailWebDownloadManager.Stage stage, DateTime fetchStart) {
        if (stage == ThumbnailWebDownloadManager.Stage.All)
            return;

        if (fetchStart != fetchStartDateTime)
            return;

        countLoadedStreamData++;

        if (streamJsonData.Count != 0)
            switch(stage) {
            case ThumbnailWebDownloadManager.Stage.Announced:
                eventStreamJsonData.AddRange(streamJsonData);
                break;
            case ThumbnailWebDownloadManager.Stage.Live:
                liveStreamJsonData.AddRange(streamJsonData);
                break;
            case ThumbnailWebDownloadManager.Stage.Finished:
                finishedStreamJsonData.AddRange(streamJsonData);
                break;
            }

        if (countLoadedStreamData == compliteCountLoadedStreamData)
            FetchTextureData(fetchStart);

    }

    private void FetchTextureData(DateTime fetchStart) {

        if (fetchStart != fetchStartDateTime)
            return;

        int waitingCount = eventStreamJsonData.Count + liveStreamJsonData.Count + finishedStreamJsonData.Count;

        Debug.Log("event");
        foreach (var data in eventStreamJsonData) {
            Debug.Log(data.preview_s3_url);
            mediaFileDataHandler.LoadImg(data.preview_s3_url,
                (code, body, texture) => TextureDataFetchedCallBack(waitingCount, fetchStart, ThumbnailWebDownloadManager.Stage.Announced, data, texture),
                ((code, body) => Debug.Log(body)));
        }
        Debug.Log("live");
        foreach (var data in liveStreamJsonData) {
            Debug.Log(data.preview_s3_url);
            mediaFileDataHandler.LoadImg(data.preview_s3_url,
                (code, body, texture) => TextureDataFetchedCallBack(waitingCount, fetchStart, ThumbnailWebDownloadManager.Stage.Live, data, texture),
                ((code, body) => Debug.Log(body)));
        }
        Debug.Log("stream");
        foreach (var data in finishedStreamJsonData) {
            Debug.Log(data.preview_s3_url);
            mediaFileDataHandler.LoadImg(data.preview_s3_url,
                (code, body, texture) => TextureDataFetchedCallBack(waitingCount, fetchStart, ThumbnailWebDownloadManager.Stage.Finished, data, texture),
                ((code, body) => Debug.Log(body)));
        }

    }

    private void TextureDataFetchedCallBack(int waitingCount, DateTime fetchStart, ThumbnailWebDownloadManager.Stage stage, StreamJsonData.Data streamJsonData, Texture texture) {
        if (fetchStart != fetchStartDateTime)
            return;

        countLoadedTextures++;

        HomeScreenDataElement homeScreenDataElement = new HomeScreenDataElement();
        homeScreenDataElement.streamJsonData = streamJsonData;
        homeScreenDataElement.texture = texture;

        switch(stage) {
        case ThumbnailWebDownloadManager.Stage.Announced:
            eventHomeScreenDataElement.Add(homeScreenDataElement);
            break;
        case ThumbnailWebDownloadManager.Stage.Finished:
            streamHomeScreenDataElement.Add(homeScreenDataElement);
            break;
        case ThumbnailWebDownloadManager.Stage.Live:
            liveHomeScreenDataElement.Add(homeScreenDataElement);
            break;
        }

        if (countLoadedTextures == waitingCount)
            OnDataFetched.Invoke();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
