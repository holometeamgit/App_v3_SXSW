using System;
using UnityEngine;
using UnityEngine.UI;

public class BtnThumbnailItem : MonoBehaviour
{
    [SerializeField]
    RawImage imgThumbnail;

    [SerializeField]
    GameObject imgPastLifeGO;

    [SerializeField]
    GameObject imgLifeGO; 

    [SerializeField]
    Button buttonComponent;
    string code;

    public void SetThumbnailPressAction(Action<string> OnPress)
    {
        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(() => OnPress?.Invoke(code));
    }

    public void UpdateThumbnailData(string code, Texture texture)
    {
        imgThumbnail.texture = texture;
        this.code = code;
    }   
}
