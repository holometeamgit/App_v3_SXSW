using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using agora_gaming_rtc;
using Beem.UI;
using Beem.Permissions;

public class PnlStreamOverlay : AgoraMessageReceiver {

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

    [Tooltip("Controls which interactable bool will be set to true when live")]
    [SerializeField]
    private Selectable[] onlineInteractableControlToggle;

    [Header("These Views")]

    [SerializeField]
    private RawImage cameraRenderImage;

    [SerializeField]
    private GameObject imgBackground;

    [SerializeField]
    private Toggle togglePushToTalk;

    [SerializeField]
    private GameObject[] gameObjectsToDisableWhileGoingLive;

    [SerializeField]
    private UIBtnLikes uiBtnLikes;

    [SerializeReference]
    private UITextLabelLikes uiViewersTextLabelLikes;

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

    [SerializeField]
    private SpeechNotificationPopups speechNotificationPopups;

    [Header("Other Views")]

    [SerializeField]
    private AnimatedTransition chat;

    [Space]
    [SerializeField]
    private AgoraController _agoraController;
    [SerializeField]
    private UserWebManager _userWebManager;

    private bool initialised;
    private int countDown;
    private string tweenAnimationID = nameof(tweenAnimationID);
    private Coroutine countdownRoutine;
    private bool isChannelCreator;
    private bool isUsingFrontCamera;
    private bool isPushToTalkActive;

    VideoSurface videoSurface;
    string currentStreamId = string.Empty;

    //private Coroutine delayToggleAudioOffRoutine;  //TODO keeping here in case we revert to push to talk
    //private const int PUSH_TO_TALK_MUTE_DELAY = 1; //TODO keeping here in case we revert to push to talk

    private Coroutine statusUpdateRoutine;

    private bool _muteAudio = false;
    private bool _hideVideo = false;

    private string lastPauseStatusMessageReceived; //Intended for viewers use only it's record state of streamers pause situation and to prevent double calls
    private string lastPushToTalkStatusMessageReceived; //To stop audio toggling twice

    const int STATUS_MESSAGE_HIDE_DELAY = 3;

    private const char MessageSplitter = '+';
    private const string ToViewerTag = "ToViewer"; //Indicates message is for viewers only
    private const string MessageToViewerDisableTwoWayAudio = ToViewerTag + "DisableTwoWayAudio";
    private const string MessageToViewerEnableTwoWayAudio = ToViewerTag + "EnableTwoWayAudio";
    private const string MessageToViewerStreamerLeft = ToViewerTag + "StreamerLeft";
    private const string MessageToViewerChannelCreatorUID = ToViewerTag + "StreamerUID:";
    private const string MessageToViewerBroadcasterAudioPaused = ToViewerTag + "BroadcasterAudioPaused";
    private const string MessageToViewerBroadcasterVideoPaused = ToViewerTag + "BroadcasterVideoPaused";
    private const string MessageToViewerBroadcasterAudioAndVideoPaused = ToViewerTag + "BroadcasterAudioAndVideoPaused";
    private const string MessageToViewerBroadcasterUnpaused = ToViewerTag + "BroadcasterUnpausedVideoAndAudio";
    private const string MessageToAllViewerIsSpeaking = "ViewerSpeakingStarted";
    private const string MessageToAllViewerSpeakingStopped = "ViewerSpeakingStopped";

    void Init() {
        if (initialised)
            return;

        _agoraController.OnStreamerLeft += StreamFinished;
        _agoraController.OnCameraSwitched += () => {
            var videoSurface = cameraRenderImage.GetComponent<VideoSurface>();
            if (videoSurface) {
                isUsingFrontCamera = !isUsingFrontCamera;
            }
        };
        _agoraController.OnPreviewStopped += PreviewStopped;
        _agoraController.OnStreamWentOffline += StopStreamCountUpdaters;
        _agoraController.OnStreamWentOffline += () => TogglePreLiveControls(true);
        _agoraController.OnStreamWentLive += StartStatusUpdateRoutine;
        _agoraController.OnUserViewerJoined += SendVideoAudioPauseStatusToViewers;
        _agoraController.OnUserViewerJoined += SendPushToTalkStatusToViewers;
        _agoraController.OnUserViewerJoined += SendChannelCreatorUIDToViewers;
        _agoraController.OnSpeechDetected += SendViewerIsSpeakingMessage;
        _agoraController.OnNoSpeechDetected += DisableSpeakingMessage;

        _agoraController.AddAgoraMessageReceiver(this);
        //cameraRenderImage.materialForRendering.SetFloat("_UseBlendTex", 0);

        AssignStreamCountUpdaterAnalyticsEvent();

        StreamCallBacks.onLiveStreamCreated += RefreshStream;

        AddVideoSurface();
        initialised = true;
    }

