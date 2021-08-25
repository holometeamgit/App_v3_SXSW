using agora_gaming_rtc;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class AgoraController : MonoBehaviour {

#if DEV
    public const string AppId = "9f6b623b2365404ea78ab4b08d8059eb";
#else
    public const string AppId = "fa326d8119c84b739d398604931a3c8b";
#endif

    [SerializeField]
    GameObject liveStreamQuad;

    [SerializeField]
    AgoraRTMChatController agoraRTMChatController;

    [SerializeField]
    SecondaryServerCalls secondaryServerCalls;

    [SerializeField]
    UserWebManager userWebManager;

    TokenAgoraResponse tokenAgoraResponseChannel;
    TokenAgoraResponse tokenAgoraResponseRTM;

    IRtcEngine iRtcEngine;

    public string ChannelName { get; set; }
    public bool IsLive { get; private set; }
    public bool IsChannelCreator { get; set; }
    public bool IsRoom { get; set; }
    public bool VideoIsReady { get; private set; }
    public uint? ChannelCreatorUID { get; set; } = null;

    private bool videoQuadWasSetup; //Required to stop new calls from reconfiguring video surface quad

    int streamID = -1;
    int agoraMessageStreamID;

    [HideInInspector]
    public uint frameRate;
    public Action OnStreamerLeft;
    public Action OnCameraSwitched;
    public Action OnPreviewStopped;
    public Action OnStreamWentLive;
    public Action OnStreamWentOffline;
    public Action<string> OnMessageRecieved;

    /// <summary>
    /// Called only for communication channels
    /// </summary>
    public Action OnUserViewerJoined;

    [SerializeField]
    RawImage videoSufaceStreamerRawTex;
    VideoSurface videoSurfaceQuadRef;
    Coroutine sendThumbnailRoutine;

    static Vector3 defaultLiveStreamQuadScale;

    //void OnUserEnableVideoHandler(uint uid, bool enabled)
    //{
    //    print("OnUserEnableVideoHandler called");
    //}

    public void Start() {
        LoadEngine(AppId);
        frameRate = 30;
        agoraRTMChatController.Init(AppId);
        secondaryServerCalls.OnStreamStarted += (x, y, z) => SecondaryServerCallsComplete(x, y, z);

        //iRtcEngine.OnUserEnableVideo = OnUserEnableVideoHandler;
        //iRtcEngine.OnUserEnableLocalVideo = OnUserEnableVideoHandler;

        iRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        iRtcEngine.OnUserJoined = OnUserJoined; //Only fired for broadcasters in broadcast profile
        iRtcEngine.OnUserOffline = OnUserOffline;
        iRtcEngine.OnWarning += (int warn, string msg) => {
            string description = IRtcEngine.GetErrorDescription(warn);
            string warningMessage = string.Format("Agora onWarning callback {0} {1} {2}", warn, msg, description);
            HelperFunctions.DevLog(warningMessage);
        };
        iRtcEngine.OnError += (int error, string msg) => {
            string description = IRtcEngine.GetErrorDescription(error);
            string errorMessage = string.Format("Agora onError callback {0} {1} {2}", error, msg, description);
            HelperFunctions.DevLogError(errorMessage);
        };

        SetEncoderSettings();
    }

    void LoadEngine(string appId) {
        if (iRtcEngine != null) {
            HelperFunctions.DevLog("Engine exists. Please unload it first!");
            return;
        }

        iRtcEngine = IRtcEngine.GetEngine(appId);

        //Logging causing iOS crashes
#if DEV
        //iRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
#else
        //iRtcEngine.SetLogFilter(LOG_FILTER.CRITICAL);
#endif

        liveStreamQuad.SetActive(false);
    }

    private void SetEncoderSettings() {
        var encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_BALANCED;
        encoderConfiguration.minFrameRate = 25;
        encoderConfiguration.frameRate = (FRAME_RATE)AgoraSharedVideoConfig.FrameRate;
        encoderConfiguration.bitrate = AgoraSharedVideoConfig.Bitrate;
        encoderConfiguration.dimensions = new VideoDimensions() { width = AgoraSharedVideoConfig.Width, height = AgoraSharedVideoConfig.Height };
        encoderConfiguration.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_FIXED_PORTRAIT;//ORIENTATION_MODE.ORIENTATION_MODE_ADAPTIVE;
        //iRtcEngine.SetVideoProfile(VIDEO_PROFILE_TYPE.VIDEO_PROFILE_PORTRAIT_720P_3,false);
        iRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
    }

    //void OnPreviewReady(uint i, bool b)
    //{
    //    HelperFunctions.DevLog("REMOTE USER CHANGED VIDEO SETTINGS");
    //}

    public void StartPreview() {
        if (iRtcEngine == null) {
            Debug.LogError("iRtC Engine was null when trying to start preview");
            return;
        }

        if (EnableVideoPlayback() == 0) {
            if (iRtcEngine.StartPreview() == 0) {
                HelperFunctions.DevLog("Agora Preview Started");
                if (iRtcEngine.EnableLocalVideo(true) == 0) {
                    VideoIsReady = true;
                }
            } else {
                HelperFunctions.DevLog("Agora Preview Failed");
            }
        }
    }

    public void StopPreview() {
        iRtcEngine.DisableVideo();
        iRtcEngine.DisableVideoObserver();
        if (iRtcEngine.StopPreview() == 0) {
            HelperFunctions.DevLog("Agora Preview Stopped");
        }
        iRtcEngine.EnableLocalVideo(false);
        VideoIsReady = false;
        OnPreviewStopped?.Invoke();
    }

    public int EnableVideoPlayback() {
        if (iRtcEngine == null) {
            Debug.LogError("iRtC Engine was null when trying to start preview");
            return -1;
        }

        if (iRtcEngine.EnableVideo() == 0) {
            return iRtcEngine.EnableVideoObserver();
        } else {
            return -1;
        }
    }

    public void JoinOrCreateChannel(bool channelCreator) {
        if (iRtcEngine == null)
            return;

        if (channelCreator) {
            secondaryServerCalls.StartStream(ChannelName, IsRoom);
        } else {
            GetViewerAgoraToken();
        }
    }

    void GetViewerAgoraToken() {
        HelperFunctions.DevLog("Getting Agora Viewer Token For Channel Name " + ChannelName);
        secondaryServerCalls.GetAgoraToken(OnViewerAgoraTokenReturned, ChannelName);
    }

    void OnViewerAgoraTokenReturned(long code, string data) {
        try {
            tokenAgoraResponseChannel = JsonUtility.FromJson<TokenAgoraResponse>(data);
            HelperFunctions.DevLog("Viewer Token Returned: " + tokenAgoraResponseChannel.token);
            GetRTMLoginToken();
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    void GetRTMLoginToken() {
        HelperFunctions.DevLog("Getting Agora RTM Token");
        secondaryServerCalls.GetAgoraToken(OnRTMAgoraTokenReturned);
    }

    void OnRTMAgoraTokenReturned(long code, string data) {
        try {
            tokenAgoraResponseRTM = JsonUtility.FromJson<TokenAgoraResponse>(data);
            HelperFunctions.DevLog("RTM Token Returned: " + tokenAgoraResponseRTM.token);
            SecondaryServerCallsComplete(tokenAgoraResponseChannel.token, tokenAgoraResponseRTM.token);
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    /// <summary>
    /// Send viewer count for live stream, creator only
    /// </summary>
    public void SendViewerCountAnalyticsUpdate(int count) {
        if (IsChannelCreator) {
            AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyViewerCountUpdate, new System.Collections.Generic.Dictionary<string, string> { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID }, { AnalyticParameters.ParamPerformanceID, streamID.ToString() }, { AnalyticParameters.ParamIsRoom, IsRoom.ToString() }, { AnalyticParameters.ParamViewerCount, count.ToString() } });
        }
    }

    public void SecondaryServerCallsComplete(string viewerBroadcasterToken, string rtmToken, int streamID = -1) {
        this.streamID = streamID;

        agoraRTMChatController.Login(rtmToken);
        iRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);

        if (IsChannelCreator) {
            iRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            SetEncoderSettings();
            AnalyticsController.Instance.SendCustomEventToSpecifiedControllers(new AnalyticsLibraryAbstraction[] { AnalyticsCleverTapController.Instance, AnalyticsAmplitudeController.Instance }, AnalyticKeys.KeyLiveStarted, new System.Collections.Generic.Dictionary<string, string> { { AnalyticParameters.ParamBroadcasterUserID, AnalyticsController.Instance.GetUserID }, { AnalyticParameters.ParamPerformanceID, streamID.ToString() }, { AnalyticParameters.ParamIsRoom, IsRoom.ToString() } });
        } else {
            iRtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
            EnableVideoPlayback(); //Must be called for viewers to view
            ToggleLocalVideo(true); //Disable local video freeze fix iOS
        }

        //iRtcEngine.EnableDualStreamMode(true);

        var result = iRtcEngine.JoinChannelByKey(viewerBroadcasterToken, ChannelName, null, Convert.ToUInt32(userWebManager.GetUserID()));

        if (result < 0) {
            Debug.LogError("Agora Stream Join Failed!");
        } else {
            HelperFunctions.DevLog("Agora Stream Join Success!");
        }

        //print("JOINED");

        if (IsChannelCreator && !IsRoom)//No thumbnails for rooms for now
            sendThumbnailRoutine = StartCoroutine(SendThumbnailData(true));

        IsLive = true;

        agoraMessageStreamID = iRtcEngine.CreateDataStream(true, true);

        iRtcEngine.OnStreamMessage = OnStreamMessageRecieved;
        iRtcEngine.OnStreamMessageError = OnStreamMessageError;

        OnStreamWentLive?.Invoke();
    }

    public void Leave() {

        if (iRtcEngine == null)
            return;

        if (!IsLive)
            return;

        //if (isChannelCreator)
        //{
        //iRtcEngine.SendStreamMessage(streamID, "CreatorLeft");
        //}

        if (sendThumbnailRoutine != null && !IsRoom)//No thumbnails for rooms for now
            StopCoroutine(sendThumbnailRoutine);

        if (IsChannelCreator) {
            secondaryServerCalls.EndStream();
        } else {
            liveStreamQuad.SetActive(false);
            ResetVideoQuadSurface();
            videoQuadWasSetup = false;
        }
        ChannelCreatorUID = null;

        iRtcEngine.LeaveChannel();
        agoraRTMChatController.LeaveChannel();

        IsLive = false;
        OnStreamWentOffline?.Invoke();
    }

    IEnumerator SendThumbnailData(bool flipVertical) {
        yield return new WaitForSeconds(5);
        Texture2D originalSnapShot = (Texture2D)videoSufaceStreamerRawTex.texture;
        byte[] data;

        if (originalSnapShot == null) {
            Debug.LogError("Stream thumbnail was null");
            yield break;
        }

        if (flipVertical) {
            int width = originalSnapShot.width;
            int height = originalSnapShot.height;
            Texture2D resultTexture = new Texture2D(width, height);
            Color[] pixels = originalSnapShot.GetPixels();
            Color[] pixelsFlipped = new Color[pixels.Length];
            for (int i = 0; i < height; i++) {
                Array.Copy(pixels, i * width, pixelsFlipped, (height - i - 1) * width, width);
            }
            resultTexture.SetPixels(pixelsFlipped);
            resultTexture.Apply();
            data = resultTexture.EncodeToPNG();
        } else {
            data = originalSnapShot.EncodeToPNG();
        }

        secondaryServerCalls.UploadPreviewImage(data);
    }

    private void ResetVideoQuadSurface() {
        if (videoSurfaceQuadRef) {
            Destroy(videoSurfaceQuadRef);
            liveStreamQuad.GetComponent<MeshRenderer>().material.mainTexture = null;
            Resources.UnloadUnusedAssets();
        }
    }

    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed) {
        HelperFunctions.DevLog("JoinChannelSuccessHandler: uid = " + uid);
        if (IsChannelCreator) {
            ChannelCreatorUID = uid;
        }
        agoraRTMChatController.JoinChannel(channelName);
    }

    private void OnUserJoined(uint uid, int elapsed) {
        HelperFunctions.DevLog("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        if (IsChannelCreator) {
            OnUserViewerJoined?.Invoke();
        }
    }

    /// <summary>
    /// Adds and activates the video surface component onto the quad
    /// </summary>
    public void ActivateViewerVideoSufaceFeatures() {
        if (videoQuadWasSetup)
            return;

        //print("AGORA MESSAGE QUAD SET");

        videoQuadWasSetup = true;
        ResetVideoQuadSurface();

        if (defaultLiveStreamQuadScale == Vector3.zero) {
            defaultLiveStreamQuadScale = liveStreamQuad.transform.localScale;
        }

        videoSurfaceQuadRef = liveStreamQuad.GetComponent<VideoSurface>();
        if (!videoSurfaceQuadRef) {
            videoSurfaceQuadRef = liveStreamQuad.AddComponent<VideoSurface>();
            //print("Agora message Added video surface component");
        }
        //print("agora message UID = " + ChannelCreatorUID);
        videoSurfaceQuadRef.SetForUser((uint)ChannelCreatorUID);
        videoSurfaceQuadRef.SetEnable(true);
        videoSurfaceQuadRef.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
        videoSurfaceQuadRef.SetGameFps(frameRate);
    }

    void OnUserOffline(uint uid, USER_OFFLINE_REASON reason) //Only called for host in broadcast profile
    {
        HelperFunctions.DevLog("onUserOffline: uid = " + uid + " reason = " + reason);

        //if (IsChannelCreator)//Stops channel creator from leaving when another user leaves DOESN'T stop other users leaving due to it.
        //    return;

        if (uid == ChannelCreatorUID) {
            HelperFunctions.DevLog($"Disconnecting channel as channel broadcaster has dropped {uid} {reason}");
            OnStreamerLeft?.Invoke();
        }
    }

    public void UnloadEngine() {
        HelperFunctions.DevLog("calling unloadEngine");

        if (iRtcEngine != null) {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            iRtcEngine = null;
        }
    }

    public void SwitchCamera() {
        int result = iRtcEngine.SwitchCamera();
        if (result == 0)
            OnCameraSwitched?.Invoke();
    }

    /// <summary>
    /// Toggle the live stream quad
    /// </summary>
    public void ToggleLiveStreamQuad(bool hide) {
        if (IsLive) {
            liveStreamQuad.SetActive(!hide);
        }
    }

    /// <summary>
    /// Enables/Disables the local video capture.
    /// </summary>
    public void ToggleLocalVideo(bool pauseVideo) {
        if (iRtcEngine != null) {
            iRtcEngine.EnableLocalVideo(!pauseVideo);
        }
    }

    /// <summary>
    /// Toggles the video module.
    /// </summary>
    public void ToggleVideo(bool pauseVideo) {
        if (iRtcEngine != null) {
            if (!pauseVideo) {
                iRtcEngine.EnableVideo();
            } else {
                iRtcEngine.DisableVideo();
            }
        }
    }

    /// <summary>
    /// Stops/Resumes sending the local audio stream.
    /// </summary>
    public void ToggleLocalAudio(bool pauseAudio) {
        if (iRtcEngine != null) {
            iRtcEngine.MuteLocalAudioStream(pauseAudio);
        }
    }

    public string GetSdkVersion() {
        string ver = IRtcEngine.GetSdkVersion();
        if (ver == "2.9.1.45") {
            ver = "2.9.2";  // A conversion for the current internal version#
        } else {
            if (ver == "2.9.1.46") {
                ver = "2.9.2.2";  // A conversion for the current internal version#
            }
        }
        return ver;
    }

    #region Messaging system

    /// <summary>
    /// Sends string message to all users in a channel.
    /// </summary>
    public void SendAgoraMessage(string message) {
        HelperFunctions.DevLog($"Sending Agora Message {message}");
        byte[] messageToBytes = Encoding.ASCII.GetBytes(message);
        iRtcEngine.SendStreamMessage(agoraMessageStreamID, messageToBytes);
    }

    private void OnStreamMessageError(uint userId, int streamId, int code, int missed, int cached) {
        HelperFunctions.DevLog($"Stream message error! Code = {code}");
    }

    private void OnStreamMessageRecieved(uint userId, int streamId, byte[] data, int length) {
        string result = Encoding.ASCII.GetString(data);
        HelperFunctions.DevLog($"Agora Message Recieved {result}");
        OnMessageRecieved?.Invoke(result);
    }

    #endregion

    void OnApplicationPause(bool paused) {
        if (!ReferenceEquals(iRtcEngine, null)) {
            ToggleVideo(paused);
        }
    }

    void OnApplicationQuit() {
        if (!ReferenceEquals(iRtcEngine, null)) {
            UnloadEngine();
        }
    }

    //IEnumerator UpdateUsers() {
    //    if (IsChannelCreator) {
    //        while (IsLive) {
    //            yield return new WaitForSeconds(5);
    //        }
    //    }
    //}

    //bool dippedBelowPerformanceThreshold;
    //bool previousPerformanceState;

    //private void Update()
    //{
    //    if (isLive)
    //    {
    //        var fps = (1.0 / Time.deltaTime);

    //        if (fps < 25)
    //        {
    //            dippedBelowPerformanceThreshold = true;
    //        }
    //        else
    //        {
    //            dippedBelowPerformanceThreshold = false;
    //        }

    //        if (previousPerformanceState != dippedBelowPerformanceThreshold)
    //        {
    //            if (dippedBelowPerformanceThreshold)
    //            {
    //                iRtcEngine.SetRemoteDefaultVideoStreamType(REMOTE_VIDEO_STREAM_TYPE.REMOTE_VIDEO_STREAM_LOW);
    //            }
    //            else
    //            {
    //                iRtcEngine.SetRemoteDefaultVideoStreamType(REMOTE_VIDEO_STREAM_TYPE.REMOTE_VIDEO_STREAM_HIGH);
    //            }
    //        }
    //        previousPerformanceState = dippedBelowPerformanceThreshold;
    //    }
    //}
}
