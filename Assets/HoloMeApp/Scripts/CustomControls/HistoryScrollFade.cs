using UnityEngine;
using UnityEngine.UI;

public class HistoryScrollFade : MonoBehaviour
{
    [SerializeField]
    bool isBottom;
    [SerializeField]
    ScrollRect scrollRect;
    [SerializeField]
    RectTransform content;
    [SerializeField]
    RectTransform viewport;

    Image imgTexture;

    private void Awake()
    {
        imgTexture = GetComponent<Image>();
    }

    private void OnEnable()
    {
        UpdateOpacity();
    }

    //Link this to scroll rect's on change event
    public void UpdateOpacity()
    {
        //print(scrollRect.verticalNormalizedPosition);

        if (imgTexture == null)
        {
            Debug.LogError($"{nameof(HistoryScrollFade)} Image wasn't found");
            return;
        }

        if (viewport.rect.height >= content.rect.height)
        {
            imgTexture.color = new Color(imgTexture.color.r, imgTexture.color.g, imgTexture.color.b, 0);
            return;
        }

        if (isBottom) //Full opacity when scroller is at the top
        {
            imgTexture.color = new Color(imgTexture.color.r, imgTexture.color.g, imgTexture.color.b, scrollRect.verticalNormalizedPosition);
        }
        else
        {
            imgTexture.color = new Color(imgTexture.color.r, imgTexture.color.g, imgTexture.color.b, 1 - scrollRect.verticalNormalizedPosition);
        }
    }
}
