using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Space]
    [SerializeField]
    AspectRatioFitterByMinSide aspectRatioFitter;
    [SerializeField]
    Texture defaultTexture;
    [SerializeField]
    UIAnimator auAnimator;

    ThumbnailElement thumbnailElement;

    //после покупки нужно проверить на домашней странице обновилось ли
    public void OpenStream(StreamJsonData.Data streamData) {
        if (streamData.is_bought) {
            uiThumbnailsController.PlayStream(streamData);
        } else {
            uiThumbnailsController.Buy(streamData);
        }

        ThumbnailElement element = new ThumbnailElement(streamData, webRequestHandler);
        AddData(element);

        ShowPnl();
    }

    public override void AddData(ThumbnailElement element) {
        if (thumbnailElement != null) {
            thumbnailElement.OnTextureLoaded -= UpdateTexture;
            thumbnailElement.OnErrorTextureLoaded -= UpdateTexture;
            thumbnailElement.Data.OnDataUpdated -= UpdateData;
        }

        auAnimator.gameObject.SetActive(true);

        thumbnailElement = element;

        thumbnailElement.OnTextureLoaded += UpdateTexture;
        thumbnailElement.OnErrorTextureLoaded += UpdateTexture;
        thumbnailElement.Data.OnDataUpdated += UpdateData;

        UpdateData();
    }

    public void ClosePnl() {
        gameObject.SetActive(false);
    }

    private void ShowPnl() {
        gameObject.SetActive(true);
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
    }
}