    /// <summary>
    /// Show the info popup for stadium
    /// </summary>
    public void ShowInfoPopupStadium() {
        InfoPopupConstructor.onActivate("HOW TO BROADCAST \n IN STADIUM", true, new Color(142f / 255f, 196f / 255f, 246f / 255f));
    }

    /// <summary>
    /// Show the info popup for room
    /// </summary>
    public void ShowInfoPopupRoom() {
        InfoPopupConstructor.onActivate("HOW TO USE \n MY ROOM", true, new Color(131f / 255f, 168f / 255f, 240f / 255f));
    }

    private void OnEnable() {
        txtCentreMessage.text = string.Empty;
        CentreMessage.localScale = Vector3.zero;
        ChatBtn.onOpen += OpenChat;
    }

    private void RefreshStream(StreamStartResponseJsonData streamStartResponseJsonData) {
        currentStreamId = streamStartResponseJsonData.id.ToString();
        RefreshControls();
        uiBtnLikes.Init(streamStartResponseJsonData.id);
        uiViewersTextLabelLikes.Init(streamStartResponseJsonData.id);
        StartStreamCountUpdaters();
    }

    public void RefreshControls() {
        RefreshStreamControls(_agoraController.IsRoom);
        RefreshBroadcasterControls(_agoraController.IsChannelCreator);
        RefreshLiveControls(!_agoraController.IsChannelCreator || _agoraController.IsLive);
        RecordARConstructor.OnActivated?.Invoke(!_agoraController.IsChannelCreator && !_agoraController.IsRoom);
        HelperFunctions.DevLog($"IsRoom = {_agoraController.IsRoom}, IsChannelCreator = {_agoraController.IsChannelCreator}, IsLive = {_agoraController.IsLive}");
    }

    private void RefreshStreamControls(bool room) {
        foreach (GameObject item in privateStreamsControls) {
            item.SetActive(room);
        }
        foreach (GameObject item in publicStreamsControls) {
            item.SetActive(!room);
        }
    }

    private void TogglePreLiveControls(bool enable) {
        foreach (GameObject objectToToggle in gameObjectsToDisableWhileGoingLive) {
            objectToToggle.SetActive(enable);
        }
    }

    private void AssignStreamCountUpdaterAnalyticsEvent() {
        foreach (StreamerCountUpdater streamerCountUpdater in streamCountUpdaters) {
            streamerCountUpdater.OnCountUpdated += _agoraController.SendViewerCountAnalyticsUpdate;
        }
    }

    private void StartStreamCountUpdaters() {
        HelperFunctions.DevLog("Stream Count Updaters Started");
        foreach (StreamerCountUpdater streamerCountUpdater in streamCountUpdaters) {
            streamerCountUpdater.StartCheck(_agoraController.ChannelName, _agoraController.IsRoom);
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
        imgBackground.SetActive(broadcaster);
        controlsViewer.SetActive(!broadcaster);
    }

    private void RefreshLiveControls(bool live) {

        foreach (GameObject item in onlineControls) {
            item.SetActive(live);
        }
        foreach (Selectable selectable in onlineInteractableControlToggle) {
            selectable.interactable = live;
        }
        foreach (GameObject item in offlineControls) {
            item.SetActive(!live);
        }
    }

    public void OpenAsRoomBroadcaster() {
        Init();
        currentStreamId = "";
        _agoraController.IsRoom = true;
        StreamerOpenSharedFunctions();
    }

    public void OpenAsStreamer() {
        Init();
        currentStreamId = "";
        _agoraController.IsRoom = false;
        StreamerOpenSharedFunctions();
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }

    private void StreamerOpenSharedFunctions() {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);

        TogglePreLiveControls(true);
        _agoraController.IsChannelCreator = true;
        _agoraController.ChannelName = _userWebManager.GetUsername();

        isChannelCreator = true;
        gameObject.SetActive(true);
        ARConstructor.onActivated?.Invoke(false);
        cameraRenderImage.transform.parent.gameObject.SetActive(true);

        ToggleLocalAudio(false);
        ToggleVideo(false);
        isPushToTalkActive = false;

        StartCoroutine(OnPreviewReady());
        _agoraController.StartPreview();
        RefreshControls();
    }

