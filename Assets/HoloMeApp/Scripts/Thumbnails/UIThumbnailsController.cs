using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIThumbnailsController : MonoBehaviour {
    public Action OnUpdated;
    public Action<StreamJsonData.Data> OnNeedPurchase;
    public Action<StreamJsonData.Data> OnPlay;

    [SerializeField] MediaFileDataHandler mediaFileDataHandler;
    [SerializeField] PnlViewingExperience pnlViewingExperience;
    [SerializeField] AgoraController agoraController;
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
        Debug.Log("RemoveUnnecessary");
        List<long> removingListID = new List<long>();

        foreach (var thumbnailElement in thumbnailElementsDictionary) {
            if (!dataList.Contains(thumbnailElement.Value.Data))
                removingListID.Add(thumbnailElement.Value.Data.id);
        }

        Debug.Log("RemoveUnnecessary count " + removingListID.Count);
        foreach (var id in removingListID) {
            thumbnailElementsDictionary.Remove(id);
        }

        for (int i = dataList.Count; i < btnThumbnailItems.Count; i++) {
            btnThumbnailItems[i].Deactivate();
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
            if (i <= 1) 
                continue;
            btnThumbnailItems[i].Deactivate();
        }

    }

    private void PrepareThumbnailElement() {
        Debug.Log("PrepareThumbnailElement " + dataList.Count);
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
            btnThumbnailItems[i].SetPressClickAction(ClickCallBack);
        }
        OnUpdated?.Invoke();
    }

    #endregion

    private void ClickCallBack(StreamJsonData.Data data) {
        Debug.Log(name + " ClickCallBack");
        if(!data.is_bought || data.is_bought && !data.IsStarted && data.HasProduct)
            OnNeedPurchase?.Invoke(data);
        Play(data);
    }

    private void Play(StreamJsonData.Data data) {
        if(data.is_bought && data.IsStarted) {
            PlayStream(data);
        } else if(data.HasTeaser) {
            PlayTeaser(data);
        }
    }

    private void PlayStream(StreamJsonData.Data data) {
        if(data.HasStreamUrl) {
            pnlViewingExperience.ActivateForPreRecorded(data.stream_s3_url, null);
            OnPlay?.Invoke(data);
        } else if(data.HasAgoraChannel) {
            agoraController.ChannelName = data.agora_channel;
            pnlStreamOverlay.OpenAsViewer();
            OnPlay?.Invoke(data);
        }
    }

    private void PlayTeaser(StreamJsonData.Data data) {
        pnlViewingExperience.ActivateForPreRecorded(data.teaser_s3_url, null);
        OnPlay?.Invoke(data);
    }
}