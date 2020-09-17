using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class BroadcasterScreenLoader : MonoBehaviour
{
    public class DataElement {
        public StreamJsonData.Data streamJsonData;
        public Texture texture;
    }

    public List<DataElement> dataElements;

    [SerializeField]
    ThumbnailWebDownloadManager thumbnailWebDownloadManager;

    [SerializeField]
    MediaFileDataHandler mediaFileDataHandler;

    [SerializeField]
    AccountManager accountManager;

    [SerializeField]
    int maxPageSize = 6;
    int currentPageNumber;

    //timestamp of the request
    DateTime fetchStartDateTime;

    private List<StreamJsonData.Data> streamJsonDataList;

    int countLoadedTextures;

    public UnityEvent OnDataFetched;

    public void FetchDataFromBeginning(string userName) {
        Debug.Log("FetchDataFromBeginning " + userName);
        currentPageNumber = 0;
        FetchNextData(userName);
    }

    public void FetchNextData(string userName) {
        fetchStartDateTime = DateTime.Now;
        ClearData();
        currentPageNumber++;
        FetchDataFromServer(userName);
    }

    private void FetchDataFromServer(string userName) {
        var thumbnailWebRequestStruct = new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(
            StreamJsonData.Data.Stage.All,
            accountManager.GetAccessToken().access,
            currentPageNumber,
            maxPageSize,
            userName
            );

        Debug.Log("FetchDataFromServer");
        thumbnailWebDownloadManager.LoadThubnails(thumbnailWebRequestStruct, fetchStartDateTime,
            (streamJsonData, stage) => FetchStreamDataCallBack(streamJsonData, stage, fetchStartDateTime));
    }

    private void FetchStreamDataCallBack(List<StreamJsonData.Data> streamJsonData, StreamJsonData.Data.Stage stage, DateTime fetchStart) {
        Debug.Log("FetchStreamDataCallBack " + streamJsonData.Count + " " + stage);
        if (stage != StreamJsonData.Data.Stage.All)
            return;

        Debug.Log(fetchStart);
        Debug.Log(fetchStartDateTime);

        if (fetchStart != fetchStartDateTime)
            return;

        streamJsonDataList.AddRange(streamJsonData);
        FetchTextureData(fetchStart);
    }

    private void FetchTextureData(DateTime fetchStart) {
        Debug.Log("FetchTextureData");

        if (fetchStart != fetchStartDateTime)
            return;

        int waitingCount = streamJsonDataList.Count;

        foreach (var data in streamJsonDataList) {
            if (string.IsNullOrWhiteSpace(data.preview_s3_key) || string.IsNullOrWhiteSpace(data.preview_s3_url)) {
                waitingCount--;
                continue;
            }
            Debug.Log(data.preview_s3_url);
            mediaFileDataHandler.LoadImg(data.preview_s3_url,
                (code, body, texture) => TextureDataFetchedCallBack(waitingCount, fetchStart, data, texture),
                ((code, body) => Debug.Log(body)));
        }
        
    }

    private void TextureDataFetchedCallBack(int waitingCount, DateTime fetchStart, StreamJsonData.Data streamJsonData, Texture texture) {
        if (fetchStart != fetchStartDateTime)
            return;

        countLoadedTextures++;

        DataElement dataElement = new DataElement();
        dataElement.streamJsonData = streamJsonData;
        dataElement.texture = texture;

        dataElements.Add(dataElement);

        if (countLoadedTextures == waitingCount)
            OnDataFetched.Invoke();
    }

    private void ClearData() {
        dataElements = new List<DataElement>();
        streamJsonDataList = new List<StreamJsonData.Data>();
        countLoadedTextures = 0;
    }
}
