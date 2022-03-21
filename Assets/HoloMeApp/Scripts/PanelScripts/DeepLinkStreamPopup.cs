using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Beem.SSO;
using System.Threading.Tasks;
using Beem.Permissions;
using Zenject;

public class DeepLinkStreamPopup : UIThumbnail {

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

    [Space]
    [SerializeField]
    private PurchaseManager _purchaseManager;
    private UserWebManager _userWebManager;
    private WebRequestHandler _webRequestHandler;

    private PermissionController _permissionController = new PermissionController();

    ThumbnailElement thumbnailElement;
    private const int REFRESH_LAYOUT_TIME = 1000;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, UserWebManager userWebManager) {
        _webRequestHandler = webRequestHandler;
        _userWebManager = userWebManager;
    }

    /// <summary>
    /// Play
    /// </summary>
    public override void Play() {
        Play(thumbnailElement.Data);
    }


    /// <summary>
    /// Play Teaser
    /// </summary>
    public override void PlayTeaser() {
        Play(thumbnailElement.Data);
    }


    /// <summary>
    /// Buy
    /// </summary>
    public override void Buy() {
        Buy(thumbnailElement.Data);
    }

    /// <summary>
    /// Close
    /// </summary>
    public void Close() {
        DeepLinkStreamConstructor.OnHide?.Invoke();
    }

    /// <summary>
    /// Buy Stadium/Prerecorded
    /// </summary>
    /// <param name="data"></param>
    private void Buy(StreamJsonData.Data data) {
        _purchaseManager.SetPurchaseStreamData(data);
        _purchaseManager.Purchase();
    }

    /// <summary>
    /// Play data
    /// </summary>
    /// <param name="data"></param>
    private void Play(StreamJsonData.Data data) {
        if (data.is_bought && data.IsStarted) {
            if (data.HasStreamUrl) {
                PlayPrerecorded(data);
            } else if (data.HasAgoraChannel) {
                PlayStadium(data);
            }
        } else if (data.HasTeaser) {
            PlayTeaser(data);
        }
    }

    /// <summary>
    /// Play Stadium
    /// </summary>
    /// <param name="roomJsonData"></param>
    private void PlayStadium(StreamJsonData.Data data) { //TODO split it to other class
        if (data.user == _userWebManager.GetUsername()) {
            WarningConstructor.ActivateSingleButton("Viewing as stream host",
                "Please connect to the stream using a different account");

            return;
        }

        if (data.agora_channel == "0" || string.IsNullOrWhiteSpace(data.agora_channel)) {
            return;
        }

        _permissionController.CheckCameraMicAccess(() => {
            DeepLinkStreamConstructor.OnHide?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
            StreamOverlayConstructor.onActivatedAsViewer?.Invoke(data.agora_channel, data.id.ToString(), false);
            PnlRecord.CurrentUser = data.user;
        });
    }

    /// <summary>
    /// Play Prerecorded
    /// </summary>
    /// <param name="roomJsonData"></param>
    private void PlayPrerecorded(StreamJsonData.Data data) { //TODO split it to other class
        _permissionController.CheckCameraMicAccess(() => {
            DeepLinkStreamConstructor.OnHide?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
            ARenaConstructor.onActivateForPreRecorded?.Invoke(data, false);
            PrerecordedVideoConstructor.OnActivated?.Invoke(data);
            PnlRecord.CurrentUser = data.user;
        });
    }

    /// <summary>
    /// Play Teaser
    /// </summary>
    /// <param name="data"></param>
    private void PlayTeaser(StreamJsonData.Data data) {
        _permissionController.CheckCameraMicAccess(() => {
            DeepLinkStreamConstructor.OnHide?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onDeactivatedAsBroadcaster?.Invoke();
            ARenaConstructor.onActivateForPreRecorded?.Invoke(data, data.HasTeaser);
            PrerecordedVideoConstructor.OnActivated?.Invoke(data);
            PnlRecord.CurrentUser = data.user;
            _purchaseManager.SetPurchaseStreamData(data);
        });
    }

    /// <summary>
    /// Show popup
    /// </summary>
    /// <param name="data"></param>
    public void Show(StreamJsonData.Data data) {
        gameObject.SetActive(true);

        ThumbnailElement element = new ThumbnailElement(data, _webRequestHandler);
        AddData(element);
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

    /// <summary>
    /// Hide popup
    /// </summary>
    public void Hide() {
        gameObject.SetActive(false);
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