    public void OpenAsViewer(string channelName, string streamID, bool isRoom) {
        Init();
        ToggleLocalAudio(true);
        lastPauseStatusMessageReceived = string.Empty;
        lastPushToTalkStatusMessageReceived = string.Empty;
        _agoraController.IsChannelCreator = false;
        _agoraController.ChannelName = channelName;
        isChannelCreator = false;
        gameObject.SetActive(true);
        togglePushToTalk.interactable = false;
        ARenaConstructor.onActivateForStreaming?.Invoke(channelName, streamID, isRoom);
        cameraRenderImage.transform.parent.gameObject.SetActive(false);
        _agoraController.JoinOrCreateChannel(false);
        currentStreamId = streamID;

        _agoraController.IsRoom = isRoom;
        RefreshControls();

        long currentStreamIdLong = 0;
        long.TryParse(streamID, out currentStreamIdLong);
        uiBtnLikes.Init(currentStreamIdLong);
        uiViewersTextLabelLikes.Init(currentStreamIdLong);
        streamLikesRefresherView.StartCountAsync(streamID);
        StartStreamCountUpdaters();
    }

    private void LeaveOnDestroy() {
        if (isChannelCreator) {
            CloseAsStreamer();
        } else {
            CloseAsViewer();
        }
    }

    public void ShowLeaveWarning() {

        if (!_agoraController.IsLive && isChannelCreator)
            StopStream();
        else if (isChannelCreator)
            WarningConstructor.ActivateDoubleButton("End the live stream?",
                "Closing this page will end the live stream and disconnect your users.",
                onButtonOnePress: () => { DeactivateLive(); });
        else
            WarningConstructor.ActivateDoubleButton("Disconnect from live stream?",
                "Closing this page will disconnect you from the live stream",
                onButtonOnePress: () => { CloseAsViewer(); });
    }

    private void DeactivateLive() {
        StopStream();
        MenuConstructor.OnActivated?.Invoke(true);
    }

    public void CloseAsStreamer() {
        StopStream();
        _agoraController.StopPreview();
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
        StreamOverlayConstructor.onDeactivate?.Invoke();
        MenuConstructor.OnActivated?.Invoke(true);
        RecordARConstructor.OnActivated?.Invoke(false);
    }

    private void StreamFinished() {
        CloseAsViewer();
        if (_agoraController.IsRoom) {
            StreamCallBacks.onRoomBroadcastFinished?.Invoke();
        }
    }

    private void CloseAsViewer() {
        StopStream();
        StreamOverlayConstructor.onDeactivate?.Invoke();
        MenuConstructor.OnActivated?.Invoke(true);
        RecordARConstructor.OnActivated?.Invoke(false);
    }

    private void PreviewStopped() {
        videoSurface.SetEnable(false);
    }

    public void ShareStream() {

        HelperFunctions.DevLog($"IsRoom = {_agoraController.IsRoom}, IsChannelCreator = {_agoraController.IsChannelCreator}, agoraController.ChannelName = {_agoraController.ChannelName}, currentStreamId = {currentStreamId}");

        if (_agoraController.IsRoom) {
            StreamCallBacks.onShareRoomLink?.Invoke(_agoraController.ChannelName);
        } else {
            StreamCallBacks.onShareStreamLinkByUsername?.Invoke(_agoraController.ChannelName);
        }
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

        if (_agoraController.IsLive) { //Check needed as Stop Stream is being called when enabled by unity events causing this to start off disabled
            TogglePreLiveControls(false);

            if (isChannelCreator) //Send event to viewers to disconnect if streamer
                SendStreamLeaveStatusToViewers();
        }
        StopStatusUpdateRoutine();
        StopCountdownRoutine();
        streamLikesRefresherView.Cancel();

        _agoraController.Leave();
        cameraRenderImage.texture = null;
        AnimatedFadeOutMessage();
        RefreshControls();
    }

