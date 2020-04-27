using UnityEngine;
using DG.Tweening;
using TMPro;
using NatShare;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

public class PnlStreamOverlay : MonoBehaviour
{
    [SerializeField]
    GameObject controlsPresenter;

    [SerializeField]
    GameObject controlsViewer;

    [SerializeField]
    PnlGenericError pnlGenericError;

    [SerializeField]
    TextMeshProUGUI txtCentreMessage;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    Toggle toggleAudio;

    [SerializeField]
    Toggle toggleVideo;

    [SerializeField]
    BlurController blurController;

    [SerializeField]
    UnityEvent OnClose;

    int countDown;
    string tweenAnimationID = nameof(tweenAnimationID);
    Coroutine countdownRoutine;

    private void OnEnable()
    {
        FadePanel(true);
        toggleAudio.isOn = false;
        toggleVideo.isOn = false;
        txtCentreMessage.text = string.Empty;
        EnableToggleControls(false);
    }

    public void OpenAsStreamer()
    {
        blurController.RemoveBlur();
        gameObject.SetActive(true);
        controlsPresenter.SetActive(true);
        controlsViewer.SetActive(false);
    }

    public void OpenAsViewer()
    {
        blurController.RemoveBlur();
        gameObject.SetActive(true);
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

    public void ShareStream()
    {
        using (var payload = new SharePayload())
        {
            string appLink;
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    appLink = "https://tinyurl.com/HoloMeiOS";//"https://apps.apple.com/us/app/holome/id1454364021";
                    break;

                case RuntimePlatform.Android:
                    appLink = "https://tinyurl.com/HoloMeAndroid";//"https://play.google.com/store/apps/details?id=com.HoloMe.Showreel&hl=en_GB";
                    break;

                default:
                    appLink = "https://tinyurl.com/HoloMeiOS - https://tinyurl.com/HoloMeAndroid";
                    break;
            }

            payload.AddText($"Check out my stream in the HoloMe App using the channel {AgoraController.ChannelName} app {appLink}");
        }
    }

    public void StartCountdown()
    {
        countdownRoutine = StartCoroutine(CountDown());
    }

    public void StopStream()
    {
        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);

        EnableToggleControls(false);
    }

    void StartStream()
    {
        EnableToggleControls(true);
    }

    private void EnableToggleControls(bool enable)
    {
        toggleAudio.interactable = enable;
        toggleVideo.interactable = enable;
    }

    public void ToggleAudio(bool mute)
    {
        TogglePauseStream();
    }

    public void ToggleVideo(bool hideVideo)
    {
        TogglePauseStream();
    }

    void TogglePauseStream()
    {
        if (toggleVideo.isOn && toggleAudio.isOn)
        {
            AnimatedCentreTextMessage("Stream Paused");
        }
        else
        {
            AnimatedFadeOutMessage();
        }
    }

    IEnumerator CountDown()
    {
        countDown = 3;

        while (countDown >= 0)
        {
            AnimatedCentreTextMessage(countDown > 0 ? countDown.ToString() : "ON AIR");
            AnimatedFadeOutMessage(.5f);
            countDown--;
            yield return new WaitForSeconds(1);
        }

        StartStream();
    }

    private void AnimatedCentreTextMessage(string message)
    {
        DOTween.Kill(tweenAnimationID);
        txtCentreMessage.rectTransform.localScale = Vector3.one;
        txtCentreMessage.text = message;
        txtCentreMessage.color = new Color(txtCentreMessage.color.r, txtCentreMessage.color.g, txtCentreMessage.color.b, 1);
        txtCentreMessage.rectTransform.DOPunchScale(Vector3.one, .25f, 3).SetId(tweenAnimationID);
    }

    private void AnimatedFadeOutMessage(float delay = 0)
    {
        txtCentreMessage.DOFade(0, .5f).SetDelay(delay).SetId(tweenAnimationID);
    }
}
