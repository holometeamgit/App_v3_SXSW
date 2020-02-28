using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class FadeTransition : MonoBehaviour
{
    private const string MoveTweenName = "FadeTransitionTween";

    [SerializeField]
    Button[] postAnimationActiveButtons;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    UnityEvent OnShowTransitionComplete;

    RectTransform parentRect;

    bool initialStartup = true;

    float speed;

    private void Awake()
    {
        parentRect = GetComponent<RectTransform>();
        DoMenuTransition(false);
        speed = .25f;
    }

    public void OnEnable()
    {
        if (initialStartup)
        {
            initialStartup = false;
            return;
        }
        DoMenuTransition(true);
    }

    public void TogglePostAnimationBehaviours(bool activate)
    {
        foreach (MonoBehaviour behaviour in postAnimationActiveButtons)
        {
            behaviour.enabled = activate;
        }
    }

    public void DoMenuTransition(bool show)
    {
        if (show)
        {
            gameObject.SetActive(true);
            //Set in front for better effect
            parentRect.SetAsLastSibling();
        }
        else
        {
            TogglePostAnimationBehaviours(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.DOFade(show ? 1 : 0, speed).SetId(MoveTweenName).OnComplete(() =>
            {
                if (show)
                {
                    OnShowTransitionComplete?.Invoke();
                    TogglePostAnimationBehaviours(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            });
        }
        else
        {
            Debug.LogError("Canvas Group wasn't assigned for FadeTransition component " + transform.name);
        }
    }
}
