using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Beem.SSO;
using System.Threading.Tasks;
using Zenject;

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
    AspectRatioFitterByMinSide aspectRatioFitter;
    [SerializeField]
    Texture defaultTexture;
    [SerializeField]
    UIAnimator auAnimator;

    [SerializeField]
    private VerticalLayoutGroup layoutGroup;

    private WebRequestHandler _webRequestHandler;
    private ContentPlayer _contentPlayer;

    ThumbnailElement thumbnailElement;
    long currentId = 0;

    const long DEFAUL_STREAM_DATA_ID = 0;
    private const int REFRESH_LAYOUT_TIME = 1000;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, UserWebManager userWebManager, PurchaseManager purchaseManager) {
        _webRequestHandler = webRequestHandler;
        _contentPlayer = new ContentPlayer(userWebManager, purchaseManager);
    }

    public override void Play() {
        _contentPlayer.Play(thumbnailElement.Data);
    }

    public override void PlayTeaser() {
        _contentPlayer.Play(thumbnailElement.Data);
    }

    public override void Buy() {
        _contentPlayer.Buy(thumbnailElement.Data);
    }

    public void OpenStream(StreamJsonData.Data data) {
        currentId = data.id;
        ShowStreamStream(data);
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
        currentId = DEFAUL_STREAM_DATA_ID;

        gameObject.SetActive(false);
    }

    private void ShowPnl() {
        gameObject.SetActive(true);
    }

    private void ShowStreamStream(StreamJsonData.Data streamData) {
        if (streamData.id != currentId ||
            !((streamData.GetStage() == StreamJsonData.Data.Stage.Prerecorded && streamData.HasStreamUrl) || streamData.GetStage() == StreamJsonData.Data.Stage.Live))
            return;
        HelperFunctions.DevLog("Thumbnail popup open stream id " + streamData.id);
        /* autoplay
         * if (streamData.is_bought) {
            uiThumbnailsController.Play(streamData);
        }*/

        ThumbnailElement element = new ThumbnailElement(streamData, _webRequestHandler);
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

    private void OnEnable() {
        CallBacks.onStreamPurchasedInStore += WaitServerPurchaseConfirmation;
    }

    private void OnDisable() {
        CallBacks.onStreamPurchasedInStore -= WaitServerPurchaseConfirmation;
    }
}
