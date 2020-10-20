using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BtnThumbnailItemV2 : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] AspectRatioFitterByMinSide aspectRatioFitter;
    [SerializeField] Texture defaultTexture;
    [SerializeField] Image imgPurchaseIcon;
    [SerializeField] Image imgLive;

    [SerializeField] TMP_Text txtPerformerName;
    [SerializeField] TMP_Text txtDate;

    [SerializeField] ThumbnailsPurchaseStateScriptableObject thumbnailsPurchaseStateSO;

    ThumbnailElement thumbnailElement;

    Action<StreamJsonData.Data> OnClick;

    public void SetPressPlayAction(Action<StreamJsonData.Data> OnPlay) {
        this.OnClick = null;
        this.OnClick += OnClick;
    }

    public void TryPlay() {
        OnClick?.Invoke(thumbnailElement.Data);
    }

    public void AddData(ThumbnailElement element) {
        if (thumbnailElement != null) {
            thumbnailElement.OnTextureLoaded -= UpdateTexture;
            thumbnailElement.OnErrorTextureLoaded -= ErrorLoadTexture;
        }

        thumbnailElement = element;

        thumbnailElement.OnTextureLoaded += UpdateTexture;
        thumbnailElement.OnErrorTextureLoaded += ErrorLoadTexture;

        imgLive.gameObject.SetActive(thumbnailElement.Data.GetStatus() == StreamJsonData.Data.Stage.Live);

        UpdateData();
    }

    public void Deactivate() {
        rawImage.texture = defaultTexture;
        gameObject.SetActive(false);
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    //TODO если премя до, то одна надпись, если куплено и если будет, если нет тизера, то не показывать плей видео
    private void UpdateData() {
        UpdateTexture(thumbnailElement.texture);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rawImage.GetComponent<RectTransform>());

        txtPerformerName.text = thumbnailElement.Data.user;
        txtDate.text = thumbnailElement.Data.StartDate.ToString("dd MMM yyyy") ;
        if(System.DateTime.Now < thumbnailElement.Data.StartDate) {
            txtDate.text = txtDate.text + " • On sale now";
        }

        if (thumbnailElement.Data.product_type != null && !string.IsNullOrWhiteSpace(thumbnailElement.Data.product_type.product_id)) {
            imgPurchaseIcon.sprite = thumbnailsPurchaseStateSO.ThumbnailIcons[(int)PurchaseState.Paid];
        } else {
            imgPurchaseIcon.sprite = thumbnailsPurchaseStateSO.ThumbnailIcons[(int)PurchaseState.NeedPay];
        }
    }

    //TODO maybe remove
    #region update data 
    private void UpdateProductData(UnityEngine.Purchasing.Product product) {
        if (thumbnailElement.Data.product_type != null && thumbnailElement.Data.product_type.product_id == product.definition.id)
            SetPlayable();
    }

    private void SetPlayable() {
        imgPurchaseIcon.sprite = thumbnailsPurchaseStateSO.ThumbnailIcons[(int)PurchaseState.Free];
    }
    #endregion

    private void UpdateTexture(Texture texture) {
        rawImage.texture = thumbnailElement.texture ?? defaultTexture;

        aspectRatioFitter.Refresh();
    }

    private void ErrorLoadTexture() {

    }
}
