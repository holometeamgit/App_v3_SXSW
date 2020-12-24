using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIThumbnailsController : MonoBehaviour {
    public Action OnUpdated;
    public Action<StreamJsonData.Data> OnPlay;

    [SerializeField] MediaFileDataHandler mediaFileDataHandler;
    [SerializeField] PnlViewingExperience pnlViewingExperience;
    [SerializeField] AgoraController agoraController;
    [SerializeField] ShareManager shareManager;
    [SerializeField] PnlStreamOverlay pnlStreamOverlay;
    [SerializeField] GameObject btnThumbnailPrefab;
    [SerializeField] Transform content;
    [SerializeField] PurchaseManager purchaseManager;

    Dictionary<long, ThumbnailElement> thumbnailElementsDictionary;

    Dictionary<long, UIThumbnail> btnThumbnailItemsDictionary;
    List<UIThumbnail> btnThumbnailItems;

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
            if (!dataList.Contains(thumbnailElement.Value.Data))
                removingListID.Add(thumbnailElement.Value.Data.id);
        }
        foreach (var id in removingListID) {
            thumbnailElementsDictionary.Remove(id);
        }

        for (int i = dataList.Count; i < btnThumbnailItems.Count; i++) {
            btnThumbnailItems[i].Deactivate();
        }
    }

    private void Awake() {

        thumbnailElementsDictionary = new Dictionary<long, ThumbnailElement>();
        btnThumbnailItemsDictionary = new Dictionary<long, UIThumbnail>();
        btnThumbnailItems = new List<UIThumbnail>();
        streamDataEqualityComparer = new StreamDataEqualityComparer();
    }

    #region Prepare thumbnails
    private void PrepareBtnThumbnails() {
        int quantityDifference = btnThumbnailItems.Count - dataList.Count;

        for (int i = 0; i < -quantityDifference; i++) {
            GameObject btnThumbnailItemsGO = Instantiate(btnThumbnailPrefab, content);
            UIThumbnail btnThumbnailItem = btnThumbnailItemsGO.GetComponent<UIThumbnail>();
            btnThumbnailItems.Add(btnThumbnailItem);
        }
        for (int i = 0; i < btnThumbnailItems.Count; i++) {
            btnThumbnailItems[i].Activate();
        }
        if (dataList.Count == btnThumbnailItems.Count)
            return;
        for (int i = dataList.Count - 1; i < btnThumbnailItems.Count; i++) {
            if (i <= 0) 
                continue;
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
            thumbnailElementsDictionary[thumbnailData.id] = new ThumbnailElement(thumbnailData, mediaFileDataHandler);
        }
    }

    private void UpdateBtnData() {
        for (int i = 0; i < dataList.Count; i++) {
            btnThumbnailItemsDictionary[dataList[i].id] = btnThumbnailItems[i];
            btnThumbnailItems[i].AddData(thumbnailElementsDictionary[dataList[i].id]);
            btnThumbnailItems[i].SetPlayAction(Play);
            btnThumbnailItems[i].SetTeaserPlayAction(PlayTeaser);
            btnThumbnailItems[i].SetBuyAction(Buy);
            btnThumbnailItems[i].SetShareAction((_) => shareManager.ShareStream());
        }
        OnUpdated?.Invoke();
    }

    #endregion

    private void Buy(StreamJsonData.Data data) {
        purchaseManager.SetPurchaseStreamData(data);
        purchaseManager.Purchase();
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
            pnlViewingExperience.ActivateForPreRecorded(data.stream_s3_url, null,false);
            OnPlay?.Invoke(data);
        } else if(data.HasAgoraChannel) {
            agoraController.ChannelName = data.agora_channel;
            pnlStreamOverlay.OpenAsViewer();
            OnPlay?.Invoke(data);
        }
    }

    private void PlayTeaser(StreamJsonData.Data data) {
        pnlViewingExperience.ActivateForPreRecorded(data.teaser_s3_url, null, data.HasTeaser);
        OnPlay?.Invoke(data);
        purchaseManager.SetPurchaseStreamData(data);
    }
}