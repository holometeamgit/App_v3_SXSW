using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class SplashFade : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnComplete;

    CanvasGroup canvasGroup;

    [Tooltip("In seconds")]
    [SerializeField]
    float fadeSpeed = 2f;

    [SerializeField]
    float delayPriorToFade = 0.5f;

    [SerializeField]
    GameObject txtStaging;

    void Start()
    {
        SetDrawOrderLast();

#if STAGING
        txtStaging.SetActive(true);
#else
        txtStaging.SetActive(false);
#endif
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0, fadeSpeed).SetDelay(delayPriorToFade).OnComplete(() => OnComplete?.Invoke());
    }

    private void SetDrawOrderLast()
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.SetAsLastSibling();
    }
}
