using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class AnimatedTransition : MonoBehaviour
{
    private const string MoveTweenName = "MoveTween";

    [SerializeField]
    Button[] postAnimationActiveButtons;

    [SerializeField]
    RectTransform rectToMove;

    [SerializeField]
    Image imgBackground;

    [SerializeField]
    AnimDir dir = AnimDir.Left;

    [SerializeField]
    UnityEvent OnShowTransitionComplete;

    [SerializeField]
    UnityEvent OnHideTransitionComplete;

    [Range(0, 255)]
    [SerializeField]
    int alphaFadeToValue = 100;

    public enum AnimDir { Left, Right, Down };

    RectTransform parentRect;

    bool initialStartup = true;

    Vector2 originalPosition;

    float speed;

    private void Awake()
    {
        parentRect = GetComponent<RectTransform>();
        originalPosition = rectToMove?.anchoredPosition ?? Vector2.zero;
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

        switch (dir)
        {
            case AnimDir.Left:
                rectToMove?.DOAnchorPosX(show ? originalPosition.x : -rectToMove.rect.width, speed).SetId(MoveTweenName);
                break;
            case AnimDir.Down:
                rectToMove?.DOAnchorPosY(show ? originalPosition.y : -rectToMove.rect.height, speed).SetId(MoveTweenName);
                break;
            case AnimDir.Right:
                rectToMove?.DOAnchorPosX(show ? originalPosition.x : rectToMove.rect.width, speed).SetId(MoveTweenName);
                break;
        }

        if (imgBackground != null)
        {
            imgBackground.DOFade(show ? alphaFadeToValue / 255f : 0, speed).SetId(MoveTweenName).OnComplete(() =>
            {
                if (show)
                {
                    OnShowTransitionComplete?.Invoke();
                    TogglePostAnimationBehaviours(true);
                }
                else
                {
                    gameObject.SetActive(false);
                    OnHideTransitionComplete?.Invoke();
                }
            });
        }
        else
        {
            Debug.LogError("Transition background image was null this will cause post animation issues " + transform.name);
        }
    }
}