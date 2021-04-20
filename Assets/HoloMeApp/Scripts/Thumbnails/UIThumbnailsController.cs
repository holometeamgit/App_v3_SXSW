using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.Firebase.DynamicLink;

public class UIThumbnailsController : MonoBehaviour {
    public Action OnUpdated;
    public Action<string> OnPlayFromUser;

    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] PnlViewingExperience pnlViewingExperience;
    [SerializeField] PnlStreamOverlay pnlStreamOverlay;
    [SerializeField] GameObject btnThumbnailPrefab;
    [SerializeField] Transform content;
    [SerializeField] PurchaseManager purchaseManager;
    [SerializeField] PermissionController permissionController;

    Dictionary<long, ThumbnailElement> thumbnailElementsDictionary;

    Dictionary<long, UIThumbnail> btnThumbnailItemsDictionary;
    List<UIThumbnail> btnThumbnailItems;

    List<StreamJsonData.Data> dataList;

    StreamDataEqualityComparer streamDataEqualityComparer;

    //void OnEnable()
    //{
    //    pnlViewingExperience.ToggleARSessionObjects(false);
    //}

    public void Buy(StreamJsonData.Data data) {
        purchaseManager.SetPurchaseStreamData(data);
        purchaseManager.Purchase();
    }

    public void Play(StreamJsonData.Data data) {
        if (data.is_bought && data.IsStarted) {
            PlayStream(data);
        } else if (data.HasTeaser) {
            PlayTeaser(data);
        }
    }

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

    public void LockToPressElements() {
        for (int i = 0; i < btnThumbnailItems.Count; i++) {
            btnThumbnailItems[i].LockToPress(true);
        }
    }
    /// <summary>
    /// Play live stream from user 
    /// </summary>

    public void PlayLiveStream(string user, string agoraChannel, string streamID) { //TODO split it to ather class
        pnlStreamOverlay.OpenAsViewer(agoraChannel, streamID);
        OnPlayFromUser?.Invoke(user);
    }

    private void Awake() {

        thumbnailElementsDictionary = new Dictionary<long, ThumbnailElement>();
        btnThumbnailItemsDictionary = new Dictionary<long, UIThumbnail>();
        btnThumbnailItems = new List<UIThumbnail>();
        streamDataEqualityComparer = new StreamDataEqualityComparer();
    }

    #region Prepare thumbnails
    private void PrepareBtnThumbnails() {

        if(dataList.Count == 0) {
            HelperFunctions.DevLog("Deactivate all thumbnails count = " + btnThumbnailItems.Count);
            foreach(var btn in btnThumbnailItems) {
                btn.Deactivate();
            }
            return;
        }

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
            thumbnailElementsDictionary[thumbnailData.id] = new ThumbnailElement(thumbnailData, webRequestHandler);
        }
    }

    private void UpdateBtnData() {
        for (int i = 0; i < dataList.Count; i++) {
            btnThumbnailItemsDictionary[dataList[i].id] = btnThumbnailItems[i];
            btnThumbnailItems[i].AddData(thumbnailElementsDictionary[dataList[i].id]);
            btnThumbnailItems[i].SetPlayAction(Play);
            btnThumbnailItems[i].SetTeaserPlayAction(PlayTeaser);
            btnThumbnailItems[i].SetBuyAction(Buy);
            btnThumbnailItems[i].SetShareAction( (data) => {
                    //btnThumbnailItems[i]
                    StreamCallBacks.onGetStreamLink?.Invoke(data.id.ToString());
                    AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);
                });
            btnThumbnailItems[i].LockToPress(false);
        }
        OnUpdated?.Invoke();
    }

    #endregion

    private void PlayStream(StreamJsonData.Data data) {
        if (!permissionController.CheckCameraAccess())
            return;

        if (data.HasStreamUrl) {
            pnlViewingExperience.ActivateForPreRecorded(data.stream_s3_url, data, null, false);
            OnPlayFromUser?.Invoke(data.user);
        } else if (data.HasAgoraChannel) {
            if (data.agora_channel == "0" || string.IsNullOrWhiteSpace(data.agora_channel))
                return;
            PlayLiveStream(data.user, data.agora_channel, data.id.ToString());
        }
    }

    private void PlayTeaser(StreamJsonData.Data data) {
        if (!permissionController.CheckCameraAccess())
            return;

        pnlViewingExperience.ActivateForPreRecorded(data.teaser_s3_url, data, null, data.HasTeaser);
        OnPlayFromUser?.Invoke(data.user);
        purchaseManager.SetPurchaseStreamData(data);
    }
}
