using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using agora_gaming_rtc;
using Beem.UI;

public class PnlStreamOverlay : AgoraMessageReceiver {

    [SerializeField]
    StreamUIWindow streamUIWindow;

    [SerializeField]
    private RawImage cameraRenderImage;

    [SerializeField]
    private GameObject imgBackground;

    [SerializeField]
    private StreamLikesRefresherView streamLikesRefresherView;

    [SerializeField]
    StreamNotificationPopupWindow streamNotificationPopupWindow;

    [SerializeField]
    StreamerCountUpdater[] streamCountUpdaters;

    [SerializeField]
    private CanvasGroup canvasGroup;

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
        _agoraController.OnStreamWentOffline += () => streamUIWindow.TogglePreLiveControls(true);
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
        ChatBtn.onOpen += OpenChat;
    }

    private void RefreshStream(StreamStartResponseJsonData streamStartResponseJsonData) {
        currentStreamId = streamStartResponseJsonData.id.ToString();
        RefreshControls();
        streamUIWindow.InitLikes(streamStartResponseJsonData.id);
        StartStreamCountUpdaters();
    }

    public void RefreshControls() {
        streamUIWindow.RefreshStreamControls(_agoraController.IsRoom);
        RefreshBroadcasterControls(_agoraController.IsChannelCreator);
        streamUIWindow.RefreshLiveControls(!_agoraController.IsChannelCreator || _agoraController.IsLive);
        RecordARConstructor.OnActivated?.Invoke(!_agoraController.IsChannelCreator && !_agoraController.IsRoom);
        HelperFunctions.DevLog($"IsRoom = {_agoraController.IsRoom}, IsChannelCreator = {_agoraController.IsChannelCreator}, IsLive = {_agoraController.IsLive}");
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
        streamUIWindow.RefreshBroadcasterControls(broadcaster);
        imgBackground.SetActive(broadcaster);
    }


    public void OpenAsRoomBroadcaster() {
        Init();
        currentStreamId = "";
        _agoraController.IsRoom = true;
        StreamerOpenSharedFunctions();
    }

    public void OpenAsStreamer() {
        if (!_userWebManager.CanGoLive()) {
            ShowPremiumRequiredMessage();
        }

        Init();
        currentStreamId = "";
        _agoraController.IsRoom = false;
        StreamerOpenSharedFunctions();
    }

    private void ShowPremiumRequiredMessage() {
        InfoPopupConstructor.onActivateAsMessage("PREMIUM FEATURE", "Please get in contact with us \n to explore Beeming live to \nthousands of people", new Color(142f / 255f, 196f / 255f, 246f / 255f));
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        gameObject.SetActive(false);
    }

    private void StreamerOpenSharedFunctions() {
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(true);

        streamUIWindow.TogglePreLiveControls(true);
        _agoraController.IsChannelCreator = true;
        _agoraController.ChannelName = _userWebManager.GetUsername();

        isChannelCreator = true;
        gameObject.SetActive(true);
        ARConstructor.onActivated?.Invoke(false);
        cameraRenderImage.transform.parent.gameObject.SetActive(true);

        _agoraController.ToggleLocalAudio(false);
        _agoraController.ToggleVideo(false);
        isPushToTalkActive = false;

        StartCoroutine(OnPreviewReady());
        _agoraController.StartPreview();
        RefreshControls();
        AnimatedFadeOutMessage();
    }

    public void OpenAsViewer(string channelName, string streamID, bool isRoom) {
        Init();
        _agoraController.ToggleLocalAudio(true);
        lastPauseStatusMessageReceived = string.Empty;
        lastPushToTalkStatusMessageReceived = string.Empty;
        _agoraController.IsChannelCreator = false;
        _agoraController.ChannelName = channelName;
        isChannelCreator = false;
        gameObject.SetActive(true);
        streamUIWindow.TogglePustoTalk.interactable = false;
        ARenaConstructor.onActivateForStreaming?.Invoke(channelName, streamID, isRoom);
        cameraRenderImage.transform.parent.gameObject.SetActive(false);
        _agoraController.JoinOrCreateChannel(false);
        currentStreamId = streamID;

        _agoraController.IsRoom = isRoom;
        RefreshControls();

        long currentStreamIdLong = 0;
        long.TryParse(streamID, out currentStreamIdLong);
        streamUIWindow.InitLikes(currentStreamIdLong);
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
            streamUIWindow.TogglePreLiveControls(false);

            if (isChannelCreator) //Send event to viewers to disconnect if streamer
                SendStreamLeaveStatusToViewers();
        }
        StopStatusUpdateRoutine();
        StopCountdownRoutine();
        streamLikesRefresherView.Cancel();

        _agoraController.Leave();
        cameraRenderImage.texture = null;
        streamNotificationPopupWindow.AnimatedFadeOutMessage();
        RefreshControls();
    }

    /// <summary>
    /// Call this to grant viewers ability to speak
    /// </summary>
    public void TogglePushToTalk(bool enabled) {
        isPushToTalkActive = enabled;
        SendPushToTalkStatusToViewers();
        if (isPushToTalkActive) {
            streamNotificationPopupWindow.AnimatedCentreTextMessage("Two way audio is on. \n Listeners can talk to you now.");
            streamNotificationPopupWindow.AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
        } else {
            streamNotificationPopupWindow.AnimatedCentreTextMessage("Two way audio is off.");
            streamNotificationPopupWindow.AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
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
                streamUIWindow.TogglePustoTalk.isOn = true; //Mute the mic
                streamUIWindow.TogglePustoTalk.interactable = true;
                streamNotificationPopupWindow.AnimatedCentreTextMessage("Tap the Talk button to enable \n your microphone");
                streamNotificationPopupWindow.AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
                return;
            case MessageToViewerDisableTwoWayAudio:
                if (LastMessageWasRecievedAlready(ref lastPushToTalkStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                streamUIWindow.TogglePustoTalk.isOn = true; //Mute the mic
                streamUIWindow.TogglePustoTalk.interactable = false;
                return;
            case MessageToViewerBroadcasterAudioPaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                streamNotificationPopupWindow.AnimatedCentreTextMessage("Audio has been turned off \n by the broadcaster");
                _agoraController.ToggleLiveStreamQuad(false);
                return;
            case MessageToViewerBroadcasterVideoPaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                streamNotificationPopupWindow.AnimatedCentreTextMessage("Video has been turned off \n by the broadcaster");
                _agoraController.ToggleLiveStreamQuad(true);
                return;
            case MessageToViewerBroadcasterAudioAndVideoPaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                streamNotificationPopupWindow.AnimatedCentreTextMessage("Video and Audio has been turned off \n by the broadcaster");
                _agoraController.ToggleLiveStreamQuad(true);
                return;
            case MessageToViewerBroadcasterUnpaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                streamNotificationPopupWindow.AnimatedFadeOutMessage();
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
            streamUIWindow.SpeechNotificationPopups.ActivatePopup(message.Replace(MessageToAllViewerIsSpeaking, "")); //Name is concatenated in string temporarily
        }

        if (message.Contains(MessageToAllViewerSpeakingStopped)) {
            streamUIWindow.SpeechNotificationPopups.DeactivatePopup(message.Replace(MessageToAllViewerSpeakingStopped, "")); //Name is concatenated in string temporarily
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
        streamUIWindow.SpeechNotificationPopups.ActivatePopup(_userWebManager.GetUsername());
        _agoraController.SendAgoraMessage(MessageToAllViewerIsSpeaking + _userWebManager.GetUsername());
    }

    /// <summary>
    /// Should be called when viewers lets go off push to talk button, not intended to be called when messages are received.
    /// </summary>
    private void DisableSpeakingMessage() {
        streamUIWindow.SpeechNotificationPopups.DeactivatePopup(_userWebManager.GetUsername());
        _agoraController.SendAgoraMessage(MessageToAllViewerSpeakingStopped + _userWebManager.GetUsername());
    }

    /// <summary>
    /// Starts the stream, use countdown coroutine to start with delay
    /// </summary>
    public void StartStream() {
        if (!_userWebManager.CanGoLive() && !_agoraController.IsRoom) {
            ShowPremiumRequiredMessage();
            return;
        }

        MenuConstructor.OnActivateCanvas(false);
        streamUIWindow.TogglePreLiveControls(false);
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

        while (!_agoraController.VideoIsReady || cameraRenderImage.texture == null) {
            yield return null;
        }

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
        } else if (streamUIWindow.TogglePustoTalk.interactable) { //Do not show microphone is off message unless push to talk is active for viewers
            ShowMicrophoneMuteStatusMessage(mute);
        }
        _agoraController.ToggleLocalAudio(mute);
    }

    private void ShowMicrophoneMuteStatusMessage(bool mute, bool autoHide = true) {
        streamNotificationPopupWindow.AnimatedCentreTextMessage("Your microphone is " + (mute ? "off" : "on") + ".");
        if (autoHide) {
            streamNotificationPopupWindow.AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
        }
    }

    public void ToggleVideo(bool hideVideo) { //Only called for stream host
        _hideVideo = hideVideo;

        streamNotificationPopupWindow.AnimatedCentreTextMessage("Your camera is " + (hideVideo ? "off" : "on") + ".");

        if (!_agoraController.IsLive) { //Don't hide if not live and camera is being disabled
            if (!hideVideo) {
                streamNotificationPopupWindow.AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
            }
        } else {
            streamNotificationPopupWindow.AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
        }

        SendVideoAudioPauseStatusToViewers();

        _agoraController.ToggleVideo(hideVideo);
    }

    IEnumerator CountDown() {
        countDown = 0;

        while (countDown >= 0) {
            streamNotificationPopupWindow.AnimatedCentreTextMessage(countDown > 0 ? countDown.ToString() : "ON AIR");
            streamNotificationPopupWindow.AnimatedFadeOutMessage(.5f);
            countDown--;
            //yield return new WaitForSeconds(1);
            yield return new WaitForEndOfFrame();
        }

        StartStream();
    }



    private void OnDisable() {
        StopAllCoroutines();
        streamUIWindow.SpeechNotificationPopups.DeactivateAllPopups();
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
