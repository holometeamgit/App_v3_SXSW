using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using agora_gaming_rtc;
using LostNative.Toolkit.FluidUI;
using Beem.Firebase.DynamicLink;
using System;

public class PnlStreamOverlay : MonoBehaviour {

    [Header("Views")]

    [SerializeField]
    private GameObject controlsPresenter;

    [SerializeField]
    private GameObject controlsViewer;

    [SerializeField]
    private GameObject[] publicStreamsControls;

    [SerializeField]
    private GameObject[] privateStreamsControls;

    [SerializeField]
    private GameObject[] onlineControls;

    [SerializeField]
    private GameObject[] offlineControls;

    [Header("These Views")]
    [SerializeField]
    private RawImage cameraRenderImage;

    [SerializeField]
    private TextMeshProUGUI txtCentreMessage;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private FluidToggle fluidToggle;

    [Header("Other Views")]
    [SerializeField]
    private PnlGenericError pnlGenericError;

    [SerializeField]
    private AnimatedTransition chat;

    [SerializeField]
    private PnlViewingExperience pnlViewingExperience;

    [Header("Controllers")]
    [SerializeField]
    private AgoraController agoraController;

    [SerializeField]
    private UserWebManager userWebManager;

    [SerializeField]
    private PermissionGranter permissionGranter;

    [SerializeField]
    private UnityEvent OnCloseAsViewer;

    [SerializeField]
    private UnityEvent OnCloseAsStreamer;

    bool initialised;
    int countDown;
    string tweenAnimationID = nameof(tweenAnimationID);
    Coroutine countdownRoutine;
    bool isStreamer;
    bool isUsingFrontCamera;
    VideoSurface videoSurface;
    string currentStreamId = "";

    private bool _muteAudio = false;
    private bool _hideVideo = false;

    //Vector3 rawImageQuadDefaultScale;

    void Init() {
        if (initialised)
            return;

        agoraController.OnStreamerLeft += CloseAsViewer;
        agoraController.OnCameraSwitched += () => {
            var videoSurface = cameraRenderImage.GetComponent<VideoSurface>();
            if (videoSurface) {
                isUsingFrontCamera = !isUsingFrontCamera;
            }
        };
        agoraController.OnPreviewStopped += () => videoSurface.SetEnable(false);
        agoraController.OnStreamWentLive += () => fluidToggle.ToggleInteractibility(true);
        agoraController.OnStreamWentOffline += () => fluidToggle.ToggleInteractibility(true);
        //cameraRenderImage.materialForRendering.SetFloat("_UseBlendTex", 0);

        AddVideoSurface();
        initialised = true;
    }

    private void OnEnable() {
        FadePanel(true);
        txtCentreMessage.text = string.Empty;
        RequestMicAccess();
        ChatBtn.onOpen += OpenChat;
    }

    private void RequestMicAccess() {
        if (!permissionGranter.MicAccessAvailable && !permissionGranter.MicRequestComplete) {
            permissionGranter.RequestMicAccess();
        }
    }

    public void RefreshControls() {
        RefreshStreamControls(!agoraController.IsChannelCreator || agoraController.IsRoom);
        RefreshBroadcasterControls(agoraController.IsChannelCreator);
        RefreshLiveControls(!agoraController.IsChannelCreator || agoraController.IsLive);
        HelperFunctions.DevLog("IsRoom = " + agoraController.IsRoom);
        HelperFunctions.DevLog("IsChannelCreator = " + agoraController.IsChannelCreator);
        HelperFunctions.DevLog("IsLive = " + agoraController.IsLive);

    }

    private void RefreshStreamControls(bool room) {
        foreach (GameObject item in privateStreamsControls) {
            item.SetActive(room);
        }
        foreach (GameObject item in publicStreamsControls) {
            item.SetActive(!room);
        }
    }

    private void RefreshBroadcasterControls(bool broadcaster) {
        controlsPresenter.SetActive(broadcaster);
        controlsViewer.SetActive(!broadcaster);
    }

    private void RefreshLiveControls(bool live) {
        foreach (GameObject item in onlineControls) {
            item.SetActive(live);
        }
        foreach (GameObject item in offlineControls) {
            item.SetActive(!live);
        }
    }

    public void OpenAsRoomBroadcaster() {
        Init();
        currentStreamId = "";
        agoraController.IsRoom = true;
        StreamerOpenSharedFunctions();
    }

    public void OpenAsStreamer() {
        Init();
        currentStreamId = "";
        agoraController.IsRoom = false;
        StreamerOpenSharedFunctions();
    }

