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

    public void Click() {
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

        //text 'Watch the teaser now' 'Event coming soon' 'Watch now!'
        txtPerformerName.text = thumbnailElement.Data.user;
        txtDate.text = thumbnailElement.Data.StartDate.ToString("dd MMM yyyy") ;
        if(!thumbnailElement.Data.is_bought && thumbnailElement.Data.HasTeaser) {
            txtDate.text = txtDate.text + " • Watch the teaser now";
        } else if(thumbnailElement.Data.is_bought && thumbnailElement.Data.IsStarted) {
            txtDate.text = txtDate.text + " • Watch now!";
        } else {
            txtDate.text = txtDate.text + " • Event coming soon";
        }

        //icon don't show if we have not teaser before event or we have not eny data
        imgPurchaseIcon.gameObject.SetActive(thumbnailElement.Data.HasTeaser ||
            (thumbnailElement.Data.IsStarted && (thumbnailElement.Data.HasStreamUrl || thumbnailElement.Data.HasAgoraChannel)));
    }

    private void UpdateTexture(Texture texture) {
        rawImage.texture = thumbnailElement.texture ?? defaultTexture;

        aspectRatioFitter.Refresh();
    }

    private void ErrorLoadTexture() {

    }
}
