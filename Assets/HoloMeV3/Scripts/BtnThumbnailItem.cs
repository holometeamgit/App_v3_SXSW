using UnityEngine;
using UnityEngine.UI;

public class BtnThumbnailItem : MonoBehaviour
{
    Image imgThumbnail;
    Button buttonComponent;
    string code;

    private void Awake()
    {
        imgThumbnail = GetComponent<Image>();
        buttonComponent = GetComponent<Button>();
    }

    public void UpdateThumbnailData(string code, Sprite sprite)
    {
        imgThumbnail.sprite = sprite;
        this.code = code;
    }
}
