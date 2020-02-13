using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LogoCanvas : MonoBehaviour
{
    [SerializeField]
    Image spriteLogo;

    [SerializeField]
    Button btnLogoButton;

    VideoJsonData videoJsonDataRef;

    string url;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void ActivateIfLogoAvailable(VideoJsonData videoJsonData)
    {
        videoJsonDataRef = videoJsonData;

        if (videoJsonDataRef == null)
        {
            gameObject.SetActive(false);
            //Debug.LogError(nameof(videoJsonData.GetType) + "was null");
            return;
        }

        gameObject.SetActive(true);

        url = videoJsonDataRef.hyperlink;

        string path = HelperFunctions.PersistentDir() + videoJsonDataRef.logoImage;
        spriteLogo.sprite = LoadSprite(path);
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }

    public void SetParent(Transform parent)
    {
        var OriginalScale = transform.localScale;
        transform.parent = parent;
    }

    public void UpdateOffset(Vector3 position)
    {
        if (videoJsonDataRef != null)
        {
            transform.localPosition = videoJsonDataRef.GetOffsetVector();
        }
    }

    public void ButtonPressed()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }
}

