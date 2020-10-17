using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BtnThumbnailItemV2 : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] AspectRatioFitterByMinSide aspectRatioFitter;
    [SerializeField] Texture defaultTexture;
    [SerializeField] Image imgPurchaseIcon;

    [SerializeField]
    TMP_Text txtPerformerName;

    [SerializeField]
    TMP_Text txtDate;

    ThumbnailElement thumbnailElement;

    public void AddData(ThumbnailElement element) {
        if (thumbnailElement != null) {
            thumbnailElement.OnTextureLoaded -= UpdateTexture;
            thumbnailElement.OnErrorTextureLoaded -= ErrorLoadTexture;
        }

        thumbnailElement = element;

        thumbnailElement.OnTextureLoaded += UpdateTexture;
        thumbnailElement.OnErrorTextureLoaded += ErrorLoadTexture;

        UpdateData();
    }

    public void Deactivate() {
        rawImage.texture = defaultTexture;
        gameObject.SetActive(false);
    }

    public void Activate() {
        gameObject.SetActive(true);
    }

    private void UpdateData() {
        UpdateTexture(thumbnailElement.texture);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rawImage.GetComponent<RectTransform>());

        txtPerformerName.text = thumbnailElement.Data.user;
        txtDate.text = thumbnailElement.Data.StartDate.ToString("dd MMM yyyy") ;
        if(true) {
            txtDate.text = txtDate.text + " • On sale now";
        }
        //дата, имя и т.д. 
    }

    private void UpdateTexture(Texture texture) {
        rawImage.texture = thumbnailElement.texture ?? defaultTexture;

        aspectRatioFitter.Refresh();
    }

    private void ErrorLoadTexture() {

    }

}
