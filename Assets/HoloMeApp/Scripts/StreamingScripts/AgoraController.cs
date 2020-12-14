using agora_gaming_rtc;
using System;
using System.Collections;
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
    StreamerCountUpdater streamerCountUpdater;

    TokenAgoraResponse tokenAgoraResponseChannel;
    TokenAgoraResponse tokenAgoraResponseRTM;

    IRtcEngine iRtcEngine;

    public string ChannelName { get; set; }
    public bool IsLive { get; private set; }
    public bool IsChannelCreator { get; private set; }
    public bool VideoIsReady { get; private set; }

    int userCount;

    [HideInInspector]
    public uint frameRate;
    public Action<int> OnCountIncremented;
    public Action OnStreamerLeft;
    public Action OnCameraSwitched;

    [SerializeField]
    RawImage videoSufaceStreamerRawTex;
    VideoSurface videoSurfaceQuadRef;
    Coroutine sendThumbnailRoutine;

    static Vector3 defaultLiveStreamQuadScale;

    public void Start() {
        LoadEngine(AppId);
        frameRate = 30;
        agoraRTMChatController.Init(AppId);
        secondaryServerCalls.OnStreamStarted += (x, y) => SecondaryServerCallsComplete(x, y);
                
        iRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        iRtcEngine.OnUserJoined = OnUserJoined; //Only fired for broadcasters
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

        var encoderConfiguration = new VideoEncoderConfiguration();
        encoderConfiguration.degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_BALANCED;
        encoderConfiguration.minFrameRate = 15;
        encoderConfiguration.frameRate = FRAME_RATE.FRAME_RATE_FPS_30;
        encoderConfiguration.bitrate = 3000;
        encoderConfiguration.dimensions = new VideoDimensions() { width = 720, height = 1280 };
        encoderConfiguration.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_ADAPTIVE;
        iRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
    }

    void LoadEngine(string appId)
    {
        if (iRtcEngine != null)
        {
            HelperFunctions.DevLog("Engine exists. Please unload it first!");
            return;
        }

        iRtcEngine = IRtcEngine.GetEngine(appId);

        if (Debug.isDebugBuild || Application.isEditor)
            iRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
        else
            iRtcEngine.SetLogFilter(LOG_FILTER.CRITICAL);

        liveStreamQuad.SetActive(false);
    }

    void OnPreviewReady(uint i, bool b)
    {
        HelperFunctions.DevLog("REMOTE USER CHANGED VIDEO SETTINGS");
    }

    public void StartPreview()
    {
        //iRtcEngine.OnUserEnableVideo  += OnPreviewReady; TODO: this may be used to show a custom videoDisabled image for remote users only
        //iRtcEngine.EnableLocalVideo(true);

        if (iRtcEngine.EnableVideo() == 0)
        {
            if (iRtcEngine.EnableVideoObserver() == 0)
            {
                if (iRtcEngine.StartPreview() == 0)
                {
                    HelperFunctions.DevLog("Agora Preview Started");
                    VideoIsReady = true;
                }
                else
                {
                    HelperFunctions.DevLog("Agora Preview Failed");
                }
            }
        }
    }
    
    public void StopPreview()
    {
        iRtcEngine.DisableVideo();
        iRtcEngine.DisableVideoObserver();
        if( iRtcEngine.StopPreview() == 0)
        {
            HelperFunctions.DevLog("Agora Preview Stopped");
        }        
        ResetVideoQuadSurface();
    }
          
    public void JoinOrCreateChannel(bool channelCreator) {
        if (iRtcEngine == null)
            return;

        IsChannelCreator = channelCreator;
        if (channelCreator)
            secondaryServerCalls.StartStream(ChannelName);
        else {
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
        } catch (System.Exception) { }
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
        } catch (System.Exception) { }
    }

    public void SecondaryServerCallsComplete(string viewerBroadcasterToken, string rtmToken) {
        agoraRTMChatController.Login(rtmToken);

        iRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);

        if (IsChannelCreator) {
                iRtcEngine.SetClientRole(CLIENT_ROLE.BROADCASTER);
        } else {
                liveStreamQuad.SetActive(true);
                iRtcEngine.SetClientRole(CLIENT_ROLE.AUDIENCE);
                StartPreview(); //Must be called for viewers to view
        }
              
        //iRtcEngine.EnableDualStreamMode(true);
               
        var result = iRtcEngine.JoinChannelByKey(viewerBroadcasterToken, ChannelName, null, IsChannelCreator ? 1u : 0);

        if (result < 0) {
            Debug.LogError("Agora Stream Join Failed!");
        } else {
            HelperFunctions.DevLog("Agora Stream Join Success!");
        }

        //print("JOINED");

        if (IsChannelCreator)
            sendThumbnailRoutine = StartCoroutine(SendThumbnailData());

        streamerCountUpdater.StartCheck(ChannelName);

        IsLive = true;

        //streamID = iRtcEngine.CreateDataStream(true, true);

        //iRtcEngine.OnStreamMessage = OnStreamMessageRecieved;
        //iRtcEngine.OnStreamMessageError = OnStreamMessageError;
    }

    void OnUserOffline(uint uid, USER_OFFLINE_REASON reason) //Only called for host
    {
        HelperFunctions.DevLog("onUserOffline: uid = " + uid + " reason = " + reason);
        OnStreamerLeft?.Invoke();
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

        if (sendThumbnailRoutine != null)
            StopCoroutine(sendThumbnailRoutine);

        streamerCountUpdater.StopCheck();
        StopPreview();

        liveStreamQuad.SetActive(false);

        if (IsChannelCreator)
            secondaryServerCalls.EndStream();

        iRtcEngine.LeaveChannel();
        agoraRTMChatController.LeaveChannel();
        //OnStreamDisconnected();

        IsLive = false;
    }

    IEnumerator SendThumbnailData() {
        yield return new WaitForSeconds(5);
        Texture2D originalSnapShot = (Texture2D)videoSufaceStreamerRawTex.texture;
        Color[] pixels = originalSnapShot.GetPixels();
        Array.Reverse(pixels);
        Texture2D flippedTexture = new Texture2D(originalSnapShot.width, originalSnapShot.height);
        flippedTexture.SetPixels(pixels);
        byte[] data = flippedTexture.EncodeToPNG();
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
        agoraRTMChatController.JoinChannel(channelName);
    }

    void IncrementCount() {
        userCount++;
        OnCountIncremented(userCount);
    }
    
    private void OnUserJoined(uint uid, int elapsed) {
        HelperFunctions.DevLog("onUserJoined: uid = " + uid + " elapsed = " + elapsed);

        if (!IsChannelCreator) {
            ResetVideoQuadSurface();

            if (defaultLiveStreamQuadScale == Vector3.zero) {
                //print("SETTING DEFAULT QUAD SCALE");
                defaultLiveStreamQuadScale = liveStreamQuad.transform.localScale;
            }

            videoSurfaceQuadRef = liveStreamQuad.GetComponent<VideoSurface>();
            if (!videoSurfaceQuadRef) {
                videoSurfaceQuadRef = liveStreamQuad.AddComponent<VideoSurface>();
                //print("ADDED VIDEO SURFACE");
            }
                        
            videoSurfaceQuadRef.SetForUser(uid);
            videoSurfaceQuadRef.SetEnable(true);
            videoSurfaceQuadRef.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
            videoSurfaceQuadRef.EnableFlipTextureApplyTransform(true, true, defaultLiveStreamQuadScale);
            //videoSurfaceRef.EnableFilpTextureApply(true, true);
            videoSurfaceQuadRef.SetGameFps(frameRate);
            StartPreview();
            //liveStreamQuad.GetComponent<LiveStreamGreenCalculator>().StartBackgroundRemoval();

            //Invoke("VideoResolution", 3);
        }
    }

    //public void FlipVideoQuad(bool flipHorizontal, bool flipVertical)
    //{
    //    float newXScale = flipHorizontal ? -2 : 2;//? -liveStreamQuad.transform.localScale.x : liveStreamQuad.transform.localScale.x;
    //    float newYScale = flipVertical ? -3.4f : 3.4f;
    //    liveStreamQuad.transform.localScale = new Vector3(newXScale, newYScale, liveStreamQuad.transform.localScale.z);
    //}

    private void VideoResolution() {
        int width = liveStreamQuad.GetComponent<MeshRenderer>().material.mainTexture.width;
        int height = liveStreamQuad.GetComponent<MeshRenderer>().material.mainTexture.height;
        HelperFunctions.DevLog($"TextureSize = {width} x {height}");
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

    public void ToggleVideo(bool pauseVideo) {
        if (iRtcEngine != null) {
            if (!pauseVideo) {
                iRtcEngine.EnableVideo();
            } else {
                iRtcEngine.DisableVideo();
            }
        }
    }

    public void ToggleAudio(bool pauseAudio) {
        if (iRtcEngine != null) {
            if (!pauseAudio) {
                iRtcEngine.EnableAudio();
            } else {
                iRtcEngine.DisableAudio();
            }
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

    //public void SendMessage(string message)
    //{
    //    iRtcEngine.SendStreamMessage(streamID, message);
    //}
    //public void OnStreamMessageError(uint userId, int streamId, int code, int missed, int cached)
    //{
    //    HelperFunctions.DevLog($"Stream message error! Code = {code}");
    //}

    //List<AgoraMessageReceiver> messageReceivers = new List<AgoraMessageReceiver>();
    //public void AddMessageReceiver(AgoraMessageReceiver agoraMessageReceiver)
    //{
    //    if (!messageReceivers.Contains(agoraMessageReceiver))
    //    {
    //        messageReceivers.Add(agoraMessageReceiver);
    //    }
    //    else
    //    {
    //        Debug.LogError("Tried to add the same messageReceiver");
    //    }
    //}

    //public void RemoveMessageReceiver(AgoraMessageReceiver agoraMessageReceiver)
    //{
    //    if (messageReceivers.Contains(agoraMessageReceiver))
    //    {
    //        messageReceivers.Remove(agoraMessageReceiver);
    //    }
    //    else
    //    {
    //        Debug.LogError("Tried to remove messageReceiver but wasn't in colection");
    //    }
    //}

    //public void OnStreamMessageRecieved(uint userId, int streamId, string data, int length)
    //{
    //    HelperFunctions.DevLog($"Message recieved {data}");

    //    foreach (AgoraMessageReceiver agoraMessageReceiver in messageReceivers)
    //    {
    //        agoraMessageReceiver.ReceivedChatMessage(data);
    //    }
    //}

    //public void OnStreamDisconnected()
    //{
    //    foreach (AgoraMessageReceiver agoraMessageReceiver in messageReceivers)
    //    {
    //        agoraMessageReceiver.OnDisconnected();
    //    }
    //}

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

    IEnumerator UpdateUsers() {
        if (IsChannelCreator) {
            while (IsLive) {
                yield return new WaitForSeconds(5);
            }
        }
    }

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