    /// <summary>
    /// Call this to grant viewers ability to speak
    /// </summary>
    public void TogglePushToTalk(bool enabled) {
        isPushToTalkActive = enabled;
        SendPushToTalkStatusToViewers();
        if (isPushToTalkActive) {
            AnimatedCentreTextMessage("Two way audio is on. \n Listeners can talk to you now.");
            AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
        } else {
            AnimatedCentreTextMessage("Two way audio is off.");
            AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
        }
    }

    private void StartStatusUpdateRoutine() {
        if (!isChannelCreator)
            return;

        statusUpdateRoutine = StartCoroutine(SendStatusUpdateToViewers());
    }

    private void StopStatusUpdateRoutine() {
        if (statusUpdateRoutine != null) {
            StopCoroutine(statusUpdateRoutine);
        }
    }

    private IEnumerator SendStatusUpdateToViewers() {
        while (true) {
            HelperFunctions.DevLog("PollingStreamStatus sending status update to viewers");

            if (!isChannelCreator) {
                HelperFunctions.DevLogError("Tried to send stream status update as viewer");
                yield break;
            }

            if (!_agoraController.IsLive) {
                HelperFunctions.DevLogError("Tried to send stream status update before live");
                yield break;
            }

            yield return new WaitForSeconds(5);
            _agoraController.SendAgoraMessage(CreateMultiMessage(GetPushToTalkStatusMessage(), GetChannelCreatorUIDMessage(), GetVideoAudioOnOffStatusMessage()));
        }
    }

    private string CreateMultiMessage(params string[] messages) {
        string output = string.Empty;
        for (int i = 0; i < messages.Length; i++) {
            output = output + messages[i];

            if (i < (messages.Length - 1)) {
                output = output + MessageSplitter;
            }
        }
        return output;
    }

    private void SendVideoAudioPauseStatusToViewers() {

        if (!_agoraController.IsLive)
            return;

        _agoraController.SendAgoraMessage(GetVideoAudioOnOffStatusMessage());
    }

    private string GetVideoAudioOnOffStatusMessage() {

        if (_hideVideo && _muteAudio) {
            return MessageToViewerBroadcasterAudioAndVideoPaused;
        } else if (_hideVideo) {
            return MessageToViewerBroadcasterVideoPaused;
        } else if (_muteAudio) {
            return MessageToViewerBroadcasterAudioPaused;
        } else {
            return MessageToViewerBroadcasterUnpaused;
        }
    }

    private void SendPushToTalkStatusToViewers() {
        _agoraController.SendAgoraMessage(GetPushToTalkStatusMessage());
    }

    private string GetPushToTalkStatusMessage() {
        return isPushToTalkActive ? MessageToViewerEnableTwoWayAudio : MessageToViewerDisableTwoWayAudio;
    }

    private void SendChannelCreatorUIDToViewers() {
        _agoraController.SendAgoraMessage(GetChannelCreatorUIDMessage());
    }

    private string GetChannelCreatorUIDMessage() {
        return MessageToViewerChannelCreatorUID + _agoraController.ChannelCreatorUID;
    }

    private void SendStreamLeaveStatusToViewers() {
        _agoraController.SendAgoraMessage(MessageToViewerStreamerLeft);
    }

    private bool CheckIncorrectMessageForStreamer(string message) {
        if (isChannelCreator && message.Contains(ToViewerTag)) {
            HelperFunctions.DevLogError($"Streamer received a message intended for viewers {message}");
            return true;
        }
        return false;
    }

    public override void ReceivedChatMessage(string data) {
        AgoraStreamMessageCommonType agoraStreamMessage = JsonParser.CreateFromJSON<AgoraStreamMessageCommonType>(data);
        if (agoraStreamMessage.requestID == AgoraMessageRequestIDs.IDStreamMessage) {
            var chatMessageJsonData = JsonParser.CreateFromJSON<ChatMessageJsonData>(data);

            string[] messages = chatMessageJsonData.message.Split(new char[] { MessageSplitter }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string parsedMessage in messages) {
                HandleReturnedMessage(parsedMessage);
            }
        }
    }

