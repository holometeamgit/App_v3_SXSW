using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnThumbnailItemV2 : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] Texture defaultTexture;
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
        rawImage.texture = thumbnailElement.texture ?? defaultTexture;
        //дата, имя и т.д. 
    }

    private void UpdateTexture(Texture texture) {
        rawImage.texture = thumbnailElement.texture ?? defaultTexture;
    }

    private void ErrorLoadTexture() {

    }

}
