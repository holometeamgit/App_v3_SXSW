using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIThumbnailsController : MonoBehaviour {
    public Action OnUpdated;
    public Action OnPlay;

    [SerializeField] MediaFileDataHandler mediaFileDataHandler;
    [SerializeField] PnlViewingExperience pnlViewingExperience;
    [SerializeField] AgoraController agoraController;
    [SerializeField] ThumbnailsPurchaser thumbnailsPurchaser;
    [SerializeField] PnlStreamOverlay pnlStreamOverlay;
    [SerializeField] GameObject btnThumbnailPrefab;
    [SerializeField] Transform content;

    Dictionary<long, ThumbnailElement> thumbnailElementsDictionary;

    Dictionary<long, BtnThumbnailItemV2> btnThumbnailItemsDictionary;
    List<BtnThumbnailItemV2> btnThumbnailItems;

    List<StreamJsonData.Data> dataList;

    StreamDataEqualityComparer streamDataEqualityComparer;

    public void SetStreamJsonData(List<StreamJsonData.Data> data) {
        dataList = data;
    }

    public void UpdateData() {
        PrepareBtnThumbnails();
        PrepareThumbnailElement();
        UpdateBtnData();
    }

    public void RemoveUnnecessary() {
        List<long> removingListID = new List<long>();

        foreach (var thumbnailElement in thumbnailElementsDictionary) {
            if (dataList.Contains(thumbnailElement.Value.Data))
                removingListID.Add(thumbnailElement.Value.Data.id);
        }

        foreach (var id in removingListID) {
            thumbnailElementsDictionary.Remove(id);
        }
    }

    private void Awake() {
        thumbnailElementsDictionary = new Dictionary<long, ThumbnailElement>();
        btnThumbnailItemsDictionary = new Dictionary<long, BtnThumbnailItemV2>();
        btnThumbnailItems = new List<BtnThumbnailItemV2>();
        streamDataEqualityComparer = new StreamDataEqualityComparer();
    }

    #region Prepare thumbnails
    private void PrepareBtnThumbnails() {
        int quantityDifference = btnThumbnailItems.Count - dataList.Count;

        for (int i = 0; i < -quantityDifference; i++) {
            GameObject btnThumbnailItemsGO = Instantiate(btnThumbnailPrefab, content);
            BtnThumbnailItemV2 btnThumbnailItem = btnThumbnailItemsGO.GetComponent<BtnThumbnailItemV2>();
            btnThumbnailItems.Add(btnThumbnailItem);
        }
        for (int i = 0; i < btnThumbnailItems.Count; i++) {
            btnThumbnailItems[i].Activate();
        }
        for (int i = dataList.Count; i < btnThumbnailItems.Count; i++) {
            btnThumbnailItems[i].Deactivate();
        }

    }

    private void PrepareThumbnailElement() {
        foreach (var thumbnailData in dataList) {
            if (thumbnailElementsDictionary.ContainsKey(thumbnailData.id)) {
                ThumbnailElement thumbnailElement = thumbnailElementsDictionary[thumbnailData.id];
                if (thumbnailElement.Data == thumbnailData || streamDataEqualityComparer.Equals(thumbnailElement.Data, thumbnailData))
                    continue;
            }
            Debug.Log("thumbnailData " + thumbnailData.id + " " + thumbnailData.preview_s3_url);
            thumbnailElementsDictionary[thumbnailData.id] = new ThumbnailElement(thumbnailData, mediaFileDataHandler);
        }
    }

    private void UpdateBtnData() {
        for (int i = 0; i < dataList.Count; i++) {
            btnThumbnailItemsDictionary[dataList[i].id] = btnThumbnailItems[i];
            btnThumbnailItems[i].AddData(thumbnailElementsDictionary[dataList[i].id]);
            btnThumbnailItems[i].SetPressPlayAction(Play);
        }
        OnUpdated?.Invoke();
    }

    #endregion

    private void TryPlay() {
/*
        if (!thumbnailElement.Data.is_bought && thumbnailElement.Data.product_type != null &&
            !string.IsNullOrWhiteSpace(thumbnailElement.Data.product_type.product_id)) {
            thumbnailsPurchaser.Purchase(thumbnailElement.Data.product_type.product_id);
        } else {
            if (DateTime.Now < thumbnailElement.Data.StartDate)
                PlayTeaser();
            else
                Play();
        }*/
    }

    private void Play(StreamJsonData.Data data) {
/*        if (!string.IsNullOrWhiteSpace(thumbnailElement.Data.stream_s3_url))
            OnPlay?.Invoke(StreamJsonData.Data.Stage.Finished, thumbnailElement.Data.stream_s3_url);
        else if (!string.IsNullOrWhiteSpace(thumbnailElement.Data.agora_channel) && thumbnailElement.Data.GetStatus() == StreamJsonData.Data.Stage.Live)
            OnPlay?.Invoke(StreamJsonData.Data.Stage.Live, thumbnailElement.Data.agora_channel);*/
    }

    private void Play(StreamJsonData.Data.Stage stage, string url) {
        switch (stage) {
            case StreamJsonData.Data.Stage.Finished:
                pnlViewingExperience.ActivateForPreRecorded(url, null);
                OnPlay?.Invoke();
                break;
            case StreamJsonData.Data.Stage.Live:
                agoraController.ChannelName = url;
                pnlStreamOverlay.OpenAsViewer();
                OnPlay?.Invoke();
                break;
        }
    }

    private void PlayTeaser() {
/*        if (!string.IsNullOrWhiteSpace(thumbnailElement.Data.teaser))
            OnPlay?.Invoke(StreamJsonData.Data.Stage.Finished, thumbnailElement.Data.teaser);*/
    }
}