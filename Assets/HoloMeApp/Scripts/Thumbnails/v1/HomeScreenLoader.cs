/* needed to download data for the home page
 * 
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class HomeScreenLoader : MonoBehaviour {

    public class DataElement {
        public StreamJsonData.Data streamJsonData;
        public Texture texture;
    }

    public List<DataElement> eventHomeScreenDataElement;
    public List<DataElement> liveHomeScreenDataElement;
    public List<DataElement> streamHomeScreenDataElement;

    [SerializeField]
    ThumbnailWebDownloadManager thumbnailWebDownloadManager;

    [SerializeField] ThumbnailPriorityScriptableObject thumbnailPriority;

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
    int waitingCount;

    public UnityEvent OnDataFetched;

    public void FetchData() {
        ThumbnailsData thumbnailsData = new ThumbnailsData(thumbnailPriority.ThumbnailPriority, thumbnailWebDownloadManager);
        thumbnailsData.RefreshData();
        //ClearData();
        //FetchEventStreamData();
    }

    void Start() {
        
    }

    private void FetchEventStreamData() {

        fetchStartDateTime = DateTime.Now;

/*        var thumbnailEventDataRequest = new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(
            StreamJsonData.Data.Stage.Announced,
            accountManager.GetAccessToken().access,
            currentEventPageNumber,
            maxEventPageSize,
            "");

        var thumbnailLiveDataRequest = new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(
            StreamJsonData.Data.Stage.Live,
            accountManager.GetAccessToken().access,
            currentLivePageNumber,
            maxLivePageSize,
            "");

        var thumbnailFinishedDataRequest = new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(
            StreamJsonData.Data.Stage.Finished,
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
*/
    }

    private void FetchStreamDataCallBack(List<StreamJsonData.Data> streamJsonData, StreamJsonData.Data.Stage stage, DateTime fetchStart) {
        if (stage == StreamJsonData.Data.Stage.All)
            return;

        if (fetchStart != fetchStartDateTime)
            return;

        countLoadedStreamData++;

        if (streamJsonData.Count != 0)
            switch (stage) {
                case StreamJsonData.Data.Stage.Announced:
                    eventStreamJsonData.AddRange(streamJsonData);
                    break;
                case StreamJsonData.Data.Stage.Live:
                    liveStreamJsonData.AddRange(streamJsonData);
                    break;
                case StreamJsonData.Data.Stage.Finished:
                    finishedStreamJsonData.AddRange(streamJsonData);
                    break;
            }

        if (countLoadedStreamData == compliteCountLoadedStreamData)
            FetchTextureData(fetchStart);
    }

    private void FetchTextureData(DateTime fetchStart) {

        if (fetchStart != fetchStartDateTime)
            return;

        waitingCount = eventStreamJsonData.Count + liveStreamJsonData.Count + finishedStreamJsonData.Count;

        //Debug.Log("event");
        foreach (var data in eventStreamJsonData) {
            if (string.IsNullOrWhiteSpace(data.preview_s3_key) || string.IsNullOrWhiteSpace(data.preview_s3_url)) {
                waitingCount--;
                continue;
            }
            Debug.Log(data.preview_s3_url);
            mediaFileDataHandler.LoadImg(data.preview_s3_url,
                (code, body, texture) => TextureDataFetchedCallBack(fetchStart, StreamJsonData.Data.Stage.Announced, data, texture),
                (code, body) => ErrorTextureDataFetchedCallBack(fetchStart, code, body));
        }
        //Debug.Log("live");
        foreach (var data in liveStreamJsonData) {
            if (string.IsNullOrWhiteSpace(data.preview_s3_key) || string.IsNullOrWhiteSpace(data.preview_s3_url)) {
                waitingCount--;
                continue;
            }
            Debug.Log(data.preview_s3_url);
            mediaFileDataHandler.LoadImg(data.preview_s3_url,
                (code, body, texture) => TextureDataFetchedCallBack(fetchStart, StreamJsonData.Data.Stage.Live, data, texture),
                (code, body) => ErrorTextureDataFetchedCallBack(fetchStart, code, body));
        }
        //Debug.Log("stream");
        foreach (var data in finishedStreamJsonData) {
            if (string.IsNullOrWhiteSpace(data.preview_s3_key) || string.IsNullOrWhiteSpace(data.preview_s3_url)) {
                waitingCount--;
                continue;
            }
            Debug.Log(data.preview_s3_url);
            mediaFileDataHandler.LoadImg(data.preview_s3_url,
                (code, body, texture) => TextureDataFetchedCallBack(fetchStart, StreamJsonData.Data.Stage.Finished, data, texture),
                (code, body) => ErrorTextureDataFetchedCallBack(fetchStart, code, body));
        }

    }

    private void TextureDataFetchedCallBack(DateTime fetchStart, StreamJsonData.Data.Stage stage, StreamJsonData.Data streamJsonData, Texture texture) {
        if (fetchStart != fetchStartDateTime)
            return;

        Debug.Log("TextureDataFetchedCallBack " + countLoadedTextures);

        countLoadedTextures++;

        DataElement homeScreenDataElement = new DataElement();
        homeScreenDataElement.streamJsonData = streamJsonData;
        homeScreenDataElement.texture = texture;

        switch (stage) {
            case StreamJsonData.Data.Stage.Announced:
                eventHomeScreenDataElement.Add(homeScreenDataElement);
                break;
            case StreamJsonData.Data.Stage.Finished:
                streamHomeScreenDataElement.Add(homeScreenDataElement);
                break;
            case StreamJsonData.Data.Stage.Live:
                liveHomeScreenDataElement.Add(homeScreenDataElement);
                break;
        }

        if (countLoadedTextures == waitingCount)
            OnDataFetched.Invoke();
    }

    private void ErrorTextureDataFetchedCallBack(DateTime fetchStart, long code, string body) {
        if (fetchStart != fetchStartDateTime)
            return;
        waitingCount--;

        Debug.Log(body + " " + countLoadedTextures + " " + waitingCount);

        if (countLoadedTextures == waitingCount)
            OnDataFetched.Invoke();
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

        eventHomeScreenDataElement = new List<DataElement>();
        liveHomeScreenDataElement = new List<DataElement>();
        streamHomeScreenDataElement = new List<DataElement>();
    }
}
