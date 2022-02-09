using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Beem.SSO;
using Beem;
using Beem.UI;
using System.Globalization;

public class UIThumbnailV3 : UIThumbnail {
    [SerializeField] RawImage rawImage;
    [SerializeField] Texture defaultTexture;
    [Space]
    [SerializeField] Image imgLive;
    [Header("Btns")]
    [SerializeField] GameObject btnThumbnail;
    [SerializeField] GameObject btnWatchNow;
    [SerializeField] GameObject btnPlayTeaser;
    [SerializeField] GameObject btnBuyTicketR;
    [SerializeField] GameObject btnShareEvent;

    [Header("text info")]
    [SerializeField] TMP_Text txtDate;
    [SerializeField] TMP_Text txtTime;
    [SerializeField] TMP_Text txtTitle;
    [SerializeField] TMP_Text txtDescription;
    [SerializeField] TMP_Text txtInfoText;
    [SerializeField] TMP_Text txtPrice;

    [Header("Like")]
    [SerializeField] UIBtnLikes btnLikes;

    [Header("Views")]
    [SerializeField] TMP_Text txtViews;

    [Space]
    [SerializeField] AspectRatioFitterByMinSide aspectRatioFitter;

    [SerializeField]
    private StreamTimerView _streamTimerView;

    Action<StreamJsonData.Data> OnPlayClick;
    Action<StreamJsonData.Data> OnTeaserClick;
    Action<StreamJsonData.Data> OnBuyClick;
    Action<StreamJsonData.Data> OnShareClick;

    ThumbnailElement thumbnailElement;

    public override void SetPlayAction(Action<StreamJsonData.Data> OnPlayClick) {
        this.OnPlayClick = null;
        this.OnPlayClick += OnPlayClick;
    }

    public override void SetTeaserPlayAction(Action<StreamJsonData.Data> OnTeaserClick) {
        this.OnTeaserClick = null;
        this.OnTeaserClick += OnTeaserClick;
    }

    public override void SetBuyAction(Action<StreamJsonData.Data> OnBuyClick) {
        this.OnBuyClick = null;
        this.OnBuyClick += OnBuyClick;
    }

    public override void SetShareAction(Action<StreamJsonData.Data> OnShareClick) {
        this.OnShareClick = null;
        this.OnShareClick += OnShareClick;
    }

    public override void ThumbnailClick() {
        base.ThumbnailClick();

        if (thumbnailElement.Data.is_bought && thumbnailElement.Data.GetStage() == StreamJsonData.Data.Stage.Live) {
            Play();
        } else if (thumbnailElement.Data.is_bought && thumbnailElement.Data.IsStarted && thumbnailElement.Data.HasStreamUrl) {
            Play();
        } else if (!thumbnailElement.Data.is_bought && !thumbnailElement.Data.HasTeaser) {
            Buy();
        } else if (thumbnailElement.Data.HasTeaser) {
            PlayTeaser();
        }
    }

    public void OpenComment() {
        CommentsControllersConstructor.OnShow?.Invoke(thumbnailElement.Data);
    }

    public override void Play() {
        OnPlayClick?.Invoke(thumbnailElement.Data);
    }

    public override void PlayTeaser() {
        OnTeaserClick?.Invoke(thumbnailElement.Data);
    }

    public override void Buy() {
        OnBuyClick?.Invoke(thumbnailElement.Data);
    }

    public override void Share() {
        OnShareClick?.Invoke(thumbnailElement.Data);
    }

    public override void AddData(ThumbnailElement element) {
        if (thumbnailElement != null) {
            thumbnailElement.OnTextureLoaded -= UpdateTexture;
            thumbnailElement.OnErrorTextureLoaded -= UpdateTexture;
            thumbnailElement.Data.OnDataUpdated -= UpdateData;
        }

        thumbnailElement = element;
#if UNITY_EDITOR
        name = this.GetType().Name + " (" + element.Data.id + ")";
#endif

        thumbnailElement.OnTextureLoaded += UpdateTexture;
        thumbnailElement.OnErrorTextureLoaded += UpdateTexture;
        thumbnailElement.Data.OnDataUpdated += UpdateData;

        UpdateData();
    }

