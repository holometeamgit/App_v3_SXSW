using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using agora_gaming_rtc;
using Beem.Firebase.DynamicLink;
using Beem.UI;

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
    private GameObject btnPushToTalk;

    [SerializeField]
    private Button btnGoLive;

    [SerializeField]
    private UIBtnLikes uiBtnLikes;

    [SerializeField]
    private StreamLikesRefresherView streamLikesRefresherView;

    [SerializeField]
    StreamerCountUpdater[] streamCountUpdaters;

    [SerializeField]
    private RectTransform CentreMessage;

    [SerializeField]
    private TextMeshProUGUI txtCentreMessage;

    [SerializeField]
    private CanvasGroup canvasGroup;

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
    bool isPushToTalkActive;

    VideoSurface videoSurface;
    string currentStreamId = string.Empty;
    string currentRoomId = string.Empty;

    private bool _muteAudio = false;
    private bool _hideVideo = false;

    const string MessageDisableAudio = "DisableAudio";
    const string MessageEnableAudio = "EnableAudio";
    const string MessageStreamerLeft = "StreamerLeft";
    const string MessageChannelCreatorUID = "StreamerUID:";
    const string MessageBroadcasterAudioPaused = "BroadcasterAudioPaused";
    const string MessageBroadcasterVideoPaused = "BroadcasterVideoPaused";
    const string MessageBroadcasterAudioAndVideoPaused = "BroadvasterAudioAndVideoPaused";
    const string MessageBroadcasterUnpaused = "BroadvasterUnpausedVideoAndAudio";

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
        agoraController.OnStreamWentOffline += StopStreamCountUpdaters;
        agoraController.OnStreamWentOffline += () => btnGoLive.interactable = true;
        agoraController.OnMessageRecieved += StreamMessageResponse;
        agoraController.OnUserViewerJoined += SendVideoAudioPauseStatusToViewers;
        agoraController.OnUserViewerJoined += SendPushToTalkStatusToViewers;
        agoraController.OnUserViewerJoined += SendChannelCreatorUIDToViewers;

        //cameraRenderImage.materialForRendering.SetFloat("_UseBlendTex", 0);

        StreamCallBacks.onLiveStreamCreated += RefreshStream;

        AddVideoSurface();
        initialised = true;
    }

    private void OnEnable() {
        FadePanel(true);
        txtCentreMessage.text = string.Empty;
        CentreMessage.localScale = Vector3.zero;
        RequestMicAccess();
        ChatBtn.onOpen += OpenChat;
    }

    private void RefreshStream(StreamStartResponseJsonData streamStartResponseJsonData) {
        currentStreamId = streamStartResponseJsonData.id.ToString();
        RefreshControls();
        uiBtnLikes.Init(streamStartResponseJsonData.id);
        streamLikesRefresherView.StartCountAsync(currentStreamId);
        StartStreamCountUpdaters();
    }

    private void RefreshRoom(string roomID) {
        currentRoomId = roomID;
        RefreshControls();
        StartStreamCountUpdaters();
    }

    private void RequestMicAccess() {
        if (!permissionGranter.MicAccessAvailable && !permissionGranter.MicRequestComplete) {
            permissionGranter.RequestMicAccess();
        }
    }

    public void RefreshControls() {
        RefreshStreamControls(agoraController.IsRoom);
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

    private void StartStreamCountUpdaters() {
        HelperFunctions.DevLog("Stream Count Updaters Started");
        foreach (StreamerCountUpdater streamerCountUpdater in streamCountUpdaters) {
            streamerCountUpdater.StartCheck(agoraController.ChannelName);
        }
    }

    private void StopStreamCountUpdaters() {
        HelperFunctions.DevLog("Stream Count Updaters Stopped");
        foreach (StreamerCountUpdater streamerCountUpdater in streamCountUpdaters) {
            streamerCountUpdater.StopCheck();
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

        btnGoLive.gameObject.SetActive(true);
        agoraController.IsChannelCreator = true;
        agoraController.ChannelName = userWebManager.GetUsername();

        isStreamer = true;
        gameObject.SetActive(true);
        pnlViewingExperience.ToggleARSessionObjects(false);
        cameraRenderImage.transform.parent.gameObject.SetActive(true);

        ToggleLocalAudio(false);
        ToggleVideo(false);
        isPushToTalkActive = false;

        StartCoroutine(OnPreviewReady());
        agoraController.StartPreview();
        RefreshControls();
    }

    public void OpenAsViewer(string channelName, string streamID, bool isRoom) {

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
        btnPushToTalk.SetActive(false);
        pnlViewingExperience.ActivateForStreaming(agoraController.ChannelName, streamID);
        cameraRenderImage.transform.parent.gameObject.SetActive(false);
        agoraController.JoinOrCreateChannel(false);
        currentStreamId = streamID;

        agoraController.IsRoom = isRoom;
        RefreshControls();

        long currentStreamIdLong = 0;
        long.TryParse(streamID, out currentStreamIdLong);
        uiBtnLikes.Init(currentStreamIdLong);
        streamLikesRefresherView.StartCountAsync(streamID);

        StartStreamCountUpdaters();
    }

    public void FadePanel(bool show) {
        canvasGroup.DOFade(show ? 1 : 0, 0.5f).OnComplete(() => { if (!show) { gameObject.SetActive(false); } });
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
            StopStream();
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

        HelperFunctions.DevLog("agoraController.IsChannelCreator = " + agoraController.IsChannelCreator);
        HelperFunctions.DevLog("agoraController.IsRoom = " + agoraController.IsRoom);

        HelperFunctions.DevLog("currentRoomId = " + currentRoomId);
        HelperFunctions.DevLog("currentStreamId = " + currentStreamId);

        if (agoraController.IsRoom) {
            if (agoraController.IsChannelCreator) {
                StreamCallBacks.onGetMyRoomLink?.Invoke();
            } else {
                if (!string.IsNullOrWhiteSpace(currentStreamId)) {
                    StreamCallBacks.onGetRoomLink?.Invoke(currentStreamId);
                } else {
                    DynamicLinksCallBacks.onShareAppLink?.Invoke();
                }
            }
        } else {
            if (!string.IsNullOrWhiteSpace(currentStreamId)) {
                StreamCallBacks.onGetStreamLink?.Invoke(currentStreamId);
            } else {
                DynamicLinksCallBacks.onShareAppLink?.Invoke();
            }
        }

        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);
    }

    public void StartCountdown() {
        countdownRoutine = StartCoroutine(CountDown());
    }

    public void StopCountdownRoutine() {
        if (countdownRoutine != null)
            StopCoroutine(countdownRoutine);
    }

    public void StopStream() {
        HelperFunctions.DevLog(nameof(StopStream) + " was called");

        if (agoraController.IsLive) { //Check needed as Stop Stream is being called when enabled by unity events causing this to start off disabled
            btnGoLive.gameObject.SetActive(false);

            if (isStreamer) //Send event to viewers to disconnect if streamer
                SendStreamLeaveStatusToViewers();
        }

        StopCountdownRoutine();
        streamLikesRefresherView.Cancel();

        agoraController.Leave();
        cameraRenderImage.texture = null;
        AnimatedFadeOutMessage();
        RefreshControls();
    }

    /// <summary>
    /// Call this to grant viewers ability to speak
    /// </summary>
    public void TogglePushToTalk() {
        isPushToTalkActive = !isPushToTalkActive;
        SendPushToTalkStatusToViewers();
        if (isPushToTalkActive) {
            AnimatedCentreTextMessage("Dialog is on. Listeners can talk to you now");
            AnimatedFadeOutMessage(3);
        }
    }

    private void SendVideoAudioPauseStatusToViewers() {

        if (!agoraController.IsLive)
            return;

        if (_hideVideo && _muteAudio) {
            agoraController.SendAgoraMessage(MessageBroadcasterAudioAndVideoPaused);
        } else if (_hideVideo) {
            agoraController.SendAgoraMessage(MessageBroadcasterVideoPaused);
        } else if (_muteAudio) {
            agoraController.SendAgoraMessage(MessageBroadcasterAudioPaused);
        } else {
            agoraController.SendAgoraMessage(MessageBroadcasterUnpaused);
        }
    }

    private void SendPushToTalkStatusToViewers() {
        agoraController.SendAgoraMessage(isPushToTalkActive ? MessageEnableAudio : MessageDisableAudio);
    }

    private void SendChannelCreatorUIDToViewers() {
        agoraController.SendAgoraMessage(MessageChannelCreatorUID + agoraController.ChannelCreatorUID);
    }

    private void SendStreamLeaveStatusToViewers() {
        agoraController.SendAgoraMessage(MessageStreamerLeft);
    }

    public void StreamMessageResponse(string message) {

        HelperFunctions.DevLog($"Stream Message Received ({message})");

        switch (message) {
            case MessageEnableAudio:
                ToggleLocalAudio(true);
                btnPushToTalk.SetActive(true);
                AnimatedCentreTextMessage("Hold the Speak button to talk to the broadcaster");
                AnimatedFadeOutMessage(3);
                return;
            case MessageDisableAudio:
                ToggleLocalAudio(true);
                btnPushToTalk.SetActive(false);
                return;
            case MessageStreamerLeft:
                agoraController.OnStreamerLeft?.Invoke();
                return;
            case MessageBroadcasterAudioPaused:
                AnimatedCentreTextMessage("Audio is muted by the broadcaster");
                agoraController.ToggleLiveStreamQuad(false);
                return;
            case MessageBroadcasterVideoPaused:
                AnimatedCentreTextMessage("Video has been paused by the broadcaster");
                agoraController.ToggleLiveStreamQuad(true);
                return;
            case MessageBroadcasterAudioAndVideoPaused:
                AnimatedCentreTextMessage("Video is paused and Audio is muted by the broadcaster");
                agoraController.ToggleLiveStreamQuad(true);
                return;
            case MessageBroadcasterUnpaused:
                AnimatedFadeOutMessage();
                agoraController.ToggleLiveStreamQuad(false);
                return;
        }

        if (message.Contains(MessageChannelCreatorUID)) {

            uint result;
            if (!uint.TryParse(message.Substring(MessageChannelCreatorUID.Length), out result)) {
                HelperFunctions.DevLogError("Channel creator UID parse failed");
            }
            agoraController.ChannelCreatorUID = result;
        }
    }

    void StartStream() {
        btnGoLive.gameObject.SetActive(false);
        agoraController.JoinOrCreateChannel(true);
        RefreshControls(); //Is this call actually needed?
    }

    /// <summary>
    /// Open Chat
    /// </summary>
    public void OpenChat(bool value) {
        chat.DoMenuTransition(value);
        //foreach (GameObject item in onlineControls) {
        //    item.SetActive(!value && agoraController.IsLive);
        //}
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

    public void ToggleLocalAudio(bool mute) {
        _muteAudio = mute;
        if (isStreamer) { //Display popup only for streamers but not for 2 way audio viewers
            UpdateToggleMessage();
        }
        agoraController.ToggleLocalAudio(mute);
    }

    public void ToggleVideo(bool hideVideo) {
        _hideVideo = hideVideo;
        UpdateToggleMessage();
        agoraController.ToggleVideo(hideVideo);
    }

    private void UpdateToggleMessage() {
        if (_hideVideo && _muteAudio) {
            AnimatedCentreTextMessage("Audio is muted and Video is paused");
        } else if (_hideVideo) {
            AnimatedCentreTextMessage("Video is paused");
        } else if (_muteAudio) {
            AnimatedCentreTextMessage("Audio is muted");
        } else {
            AnimatedFadeOutMessage();
        }
        SendVideoAudioPauseStatusToViewers();
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
        CentreMessage.localScale = Vector3.zero;
        txtCentreMessage.text = message;
        txtCentreMessage.color = new Color(txtCentreMessage.color.r, txtCentreMessage.color.g, txtCentreMessage.color.b, 1);
        CentreMessage.DOScale(Vector3.one, .1f).SetId(tweenAnimationID);
    }

    public void AnimatedFadeOutMessage(float delay = 0) {
        txtCentreMessage.DOFade(0, .5f).SetDelay(delay).SetId(tweenAnimationID);
        CentreMessage.DOScale(Vector3.zero, .1f).SetDelay(delay).SetId(tweenAnimationID);
    }

    private void OnDisable() {
        StopAllCoroutines();
        pnlViewingExperience.ToggleARSessionObjects(true);
        ChatBtn.onOpen -= OpenChat;
        StreamCallBacks.onLiveStreamCreated -= RefreshStream;
    }

    IEnumerator OnApplicationFocus(bool hasFocus) //Potential fix for bug where audio and video are re-enabled after losing focus from sharing or minimising
    {
        if (hasFocus) {
            yield return new WaitForEndOfFrame();

            //HelperFunctions.DevLog("ON FOCUS CALLED");

            if (_muteAudio)
                ToggleLocalAudio(true);
            if (_hideVideo)
                ToggleVideo(true);
        }
    }
}
