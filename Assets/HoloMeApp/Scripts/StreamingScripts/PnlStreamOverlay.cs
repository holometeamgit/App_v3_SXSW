using UnityEngine;
using DG.Tweening;
using TMPro;
using NatShare;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using agora_gaming_rtc;

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
    TextMeshProUGUI txtUserCount;

    [SerializeField]
    CanvasGroup canvasGroup;

    [SerializeField]
    Toggle toggleAudio;

    [SerializeField]
    Toggle toggleVideo;

    [SerializeField]
    Button btnFlipCamera;

    [SerializeField]
    BlurController blurController;

    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    PnlViewingExperience pnlViewingExperience;

    [SerializeField]
    RawImage cameraRenderImage;

    [SerializeField]
    PermissionGranter permissionGranter;

    [SerializeField]
    GameObject arSessionOrigin;

    [SerializeField]
    GameObject arSession;

    [SerializeField]
    UnityEvent OnCloseAsViewer;

    [SerializeField]
    UnityEvent OnCloseAsStreamer;

    int countDown;
    string tweenAnimationID = nameof(tweenAnimationID);
    Coroutine countdownRoutine;
    bool isStreamer;
    bool isUsingFrontCamera;

    private void Awake()
    {
        agoraController.OnCountIncremented += (x) => txtUserCount.text = x.ToString();
        agoraController.OnStreamerLeft += CloseAsViewer;
        agoraController.OnCameraSwitched += () =>
        {
            var videoSurface = cameraRenderImage.GetComponent<VideoSurface>();
            if (videoSurface)
            {
                isUsingFrontCamera = !isUsingFrontCamera;
                videoSurface.EnableFlipTextureApplyTransform(!isUsingFrontCamera, false);
            }
        };
        //cameraRenderImage.materialForRendering.SetFloat("_UseBlendTex", 0);
    }

    private void OnEnable()
    {
        FadePanel(true);
        toggleAudio.isOn = false;
        toggleVideo.isOn = false;
        txtCentreMessage.text = string.Empty;
        EnableStreamControls(false);
        RequestMicAccess();
    }

    private void RequestMicAccess()
    {
        if (!permissionGranter.MicAccessAvailable && !permissionGranter.MicRequestComplete)
        {
            permissionGranter.RequestMicAccess();
        }
    }

    private void ToggleARSessionObjects(bool enable)
    {
        arSessionOrigin.SetActive(enable);
        arSession.SetActive(enable);
    }

    public void OpenAsStreamer()
    {
        isStreamer = true;
        gameObject.SetActive(true);
        controlsPresenter.SetActive(true);
        controlsViewer.SetActive(false);
        ToggleARSessionObjects(false);
        cameraRenderImage.transform.parent.gameObject.SetActive(true);
        StartCountdown();
    }

    public void OpenAsViewer()
    {
        isStreamer = false;
        blurController.RemoveBlur();
        gameObject.SetActive(true);
        controlsPresenter.SetActive(false);
        controlsViewer.SetActive(true);
        pnlViewingExperience.ActivateForStreaming();
        cameraRenderImage.transform.parent.gameObject.SetActive(false);
        agoraController.JoinOrCreateChannel(false);
    }

    public void FadePanel(bool show)
    {
        canvasGroup.DOFade(show ? 1 : 0, 0.5f).OnComplete(() => { if (!show) { gameObject.SetActive(false); } });
    }

    public void ShowLeaveWarning()
    {
        if (isStreamer)
            pnlGenericError.ActivateDoubleButton("End the live stream?", "Closing this page will end the live stream and disconnect your users.", onButtonOnePress: () => { OnCloseAsStreamer.Invoke(); StopStream(); }, onButtonTwoPress: () => pnlGenericError.GetComponent<AnimatedTransition>().DoMenuTransition(false));
        else
            pnlGenericError.ActivateDoubleButton("Disconnect from live stream?", "Closing this page will disconnect you from the live stream", onButtonOnePress: () => { CloseAsViewer(); }, onButtonTwoPress: () => pnlGenericError.GetComponent<AnimatedTransition>().DoMenuTransition(false));
    }

    private void CloseAsViewer()
    {
        OnCloseAsViewer.Invoke();
        StopStream();
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

            payload.AddText($"Check out my stream in the HoloMe App using the channel {agoraController.ChannelName} app {appLink}");
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

        EnableStreamControls(false);
        agoraController.Leave();
    }

    void StartStream()
    {
        agoraController.JoinOrCreateChannel(true);
        EnableStreamControls(true);
        AddVideoSurface();
    }

    private void AddVideoSurface()
    {
        VideoSurface videoSurface = cameraRenderImage.GetComponent<VideoSurface>();
        if (!videoSurface)
        {
            videoSurface = cameraRenderImage.gameObject.AddComponent<VideoSurface>();
            isUsingFrontCamera = true;
            videoSurface.EnableFlipTextureApplyTransform(true, true);
            //videoSurface.EnableFilpTextureApply(true, true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(agoraController.frameRate);
            videoSurface.SetEnable(true);
        }
        StartCoroutine(Resize());
    }

    IEnumerator Resize()
    {
        yield return new WaitForSeconds(3);
        cameraRenderImage.SizeToParent();
    }

    private void EnableStreamControls(bool enable)
    {
        toggleAudio.interactable = enable;
        toggleVideo.interactable = enable;
        btnFlipCamera.interactable = enable;
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
        countDown = 0;

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

    private void OnDisable()
    {
        StopAllCoroutines();
        ToggleARSessionObjects(true);
    }
}
