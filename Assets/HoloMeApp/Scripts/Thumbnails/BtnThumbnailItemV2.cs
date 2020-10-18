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

    [SerializeField] ThumbnailsPurchaseStateScriptableObject thumbnailsPurchaseStateScriptableObject;

    ThumbnailElement thumbnailElement;

    Action<StreamJsonData.Data.Stage, string> OnPlay;

    //TODO обновление 3-х статусов
    //статус после покупки обновляется только когда мы отправили информацию о покупке,
    //потом запросили у сервера и только после этого повторного запроса мы обновляем на UI об покупке 

    public void SetThumbnailPressAction(Action<StreamJsonData.Data.Stage, string> OnPlay) {
        this.OnPlay = null;
        this.OnPlay += OnPlay;
    }

    public void TryPlay() {
        if(thumbnailElement.Data.product_type != null && !string.IsNullOrWhiteSpace(thumbnailElement.Data.product_type.product_id)) { // TODO add check if purchased
            Debug.Log("Try Buy " + thumbnailElement.Data.product_type.name);
            FindObjectOfType<IAPController>().BuyTicket(thumbnailElement.Data.product_type.product_id);
        } else {
            if (thumbnailElement.Data.StartDate > DateTime.Now)
                return;

            if (!string.IsNullOrWhiteSpace(thumbnailElement.Data.stream_s3_url))
                OnPlay?.Invoke(StreamJsonData.Data.Stage.Finished, thumbnailElement.Data.stream_s3_url);
            else if (!string.IsNullOrWhiteSpace(thumbnailElement.Data.agora_channel) && thumbnailElement.Data.GetStatus() == StreamJsonData.Data.Stage.Live)
                OnPlay?.Invoke(StreamJsonData.Data.Stage.Live, thumbnailElement.Data.agora_channel);
        }
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

    private void Awake() {
        FindObjectOfType<IAPController>().OnPurchaseHandler += UpdateProductData;
    }

    private void UpdateProductData(UnityEngine.Purchasing.Product product) {
        if (thumbnailElement.Data.product_type != null && thumbnailElement.Data.product_type.product_id == product.definition.id)
            SetPlayable();
    }

    private void SetPlayable() {
        imgPurchaseIcon.sprite = thumbnailsPurchaseStateScriptableObject.ThumbnailIcons[2];
    }

    private void UpdateData() {
        UpdateTexture(thumbnailElement.texture);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rawImage.GetComponent<RectTransform>());

        txtPerformerName.text = thumbnailElement.Data.user;
        txtDate.text = thumbnailElement.Data.StartDate.ToString("dd MMM yyyy") ;
        if(System.DateTime.Now < thumbnailElement.Data.StartDate) {//TODO specify when this inscription should be here
            txtDate.text = txtDate.text + " • On sale now";
        }

        if (thumbnailElement.Data.product_type != null && !string.IsNullOrWhiteSpace(thumbnailElement.Data.product_type.product_id)) {
            imgPurchaseIcon.sprite = thumbnailsPurchaseStateScriptableObject.ThumbnailIcons[0];
        } else {
            imgPurchaseIcon.sprite = thumbnailsPurchaseStateScriptableObject.ThumbnailIcons[2];
        }
    }

    private void UpdateTexture(Texture texture) {
        rawImage.texture = thumbnailElement.texture ?? defaultTexture;

        aspectRatioFitter.Refresh();
    }

    private void ErrorLoadTexture() {

    }

    private void OnDestroy() {
        IAPController iAPController = FindObjectOfType<IAPController>();
        if(iAPController != null)
            iAPController.OnPurchaseHandler -= UpdateProductData;
    }

}
