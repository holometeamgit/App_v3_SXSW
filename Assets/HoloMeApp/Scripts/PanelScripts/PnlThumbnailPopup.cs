using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Beem.SSO;
using System.Threading.Tasks;

public class PnlThumbnailPopup : UIThumbnail {

    [SerializeField]
    RawImage rawImage;
    [SerializeField]
    TMP_Text txtTitle;
    [SerializeField]
    TMP_Text txtDescription;
    [SerializeField]
    GameObject btnWatchNow;
    [SerializeField]
    GameObject btnPlayTeaser;
    [SerializeField]
    GameObject btnBuyTicket;
    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField]
    UIThumbnailsController uiThumbnailsController;
    [SerializeField]
    ThumbnailWebDownloadManager thumbnailWebDownloadManager;

    [Space]
    [SerializeField]
    AspectRatioFitterByMinSide aspectRatioFitter;
    [SerializeField]
    Texture defaultTexture;
    [SerializeField]
    UIAnimator auAnimator;

    [SerializeField]
    private VerticalLayoutGroup layoutGroup;

    ThumbnailElement thumbnailElement;
    long currentId = 0;

    const long DEFAUL_STREAM_DATA_ID = 0;
    private const int REFRESH_LAYOUT_TIME = 1000;
    bool isSubscribed;

    public override void Play() {
        uiThumbnailsController.Play(thumbnailElement.Data);
    }

    public override void PlayTeaser() {
        uiThumbnailsController.Play(thumbnailElement.Data);
    }

    public override void Buy() {
        uiThumbnailsController.Buy(thumbnailElement.Data);
    }

    public void OpenStream(long id) {
        if (!isSubscribed) {
            thumbnailWebDownloadManager.OnStreamByIdJsonDataLoaded += ShowStreamStream;
            isSubscribed = true;
        }
        currentId = id;
        CallBacks.onDownloadStreamById?.Invoke(id);
    }

    public override void AddData(ThumbnailElement element) {
        if (thumbnailElement != null) {
            thumbnailElement.OnTextureLoaded -= UpdateTexture;
            thumbnailElement.OnErrorTextureLoaded -= UpdateTexture;
            thumbnailElement.Data.OnDataUpdated -= UpdateData;
        }

        thumbnailElement = element;

        thumbnailElement.OnTextureLoaded += UpdateTexture;
        thumbnailElement.OnErrorTextureLoaded += UpdateTexture;
        thumbnailElement.Data.OnDataUpdated += UpdateData;

        UpdateData();
    }

    public void ClosePnl() {
        gameObject.SetActive(false);
        currentId = DEFAUL_STREAM_DATA_ID;
    }

    private void ShowPnl() {
        gameObject.SetActive(true);
    }

    private void ShowStreamStream(StreamJsonData.Data streamData) {
        if (streamData.id != currentId || (streamData.IsPublicLiveOrPrerecorded() || streamData.HasStreamUrl))
            return;
        HelperFunctions.DevLog("Thumbnail popup open stream id " + streamData.id);
        /* autoplay
         * if (streamData.is_bought) {
            uiThumbnailsController.Play(streamData);
        }*/

        ThumbnailElement element = new ThumbnailElement(streamData, webRequestHandler);
        AddData(element);

        ShowPnl();
    }

    private void UpdateTexture() {

        if (!thumbnailElement.Data.is_bought) {
            rawImage.texture = thumbnailElement.teaserTexture ?? thumbnailElement.texture ?? defaultTexture;
        } else if (thumbnailElement.Data.is_bought && thumbnailElement.Data.IsStarted) {
            rawImage.texture = thumbnailElement.texture ?? defaultTexture;
        } else {
            rawImage.texture = thumbnailElement.teaserTexture ?? thumbnailElement.texture ?? defaultTexture;
        }

        auAnimator.gameObject.SetActive(false);

        aspectRatioFitter.Refresh();
    }

    private void UpdateData() {
        UpdateTexture();
        auAnimator.gameObject.SetActive(true);

        txtTitle.text = thumbnailElement.Data.title;
        txtDescription.text = thumbnailElement.Data.description;

        btnWatchNow.SetActive(false);
        btnPlayTeaser.SetActive(false);
        btnBuyTicket.SetActive(false);

        if (thumbnailElement.Data.is_bought && thumbnailElement.Data.GetStage() == StreamJsonData.Data.Stage.Live) {
            btnWatchNow.SetActive(true);
        } else if (!thumbnailElement.Data.is_bought) {
            btnBuyTicket.SetActive(true);
        } else if (thumbnailElement.Data.is_bought && thumbnailElement.Data.IsStarted) {
            btnWatchNow.SetActive(true);
        } else {
            btnPlayTeaser.SetActive(thumbnailElement.Data.HasTeaser);
        }

        layoutGroup.enabled = !layoutGroup.enabled;
        layoutGroup.enabled = !layoutGroup.enabled;

        ResetLayout();

        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Delay(REFRESH_LAYOUT_TIME).ContinueWith((_) => ResetLayout(), taskScheduler);
    }

    private void ResetLayout() {
        if (layoutGroup != null) {
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }
    }

    private void WaitServerPurchaseConfirmation(long id) {
        if (thumbnailElement.Data.id != id)
            return;

        btnBuyTicket.SetActive(false);
    }

    private void OnDestroy() {
        if (isSubscribed)
            thumbnailWebDownloadManager.OnStreamByIdJsonDataLoaded -= ShowStreamStream;
    }

    private void OnEnable() {
        CallBacks.onStreamPurchasedInStore += WaitServerPurchaseConfirmation;
    }

    private void OnDisable() {
        CallBacks.onStreamPurchasedInStore -= WaitServerPurchaseConfirmation;
    }
}
