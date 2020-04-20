using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class PnlStreamOverlay : MonoBehaviour
{
    [SerializeField]
    GameObject controlsPresenter;

    [SerializeField]
    GameObject controlsViewer;

    [SerializeField]
    PnlGenericError pnlGenericError;

    [SerializeField]
    TextMeshProUGUI txtCountdown;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    UnityEvent OnClose;

    private void OnEnable()
    {
        FadePanel(true);
    }

    public void OpenAsStreamer()
    {
        controlsPresenter.SetActive(true);
        controlsViewer.SetActive(false);
    }

    public void OpenAsViewer()
    {
        controlsPresenter.SetActive(false);
        controlsViewer.SetActive(true);
    }

    public void FadePanel(bool show)
    {
        canvasGroup.DOFade(show ? 1 : 0, 0.5f).OnComplete(() => { if (!show) { gameObject.SetActive(false); } });
    }

    public void ShowLeaveWarning()
    {
        pnlGenericError.ActivateDoubleButton("End the live stream?", "Closing this page will end the live stream and disconnect your users.", onButtonOnePress: () => OnClose.Invoke(), onButtonTwoPress: () => pnlGenericError.GetComponent<AnimatedTransition>().DoMenuTransition(false));
    }

}
