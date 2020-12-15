using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using agora_gaming_rtc;

public class PnlStreamOverlay : MonoBehaviour {
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
    UserWebManager userWebManager;

    [SerializeField]
    ShareManager shareManager;

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

    Vector3 rawImageQuadDefaultScale;

    private void Awake() {

        if (rawImageQuadDefaultScale == Vector3.zero)
            rawImageQuadDefaultScale = cameraRenderImage.transform.localScale;

        agoraController.OnCountIncremented += (x) => txtUserCount.text = x.ToString();
        agoraController.OnStreamerLeft += CloseAsViewer;
        agoraController.OnCameraSwitched += () => {
            var videoSurface = cameraRenderImage.GetComponent<VideoSurface>();
            if (videoSurface) {
                isUsingFrontCamera = !isUsingFrontCamera;
                videoSurface.EnableFlipTextureApplyTransform(!isUsingFrontCamera, false, rawImageQuadDefaultScale); //This may need to be adjusted if camera flip button ever comes back
            }
        };
        
        //cameraRenderImage.materialForRendering.SetFloat("_UseBlendTex", 0);

        AddVideoSurface();
    }

    private void OnEnable() {
        FadePanel(true);
        toggleAudio.isOn = false;
        toggleVideo.isOn = false;
        txtCentreMessage.text = string.Empty;
        EnableStreamControls(false);
        RequestMicAccess();
    }

    private void RequestMicAccess() {
        if (!permissionGranter.MicAccessAvailable && !permissionGranter.MicRequestComplete) {
            permissionGranter.RequestMicAccess();
        }
    }

    private void ToggleARSessionObjects(bool enable) {
        arSessionOrigin.SetActive(enable);
        arSession.SetActive(enable);
    }

    public void OpenAsStreamer() {
        agoraController.ChannelName = userWebManager.GetUsername();
        isStreamer = true;
        gameObject.SetActive(true);
        controlsPresenter.SetActive(true);
        controlsViewer.SetActive(false);
        ToggleARSessionObjects(false);
        cameraRenderImage.transform.parent.gameObject.SetActive(true);
        //StartCountdown();
        agoraController.StartPreview();
        StartCoroutine(OnPreviewReady());
    }

    public void OpenAsViewer() {
        isStreamer = false;
        blurController.RemoveBlur();
        gameObject.SetActive(true);
        controlsPresenter.SetActive(false);
        controlsViewer.SetActive(true);
        pnlViewingExperience.ActivateForStreaming();
        cameraRenderImage.transform.parent.gameObject.SetActive(false);
        agoraController.JoinOrCreateChannel(false);
    }

    public void FadePanel(bool show) {
        canvasGroup.DOFade(show ? 1 : 0, 0.5f).OnComplete(() => { if (!show) { gameObject.SetActive(false); } });
    }

    public void ShowLeaveWarning() {
        if (isStreamer)
            pnlGenericError.ActivateDoubleButton("End the live stream?", "Closing this page will end the live stream and disconnect your users.", onButtonOnePress: () => { CloseAsStreamer(); }, onButtonTwoPress: () => pnlGenericError.GetComponent<AnimatedTransition>().DoMenuTransition(false));
        else
            pnlGenericError.ActivateDoubleButton("Disconnect from live stream?", "Closing this page will disconnect you from the live stream", onButtonOnePress: () => { CloseAsViewer(); }, onButtonTwoPress: () => pnlGenericError.GetComponent<AnimatedTransition>().DoMenuTransition(false));
    }

    public void CloseAsStreamer() {
        OnCloseAsStreamer.Invoke();
        StopStream();
    }

    private void CloseAsViewer() {
        OnCloseAsViewer.Invoke();
        StopStream();
    }

    public void ShareStream() {
        if (shareManager == null) {
            shareManager = FindObjectOfType<ShareManager>();
            Debug.LogWarning("Share manager wasn't assigned in the inspect used find to replace");
        }
        shareManager.ShareStream();
    }

    public void StartCountdown() {
        countdownRoutine = StartCoroutine(CountDown());
    }

    public void StopStream() {
        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);

        EnableStreamControls(false);
        agoraController.Leave();
        cameraRenderImage.texture = null;
    }

    void StartStream() {
        agoraController.JoinOrCreateChannel(true);
        EnableStreamControls(true);
    }

    private void AddVideoSurface() {
        VideoSurface videoSurface = cameraRenderImage.GetComponent<VideoSurface>();
        if (!videoSurface) {
            videoSurface = cameraRenderImage.gameObject.AddComponent<VideoSurface>();
            isUsingFrontCamera = true;
            videoSurface.EnableFlipTextureApplyTransform(false, true, rawImageQuadDefaultScale);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(agoraController.frameRate);
            videoSurface.SetEnable(true);
        }
    }

    IEnumerator OnPreviewReady() {
        if(!agoraController.VideoIsReady || cameraRenderImage.texture == null)
            AnimatedCentreTextMessage("Loading Preview");

        while (!agoraController.VideoIsReady || cameraRenderImage.texture == null) {
            yield return null;
        }
        //yield return new WaitForSeconds(3);
        //cameraRenderImage.SizeToParent();
        AnimatedFadeOutMessage();
    }

    private void EnableStreamControls(bool enable) {
        toggleAudio.interactable = enable;
        toggleVideo.interactable = enable;
        btnFlipCamera.interactable = enable;
    }

    public void ToggleAudio(bool mute) {
        TogglePauseStream();
        agoraController.ToggleAudio(mute);
    }

    public void ToggleVideo(bool hideVideo) {
        TogglePauseStream();
        agoraController.ToggleVideo(hideVideo);
    }

    void TogglePauseStream() {
        if (toggleVideo.isOn && toggleAudio.isOn) {
            AnimatedCentreTextMessage("Stream Paused");
        } else {
            AnimatedFadeOutMessage();
        }
    }

    IEnumerator CountDown() {
        countDown = 0;

        while (countDown >= 0) {
            AnimatedCentreTextMessage(countDown > 0 ? countDown.ToString() : "ON AIR");
            AnimatedFadeOutMessage(.5f);
            countDown--;
            yield return new WaitForSeconds(1);
        }

        StartStream();
    }

    private void AnimatedCentreTextMessage(string message) {
        DOTween.Kill(tweenAnimationID);
        txtCentreMessage.rectTransform.localScale = Vector3.one;
        txtCentreMessage.text = message;
        txtCentreMessage.color = new Color(txtCentreMessage.color.r, txtCentreMessage.color.g, txtCentreMessage.color.b, 1);
        txtCentreMessage.rectTransform.DOPunchScale(Vector3.one, .25f, 3).SetId(tweenAnimationID);
    }

    private void AnimatedFadeOutMessage(float delay = 0) {
        txtCentreMessage.DOFade(0, .5f).SetDelay(delay).SetId(tweenAnimationID);
    }

    private void OnDisable() {
        StopAllCoroutines();
        ToggleARSessionObjects(true);
    }
}