    public override void LockToPress(bool isLook) {
        try {
            btnThumbnail.GetComponent<Button>().interactable = !isLook;
            btnWatchNow.GetComponent<Button>().interactable = !isLook;
            btnPlayTeaser.GetComponent<Button>().interactable = !isLook;
            btnBuyTicketR.GetComponent<Button>().interactable = !isLook;
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    public override void Deactivate() {
        gameObject.SetActive(false);

        txtDate.text = "";
        txtTime.text = "";
        txtTitle.text = "";
        txtViews.text = "";
        txtDescription.text = "";
        txtInfoText.text = "";
        txtPrice.text = "";
        _streamTimerView.Clear();
    }

    public override void Activate() {
        gameObject.SetActive(true);

        LockToPress(true);
    }

    private void UpdateData() {
        if (thumbnailElement == null || thumbnailElement.Data == null)
            return;

        UpdateTexture();

        bool isLive = thumbnailElement.Data.GetStage() == StreamJsonData.Data.Stage.Live;
        imgLive.gameObject.SetActive(isLive);

        string day = OrdinalNumberSuffix.AddOrdinalNumberSuffixDat(thumbnailElement.Data.StartDate.Day);

        txtDate.text = thumbnailElement.Data.StartDate.ToString("MMM ") + day;
        txtTime.text = thumbnailElement.Data.StartDate.ToString("HH:mm");
        txtTitle.text = thumbnailElement.Data.title;
        txtDescription.text = thumbnailElement.Data.description;

        txtViews.text = DataStringConverter.GetItems(thumbnailElement.Data.count_of_views, "view", "views");

        btnWatchNow.SetActive(false);
        btnPlayTeaser.SetActive(false);
        btnBuyTicketR.SetActive(false);
        btnShareEvent.SetActive(false);

        txtPrice.text = "Free";

        if (thumbnailElement.Data.is_bought && thumbnailElement.Data.GetStage() == StreamJsonData.Data.Stage.Live) {
            txtInfoText.text = "This is a free Live event";

            btnWatchNow.SetActive(true);
        } else if (!thumbnailElement.Data.is_bought) {
            txtInfoText.text = "Please purchase a ticket to watch full event";
            txtPrice.text = "$" + thumbnailElement.Data.product_type.price.ToString();

            btnPlayTeaser.SetActive(thumbnailElement.Data.HasTeaser);
            btnBuyTicketR.SetActive(!thumbnailElement.Data.HasTeaser);
        } else if (thumbnailElement.Data.is_bought && thumbnailElement.Data.IsStarted) {
            txtInfoText.text = "This is a free event";
            if (thumbnailElement.Data.is_bought && thumbnailElement.Data.HasProduct)
                txtInfoText.text = "Ticket purchased for event";
            btnWatchNow.SetActive(thumbnailElement.Data.HasStreamUrl);
            btnPlayTeaser.SetActive(!thumbnailElement.Data.HasStreamUrl && thumbnailElement.Data.HasTeaser);
            btnShareEvent.SetActive(!thumbnailElement.Data.HasStreamUrl && !thumbnailElement.Data.HasTeaser);
        } else {
            txtInfoText.text = "Ticket purchased for event scheduled on " + thumbnailElement.Data.StartDate.ToString("ddd d MMM");

            btnPlayTeaser.SetActive(thumbnailElement.Data.HasTeaser);
            btnShareEvent.SetActive(!thumbnailElement.Data.HasTeaser);
        }

        btnLikes.Init(thumbnailElement.Data.id);

        _streamTimerView.View(thumbnailElement.Data);
    }

    private void WaitServerPurchaseConfirmation(long id) {
        if (thumbnailElement.Data.id != id)
            return;

        btnBuyTicketR.SetActive(false);
    }

    private void UpdateTexture() {

        if (!thumbnailElement.Data.is_bought) {
            rawImage.texture = thumbnailElement.teaserTexture ?? thumbnailElement.texture ?? defaultTexture;
        } else if (thumbnailElement.Data.is_bought && thumbnailElement.Data.IsStarted) {
            rawImage.texture = thumbnailElement.texture ?? thumbnailElement.teaserTexture ?? defaultTexture;
        } else {
            rawImage.texture = thumbnailElement.teaserTexture ?? thumbnailElement.texture ?? defaultTexture;
        }

        aspectRatioFitter.Refresh();
    }

    private void OnDestroy() {
        if (thumbnailElement != null) {
            thumbnailElement.OnTextureLoaded -= UpdateTexture;
            thumbnailElement.OnErrorTextureLoaded -= UpdateTexture;
            thumbnailElement.Data.OnDataUpdated -= UpdateData;
        }
    }

    private void OnEnable() {
        CallBacks.onStreamPurchasedInStore += WaitServerPurchaseConfirmation;
        UpdateData();
    }

    private void OnDisable() {
        CallBacks.onStreamPurchasedInStore -= WaitServerPurchaseConfirmation;
    }
}