    private void StreamerOpenSharedFunctions() {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);
        fluidToggle.ToggleInteractibility(true);
        agoraController.IsChannelCreator = true;
        agoraController.ChannelName = userWebManager.GetUsername();
        isStreamer = true;
        gameObject.SetActive(true);
        pnlViewingExperience.ToggleARSessionObjects(false);
        cameraRenderImage.transform.parent.gameObject.SetActive(true);
        RefreshControls();
        StartCoroutine(OnPreviewReady());
        agoraController.StartPreview();
    }

    public void OpenAsViewer(string channelName, string streamID) {

        if (channelName == userWebManager.GetUsername()) {
            pnlGenericError.ActivateSingleButton("Viewing as stream host",
                "Please connect to the stream using a different account",
                onBackPress: () => { CloseAsStreamer(); });

            return;
        }

        Init();
        agoraController.IsChannelCreator = false;
        agoraController.ChannelName = channelName;
        isStreamer = false;
        gameObject.SetActive(true);
        pnlViewingExperience.ActivateForStreaming(agoraController.ChannelName, streamID);
        cameraRenderImage.transform.parent.gameObject.SetActive(false);
        agoraController.JoinOrCreateChannel(false);
        currentStreamId = streamID;
        RefreshControls();
    }

    public void FadePanel(bool show) {
        canvasGroup.DOFade(show ? 1 : 0, 0.5f).OnComplete(() => { if (!show) { gameObject.SetActive(false); } });
    }

    private void OnDestroy() {
        LeaveOnDestroy();
    }

    private void LeaveOnDestroy() {
        if (isStreamer) {
            CloseAsStreamer();
        } else {
            CloseAsViewer();
        }
    }

    public void ShowLeaveWarning() {

        if (!agoraController.IsLive && isStreamer)
            CloseAsStreamer();
        else if (isStreamer)
            pnlGenericError.ActivateDoubleButton("End the live stream?",
                "Closing this page will end the live stream and disconnect your users.",
                onButtonOnePress: () => { CloseAsStreamer(); },
                onButtonTwoPress: () => pnlGenericError.gameObject.SetActive(false));
        else
            pnlGenericError.ActivateDoubleButton("Disconnect from live stream?",
                "Closing this page will disconnect you from the live stream",
                onButtonOnePress: () => { CloseAsViewer(); },
                onButtonTwoPress: () => pnlGenericError.gameObject.SetActive(false));
    }

    public void CloseAsStreamer() {
        OnCloseAsStreamer.Invoke();
        StopStream();
        agoraController.StopPreview();
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
    }

    private void CloseAsViewer() {
        OnCloseAsViewer.Invoke();
        StopStream();
    }

    public void ShareStream() {

        HelperFunctions.DevLog("isStreamer = " + isStreamer);
        HelperFunctions.DevLog("agoraController.IsRoom = " + agoraController.IsRoom);
        if (isStreamer && agoraController.IsRoom) {
            ShareRoomStreamLink();
            HelperFunctions.DevLog("SHARE_ROOM");
        } else {
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);
            ShareStreamLink();
            HelperFunctions.DevLog("SHARE_STREAM");
        }
    }

    public void StartCountdown() {
        countdownRoutine = StartCoroutine(CountDown());
    }

    public void StopStream() {
        HelperFunctions.DevLog(nameof(StopStream) + " was called");

        if (agoraController.IsLive) //Check needed as Stop Stream is being called when enabled by unity events causing this to start off disabled
            fluidToggle.ToggleInteractibility(false);

        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);

        agoraController.Leave();
        cameraRenderImage.texture = null;
        RefreshControls();
    }


    public void ShareRoomStreamLink() {
        StreamCallBacks.onGetMyRoomLink?.Invoke();
    }

    private void ShareStreamLink() {
        if (!string.IsNullOrWhiteSpace(currentStreamId))
            StreamCallBacks.onGetStreamLink?.Invoke(currentStreamId);
        else
            DynamicLinksCallBacks.onShareAppLink?.Invoke();
    }

    private void StartStream() {
        fluidToggle.ToggleInteractibility(false);
        agoraController.JoinOrCreateChannel(true);
        RefreshControls();
    }

    /// <summary>
    /// Open Chat
    /// </summary>
    public void OpenChat(bool value) {
        chat.DoMenuTransition(value);
        foreach (GameObject item in onlineControls) {
            item.SetActive(!value && agoraController.IsLive);
        }
    }

    private void AddVideoSurface() {
        videoSurface = cameraRenderImage.GetComponent<VideoSurface>();
        if (!videoSurface) {
            videoSurface = cameraRenderImage.gameObject.AddComponent<VideoSurface>();
            isUsingFrontCamera = true;
            videoSurface.EnableFilpTextureApply(false, true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(agoraController.frameRate);
        }
    }

    IEnumerator OnPreviewReady() {
        videoSurface.SetEnable(true);
        cameraRenderImage.color = Color.black;

        if (!agoraController.VideoIsReady || cameraRenderImage.texture == null)
            AnimatedCentreTextMessage("Loading Preview");

        while (!agoraController.VideoIsReady || cameraRenderImage.texture == null) {
            yield return null;
        }
        //yield return new WaitForSeconds(3);
        AnimatedFadeOutMessage();
        cameraRenderImage.color = Color.white;
        cameraRenderImage.SizeToParent();
    }

    public void ToggleAudio(bool mute) {
        _muteAudio = mute;
        TogglePauseStream();
        agoraController.ToggleAudio(mute);
    }

    public void ToggleVideo(bool hideVideo) {
        _hideVideo = hideVideo;
        TogglePauseStream();
        agoraController.ToggleVideo(hideVideo);
    }

    void TogglePauseStream() {
        if (_hideVideo && _muteAudio) {
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
            //yield return new WaitForSeconds(1);
            yield return new WaitForEndOfFrame();
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

    private void Awake() {
        StreamCallBacks.onLiveStreamCreated += (data) => { currentStreamId = data.id.ToString(); RefreshControls(); };
    }

    private void OnDisable() {
        StopAllCoroutines();
        pnlViewingExperience.ToggleARSessionObjects(true);
        ChatBtn.onOpen -= OpenChat;
    }

    IEnumerator OnApplicationFocus(bool hasFocus) //Potential fix for bug where audio and video are re-enabled after losing focus from sharing or minimising
    {
        if (hasFocus) {
            yield return new WaitForEndOfFrame();

            //HelperFunctions.DevLog("ON FOCUS CALLED");

            if (_muteAudio)
                ToggleAudio(true);
            if (_hideVideo)
                ToggleVideo(true);
        }
    }
}
