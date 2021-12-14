﻿using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using agora_gaming_rtc;
using Beem.Firebase.DynamicLink;
using Beem.UI;

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

    [SerializeField]
    private PnlViewingExperience pnlViewingExperience;

    [Header("Controllers")]
    [SerializeField]
    private AgoraController agoraController;

    [SerializeField]
    private UserWebManager userWebManager;

    [SerializeField]
    private UnityEvent OnCloseAsViewer;

    [SerializeField]
    private UnityEvent OnCloseAsStreamer;

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

        agoraController.OnStreamerLeft += CloseAsViewer;
        agoraController.OnStreamerLeft += CloseRoomAsViewerWhenStreamWasStopped;
        agoraController.OnCameraSwitched += () => {
            var videoSurface = cameraRenderImage.GetComponent<VideoSurface>();
            if (videoSurface) {
                isUsingFrontCamera = !isUsingFrontCamera;
            }
        };
        agoraController.OnPreviewStopped += () => videoSurface.SetEnable(false);
        agoraController.OnStreamWentOffline += StopStreamCountUpdaters;
        agoraController.OnStreamWentOffline += () => TogglePreLiveControls(true);
        agoraController.OnStreamWentLive += StartStatusUpdateRoutine;
        agoraController.OnUserViewerJoined += SendVideoAudioPauseStatusToViewers;
        agoraController.OnUserViewerJoined += SendPushToTalkStatusToViewers;
        agoraController.OnUserViewerJoined += SendChannelCreatorUIDToViewers;
        agoraController.OnSpeechDetected += SendViewerIsSpeakingMessage;
        agoraController.OnNoSpeechDetected += DisableSpeakingMessage;

        agoraController.AddAgoraMessageReceiver(this);
        //cameraRenderImage.materialForRendering.SetFloat("_UseBlendTex", 0);

        AssignStreamCountUpdaterAnalyticsEvent();

        StreamCallBacks.onLiveStreamCreated += RefreshStream;

        AddVideoSurface();
        initialised = true;
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
        RefreshStreamControls(agoraController.IsRoom);
        RefreshBroadcasterControls(agoraController.IsChannelCreator);
        RefreshLiveControls(!agoraController.IsChannelCreator || agoraController.IsLive);
        HelperFunctions.DevLog($"IsRoom = {agoraController.IsRoom}, IsChannelCreator = {agoraController.IsChannelCreator}, IsLive = {agoraController.IsLive}");
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
            streamerCountUpdater.OnCountUpdated += agoraController.SendViewerCountAnalyticsUpdate;
        }
    }

    private void StartStreamCountUpdaters() {
        HelperFunctions.DevLog("Stream Count Updaters Started");
        foreach (StreamerCountUpdater streamerCountUpdater in streamCountUpdaters) {
            streamerCountUpdater.StartCheck(agoraController.ChannelName, agoraController.IsRoom);
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

        TogglePreLiveControls(true);
        agoraController.IsChannelCreator = true;
        agoraController.ChannelName = userWebManager.GetUsername();

        isChannelCreator = true;
        gameObject.SetActive(true);
        ARConstructor.onActivated?.Invoke(false);
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
            WarningConstructor.ActivateSingleButton("Viewing as stream host",
                "Please connect to the stream using a different account",
                onBackPress: () => { StreamOverlayConstructor.onActivatedAsLiveBroadcaster?.Invoke(false); });

            return;
        }

        Init();
        ToggleLocalAudio(true);
        lastPauseStatusMessageReceived = string.Empty;
        lastPushToTalkStatusMessageReceived = string.Empty;
        agoraController.IsChannelCreator = false;
        agoraController.ChannelName = channelName;
        isChannelCreator = false;
        gameObject.SetActive(true);
        togglePushToTalk.interactable = false;
        pnlViewingExperience.ActivateForStreaming(agoraController.ChannelName, streamID, isRoom);
        cameraRenderImage.transform.parent.gameObject.SetActive(false);
        agoraController.JoinOrCreateChannel(false);
        currentStreamId = streamID;

        agoraController.IsRoom = isRoom;
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

        if (!agoraController.IsLive && isChannelCreator)
            StopStream();
        else if (isChannelCreator)
            WarningConstructor.ActivateDoubleButton("End the live stream?",
                "Closing this page will end the live stream and disconnect your users.",
                onButtonOnePress: () => { StreamOverlayConstructor.onActivatedAsLiveBroadcaster?.Invoke(false); },
                onButtonTwoPress: () => WarningConstructor.Deactivate());
        else
            WarningConstructor.ActivateDoubleButton("Disconnect from live stream?",
                "Closing this page will disconnect you from the live stream",
                onButtonOnePress: () => { CloseAsViewer(); },
                onButtonTwoPress: () => WarningConstructor.Deactivate());
    }

    public void CloseAsStreamer() {
        StopStream();
        agoraController.StopPreview();
        ApplicationSettingsHandler.Instance.ToggleSleepTimeout(false);
        OnCloseAsStreamer.Invoke();
        HomeScreenConstructor.OnActivated?.Invoke(true);
        MenuConstructor.OnActivated?.Invoke(true);
    }

    private void CloseAsViewer() {
        StopStream();
        OnCloseAsViewer.Invoke();
        HomeScreenConstructor.OnActivated?.Invoke(true);
        MenuConstructor.OnActivated?.Invoke(true);
    }

    private void CloseRoomAsViewerWhenStreamWasStopped() {
        if (agoraController.IsRoom) {
            StreamCallBacks.onRoomClosed?.Invoke();
        }
    }

    public void ShareStream() {
        HelperFunctions.DevLog($"IsRoom = {agoraController.IsRoom}, IsChannelCreator = {agoraController.IsChannelCreator}, agoraController.ChannelName = {agoraController.ChannelName}, currentStreamId = {currentStreamId}");

        if (agoraController.IsRoom) {
            StreamCallBacks.onShareRoomLink?.Invoke(agoraController.ChannelName);
        } else {
            if (!string.IsNullOrWhiteSpace(currentStreamId)) {
                StreamCallBacks.onShareStreamLinkById?.Invoke(currentStreamId);
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
            TogglePreLiveControls(false);

            if (isChannelCreator) //Send event to viewers to disconnect if streamer
                SendStreamLeaveStatusToViewers();
        }
        StopStatusUpdateRoutine();
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

            if (!agoraController.IsLive) {
                HelperFunctions.DevLogError("Tried to send stream status update before live");
                yield break;
            }

            yield return new WaitForSeconds(5);
            agoraController.SendAgoraMessage(CreateMultiMessage(GetPushToTalkStatusMessage(), GetChannelCreatorUIDMessage(), GetVideoAudioOnOffStatusMessage()));
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

        if (!agoraController.IsLive)
            return;

        agoraController.SendAgoraMessage(GetVideoAudioOnOffStatusMessage());
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
        agoraController.SendAgoraMessage(GetPushToTalkStatusMessage());
    }

    private string GetPushToTalkStatusMessage() {
        return isPushToTalkActive ? MessageToViewerEnableTwoWayAudio : MessageToViewerDisableTwoWayAudio;
    }

    private void SendChannelCreatorUIDToViewers() {
        agoraController.SendAgoraMessage(GetChannelCreatorUIDMessage());
    }

    private string GetChannelCreatorUIDMessage() {
        return MessageToViewerChannelCreatorUID + agoraController.ChannelCreatorUID;
    }

    private void SendStreamLeaveStatusToViewers() {
        agoraController.SendAgoraMessage(MessageToViewerStreamerLeft);
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
                agoraController.ToggleLiveStreamQuad(false);
                return;
            case MessageToViewerBroadcasterVideoPaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                AnimatedCentreTextMessage("Video has been turned off \n by the broadcaster");
                agoraController.ToggleLiveStreamQuad(true);
                return;
            case MessageToViewerBroadcasterAudioAndVideoPaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                AnimatedCentreTextMessage("Video and Audio has been turned off \n by the broadcaster");
                agoraController.ToggleLiveStreamQuad(true);
                return;
            case MessageToViewerBroadcasterUnpaused:
                if (LastMessageWasRecievedAlready(ref lastPauseStatusMessageReceived, message)) {//Prevent functions being called twice if receiving messages again (when a another user joins)
                    return;
                }
                AnimatedFadeOutMessage();
                agoraController.ToggleLiveStreamQuad(false);
                return;
            case MessageToViewerStreamerLeft:
                agoraController.OnStreamerLeft?.Invoke();
                return;
        }

        if (message.Contains(MessageToViewerChannelCreatorUID)) {

            uint result;
            if (!uint.TryParse(message.Substring(MessageToViewerChannelCreatorUID.Length), out result)) {
                HelperFunctions.DevLogError("Channel creator UID parse failed");
            }
            agoraController.ChannelCreatorUID = result;
            agoraController.ActivateViewerVideoSufaceFeatures();
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
        speechNotificationPopups.ActivatePopup(userWebManager.GetUsername());
        agoraController.SendAgoraMessage(MessageToAllViewerIsSpeaking + userWebManager.GetUsername());
    }

    /// <summary>
    /// Should be called when viewers lets go off push to talk button, not intended to be called when messages are received.
    /// </summary>
    private void DisableSpeakingMessage() {
        speechNotificationPopups.DeactivatePopup(userWebManager.GetUsername());
        agoraController.SendAgoraMessage(MessageToAllViewerSpeakingStopped + userWebManager.GetUsername());
    }

    /// <summary>
    /// Starts the stream, use countdown coroutine to start with delay
    /// </summary>
    public void StartStream() {
        TogglePreLiveControls(false);
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
            videoSurface.EnableFilpTextureApply(true, true);
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
            ShowMicrophoneMuteStatusMessage(mute);
            SendVideoAudioPauseStatusToViewers();
        } else if (togglePushToTalk.interactable) { //Do not show microphone is off message unless push to talk is active for viewers
            ShowMicrophoneMuteStatusMessage(mute);
        }
        agoraController.ToggleLocalAudio(mute);
    }

    private void ShowMicrophoneMuteStatusMessage(bool mute) {
        AnimatedCentreTextMessage("Your microphone is " + (mute ? "off" : "on") + ".");
        AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
    }

    public void ToggleVideo(bool hideVideo) {
        _hideVideo = hideVideo;

        AnimatedCentreTextMessage("Your camera is " + (hideVideo ? "off" : "on") + ".");
        AnimatedFadeOutMessage(STATUS_MESSAGE_HIDE_DELAY);
        SendVideoAudioPauseStatusToViewers();

        //UpdateToggleMessageOff();
        agoraController.ToggleVideo(hideVideo);
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
