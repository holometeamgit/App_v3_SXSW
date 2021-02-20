//TODO will rewrite
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
    WebRequestHandler webRequestHandler;

    [SerializeField]
    AccountManager accountManager;

    [SerializeField]
    int maxPageSize = 6;
    int currentPageNumber;

    //timestamp of the request
    LoadingKey currentLoadingKey;

    private List<StreamJsonData.Data> streamJsonDataList;

    int countLoadedTextures;

    public UnityEvent OnDataFetched;

    public void FetchDataFromBeginning(string userName) {
        Debug.Log("FetchDataFromBeginning " + userName);
        currentPageNumber = 0;
        FetchNextData(userName);
    }

    public void FetchNextData(string userName) {
        ClearData();
        currentPageNumber++;
        FetchDataFromServer(userName);
    }

    private void FetchDataFromServer(string userName) {
        var thumbnailWebRequestStruct = new ThumbnailWebDownloadManager.ThumbnailWebRequestStruct(
            StreamJsonData.Data.Stage.All,
            currentPageNumber,
            maxPageSize,
            null
            );
        currentLoadingKey = new LoadingKey(this);
        Debug.Log("FetchDataFromServer");
        thumbnailWebDownloadManager.DownloadThumbnails(thumbnailWebRequestStruct, currentLoadingKey);
    }

    private void FetchStreamDataCallBack(List<StreamJsonData.Data> streamJsonData, StreamJsonData.Data.Stage stage, LoadingKey loadingKey) {
        Debug.Log("FetchStreamDataCallBack " + streamJsonData.Count + " " + stage);
        if (stage != StreamJsonData.Data.Stage.All)
            return;

        if (loadingKey != currentLoadingKey)
            return;

        streamJsonDataList.AddRange(streamJsonData);
        FetchTextureData(loadingKey);
    }

    private void FetchTextureData(LoadingKey loadingKey) {
        Debug.Log("FetchTextureData");

        if (loadingKey != currentLoadingKey)
            return;

        int waitingCount = streamJsonDataList.Count;

        foreach (var data in streamJsonDataList) {
            if (string.IsNullOrWhiteSpace(data.preview_s3_key) || string.IsNullOrWhiteSpace(data.preview_s3_url)) {
                waitingCount--;
                continue;
            }
            Debug.Log(data.preview_s3_url);
            webRequestHandler.GetTextureRequest(data.preview_s3_url,
                (code, body, texture) => TextureDataFetchedCallBack(waitingCount, loadingKey, data, texture),
                ((code, body) => Debug.Log(body)));
        }
        
    }

    private void TextureDataFetchedCallBack(int waitingCount, LoadingKey loadingKey, StreamJsonData.Data streamJsonData, Texture texture) {
        if (loadingKey != currentLoadingKey)
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
