using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;
using Beem.Permissions;

public class UIThumbnailsController : MonoBehaviour {
    public Action OnUpdated;
    public Action<string> OnPlayFromUser;

    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] PnlViewingExperience pnlViewingExperience;
    [SerializeField] PnlStreamOverlay pnlStreamOverlay;
    [SerializeField] PnlPrerecordedVideo pnlPrerecordedVideo;
    [SerializeField] GameObject btnThumbnailPrefab;
    [SerializeField] Transform content;
    [SerializeField] PurchaseManager purchaseManager;
    [SerializeField] int _startBtnCount = 20;

    private PermissionController _permissionController;
    private PermissionController permissionController {
        get {

            if (_permissionController == null) {
                _permissionController = FindObjectOfType<PermissionController>();
            }

            return _permissionController;
        }
    }

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

        HashSet<long> setId = new HashSet<long>();
        foreach(var data in dataList) {
            setId.Add(data.id);
        }

        foreach (var thumbnailElement in thumbnailElementsDictionary) {
            if (!setId.Contains(thumbnailElement.Value.Data.id))
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

    public void PlayLiveStream(string user, string agoraChannel, string streamID, bool isRoom) { //TODO split it to ather class
        if (isRoom) {
            if (!permissionController.CheckCameraMicAccess()) {
                return;
            }
        } else {
            if (!permissionController.CheckCameraAccess()) {
                return;
            }
        }
        pnlStreamOverlay.OpenAsViewer(agoraChannel, streamID, isRoom);
        OnPlayFromUser?.Invoke(user);
    }

    private void Awake() {

        thumbnailElementsDictionary = new Dictionary<long, ThumbnailElement>();
        btnThumbnailItemsDictionary = new Dictionary<long, UIThumbnail>();
        btnThumbnailItems = new List<UIThumbnail>();
        streamDataEqualityComparer = new StreamDataEqualityComparer();

        CallBacks.onClickLike += SetLike;
        CallBacks.onClickUnlike += SetUnlike;

        InstantiateBtns(_startBtnCount);
        CheckActiveBtns();
    }

    #region Prepare thumbnails
    private void InstantiateBtns(int count) {
        for (int i = 0; i < count; i++) {
            GameObject btnThumbnailItemsGO = Instantiate(btnThumbnailPrefab, content);
            UIThumbnail btnThumbnailItem = btnThumbnailItemsGO.GetComponent<UIThumbnail>();
            btnThumbnailItems.Add(btnThumbnailItem);
        }
    }

    private void CheckActiveBtns() {
        for (int i = 0; i < btnThumbnailItems.Count; i++) {
            btnThumbnailItems[i].Activate();
        }
        if (dataList.Count >= btnThumbnailItems.Count)
            return;

        for (int i = dataList.Count; i < btnThumbnailItems.Count; i++) {
            btnThumbnailItems[i].Deactivate();
        }
    }

    private void PrepareBtnThumbnails() {

        if (dataList.Count == 0) {
            foreach (var btn in btnThumbnailItems) {
                btn.Deactivate();
            }
            return;
        }

        int quantityDifference = btnThumbnailItems.Count - dataList.Count;
        InstantiateBtns(-quantityDifference);
        CheckActiveBtns();
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
            btnThumbnailItems[i].SetShareAction((data) => {
                //btnThumbnailItems[i]
                if (data.GetStage() == StreamJsonData.Data.Stage.Live) {
                    StreamCallBacks.onGetStreamLink?.Invoke(data.id.ToString(), data.user);
                } else {
                    StreamCallBacks.onGetPrerecordedLink.Invoke(data);
                }
                AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);
            });
            btnThumbnailItems[i].LockToPress(false);
        }
        OnUpdated?.Invoke();
    }

    #endregion

    private void PlayStream(StreamJsonData.Data data) {

        if (data.GetStage() == StreamJsonData.Data.Stage.Prerecorded) {
            if (!permissionController.CheckCameraAccess()) {
                return;
            }
        } else {
            if (!permissionController.CheckCameraMicAccess()) {
                return;
            }
        }

        if (data.HasStreamUrl) {
            pnlViewingExperience.ActivateForPreRecorded(data.stream_s3_url, data, null, false);
            pnlPrerecordedVideo.Init(data);
            OnPlayFromUser?.Invoke(data.user);
        } else if (data.HasAgoraChannel) {
            if (data.agora_channel == "0" || string.IsNullOrWhiteSpace(data.agora_channel))
                return;
            PlayLiveStream(data.user, data.agora_channel, data.id.ToString(), false);
        }
    }

    private void PlayTeaser(StreamJsonData.Data data) {
        if (!permissionController.CheckCameraAccess())
            return;

        pnlViewingExperience.ActivateForPreRecorded(data.teaser_s3_url, data, null, data.HasTeaser);
        pnlPrerecordedVideo.Init(data);
        OnPlayFromUser?.Invoke(data.user);
        purchaseManager.SetPurchaseStreamData(data);
    }

    private void SetLike(long streamId, bool isLike) {
        var stream = GetStreamElement(streamId);
        if (stream == null)
            return;

        stream.Data.is_liked = isLike;

        stream.Data.count_of_likes = isLike ? ++stream.Data.count_of_likes : Math.Max(--stream.Data.count_of_likes, 0);

        CallBacks.onGetLikeStateCallBack?.Invoke(streamId, stream.Data.is_liked, stream.Data.count_of_likes);
    }

    private void SetLike(long streamId) {
        SetLike(streamId, true);
    }

    private void SetUnlike(long streamId) {
        SetLike(streamId, false);
    }

    private ThumbnailElement GetStreamElement(long streamId) {
        if (!thumbnailElementsDictionary.ContainsKey(streamId))
            return null;

        return thumbnailElementsDictionary[streamId];
    }

    private void OnDestroy() {
        CallBacks.onClickLike -= SetLike;
        CallBacks.onClickUnlike -= SetUnlike;
    }
}