    public override void OnDisconnected() {
    }

    private void HandleReturnedMessage(string message) {
        if (CheckIncorrectMessageForStreamer(message)) {
            return;
        }

        switch (message) {
            case MessageToViewerEnableTwoWayAudio:
                if (LastMessageWasRecievedAlready(ref lastPushToTalkStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                togglePushToTalk.isOn = true; //Mute the mic
                togglePushToTalk.interactable = true;
                AnimatedCentreTextMessage("Tap the Talk button to enable \n your microphone");
                AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
                return;
            case MessageToViewerDisableTwoWayAudio:
                if (LastMessageWasRecievedAlready(ref lastPushToTalkStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                togglePushToTalk.isOn = true; //Mute the mic
                togglePushToTalk.interactable = false;
                return;
            case MessageToViewerBroadcasterAudioPaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                AnimatedCentreTextMessage("Audio has been turned off \n by the broadcaster");
                _agoraController.ToggleLiveStreamQuad(false);
                return;
            case MessageToViewerBroadcasterVideoPaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                AnimatedCentreTextMessage("Video has been turned off \n by the broadcaster");
                _agoraController.ToggleLiveStreamQuad(true);
                return;
            case MessageToViewerBroadcasterAudioAndVideoPaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                AnimatedCentreTextMessage("Video and Audio has been turned off \n by the broadcaster");
                _agoraController.ToggleLiveStreamQuad(true);
                return;
            case MessageToViewerBroadcasterUnpaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                AnimatedFadeOutMessage();
                _agoraController.ToggleLiveStreamQuad(false);
                return;
            case MessageToViewerStreamerLeft:
                _agoraController.OnStreamerLeft?.Invoke();
                return;
        }

        if (message.Contains(MessageToViewerChannelCreatorUID)) {

            uint result;
            if (!uint.TryParse(message.Substring(MessageToViewerChannelCreatorUID.Length), out result)) {
                HelperFunctions.DevLogError("Channel creator UID parse failed");
            }
            _agoraController.ChannelCreatorUID = result;
            _agoraController.ActivateViewerVideoSufaceFeatures();
        }

        if (message.Contains(MessageToAllViewerIsSpeaking)) {
            speechNotificationPopups.ActivatePopup(message.Replace(MessageToAllViewerIsSpeaking, "")); //Name is concatenated in string temporarily
        }

        if (message.Contains(MessageToAllViewerSpeakingStopped)) {
            speechNotificationPopups.DeactivatePopup(message.Replace(MessageToAllViewerSpeakingStopped, "")); //Name is concatenated in string temporarily
        }
    }

    private bool LastMessageWasRecievedAlready(ref string messageToCheck, string messageReceived) {
        if (messageToCheck == messageReceived) {
            return true;
        }
        messageToCheck = messageReceived;
        return false;
    }

    /// <summary>
    /// Should be called when viewers hold push to talk button, not intended to be called when messages are received.
    /// </summary>
    private void SendViewerIsSpeakingMessage() {
        speechNotificationPopups.ActivatePopup(_userWebManager.GetUsername());
        _agoraController.SendAgoraMessage(MessageToAllViewerIsSpeaking + _userWebManager.GetUsername());
    }

    /// <summary>
    /// Should be called when viewers lets go off push to talk button, not intended to be called when messages are received.
    /// </summary>
    private void DisableSpeakingMessage() {
        speechNotificationPopups.DeactivatePopup(_userWebManager.GetUsername());
        _agoraController.SendAgoraMessage(MessageToAllViewerSpeakingStopped + _userWebManager.GetUsername());
    }

    /// <summary>
    /// Starts the stream, use countdown coroutine to start with delay
    /// </summary>
    public void StartStream() {
        MenuConstructor.OnActivateCanvas(false);
        TogglePreLiveControls(false);
        _agoraController.JoinOrCreateChannel(true);
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
            videoSurface.EnableFilpTextureApply(true, true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(_agoraController.frameRate);
        }
    }

    IEnumerator OnPreviewReady() {
        videoSurface.SetEnable(true);
        cameraRenderImage.color = Color.black;

        if (!_agoraController.VideoIsReady || cameraRenderImage.texture == null)
            AnimatedCentreTextMessage("Loading Preview");

        while (!_agoraController.VideoIsReady || cameraRenderImage.texture == null) {
            yield return null;
        }
        //yield return new WaitForSeconds(3);
        AnimatedFadeOutMessage();
        cameraRenderImage.color = Color.white;
        cameraRenderImage.SizeToParent();
    }

    //TODO keeping here in case we revert to push to talk
    ///// <summary>
    ///// This toggle audio off with a delay, intended for push to talk users once letting go of button
    ///// </summary>
    //public void ToggleOffLocalAudioPushToTalkWithDelay() {
    //    delayToggleAudioOffRoutine = StartCoroutine(DelayToggleAudioOff());
    //}

    //TODO keeping here in case we revert to push to talk
    //private IEnumerator DelayToggleAudioOff() {
    //    yield return new WaitForSeconds(PUSH_TO_TALK_MUTE_DELAY);
    //    ToggleLocalAudio(true);
    //}

    //TODO keeping here in case we revert to push to talk
    //public void ToggleLocalAudio(bool mute) {

    //    if (delayToggleAudioOffRoutine != null) {
    //        StopCoroutine(delayToggleAudioOffRoutine);
    //    }

    //    _muteAudio = mute;
    //    if (isChannelCreator) { //Display popup only for streamers but not for 2 way audio viewers
    //        UpdateToggleMessage();
    //    }
    //    agoraController.ToggleLocalAudio(mute);
    //}

    public void ToggleLocalAudio(bool mute) {
        _muteAudio = mute;

        if (isChannelCreator) { //Display popup only for streamers but not for 2 way audio viewers
            bool autoHide = true;
            if (!_agoraController.IsLive) {
                if (mute) {
                    autoHide = false; //Keep mute messages opened if the channel not live
                }
            }
            ShowMicrophoneMuteStatusMessage(mute, autoHide); //Don't hide if not live
            SendVideoAudioPauseStatusToViewers();
        } else if (togglePushToTalk.interactable) { //Do not show microphone is off message unless push to talk is active for viewers
            ShowMicrophoneMuteStatusMessage(mute);
        }
        _agoraController.ToggleLocalAudio(mute);
    }

    private void ShowMicrophoneMuteStatusMessage(bool mute, bool autoHide = true) {
        AnimatedCentreTextMessage("Your microphone is " + (mute ? "off" : "on") + ".");
        if (autoHide) {
            AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
        }
    }

    public void ToggleVideo(bool hideVideo) { //Only called for stream host
        _hideVideo = hideVideo;

        AnimatedCentreTextMessage("Your camera is " + (hideVideo ? "off" : "on") + ".");

        if (!_agoraController.IsLive) { //Don't hide if not live and camera is being disabled
            if (!hideVideo) {
                AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
            }
        } else {
            AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
        }

        SendVideoAudioPauseStatusToViewers();

        //UpdateToggleMessageOff();
        _agoraController.ToggleVideo(hideVideo);
    }

    //private void UpdateToggleMessageOff() {
    //    if (_hideVideo && _muteAudio) {
    //        AnimatedCentreTextMessage("Your camera is Off. \n Your microphone is Off.");
    //    } else if (_hideVideo) {
    //        AnimatedCentreTextMessage("Your camera is Off.");
    //    } else if (_muteAudio) {
    //        AnimatedCentreTextMessage("Your microphone is Off.");
    //    } else {
    //        AnimatedFadeOutMessage();
    //    }
    //    SendVideoAudioPauseStatusToViewers();
    //}

    //private void UpdateToggleMessageOff()
    //{
    //    if (_hideVideo)        {
    //        AnimatedCentreTextMessage("Your camera is On.");
    //        AnimatedFadeOutMessage(3);
    //    } else if (_muteAudio)        {
    //        AnimatedCentreTextMessage("Your microphone is On.");
    //        AnimatedFadeOutMessage(3);
    //    } else {
    //        AnimatedFadeOutMessage();
    //    }
    //    SendVideoAudioPauseStatusToViewers();
    //}

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
        speechNotificationPopups.DeactivateAllPopups();
        ChatBtn.onOpen -= OpenChat;
    }

    private void OnDestroy() {
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
