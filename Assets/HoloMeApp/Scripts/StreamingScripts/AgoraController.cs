using agora_gaming_rtc;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgoraController : MonoBehaviour
{
    [SerializeField]
    string appId = "de596f86fdde42e8a7f7a39b15ad3c82";

    [SerializeField]
    GameObject liveStreamQuad;

    [SerializeField]
    AgoraRTMChatController agoraRTMChatController;

    IRtcEngine iRtcEngine;

    public string ChannelName { get; set; }

    bool isChannelCreator;
    bool isLive;
    int userCount;
    int streamID;
    [HideInInspector]
    public uint frameRate;
    public Action<int> OnCountIncremented;
    public Action OnStreamerLeft;
    public Action OnCameraSwitched;

    VideoSurface videoSurfaceRef;

    public void Start()
    {
        LoadEngine(appId);
        frameRate = 30;
        agoraRTMChatController.Init(appId);
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

    public void JoinOrCreateChannel(bool channelCreator)
    {
        if (iRtcEngine == null)
            return;

        agoraRTMChatController.Login();

        isChannelCreator = channelCreator;

        iRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);

        if (isChannelCreator)
        {
            iRtcEngine.SetClientRole(CLIENT_ROLE.BROADCASTER);
            var encoderConfiguration = new VideoEncoderConfiguration();
            encoderConfiguration.degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_BALANCED;
            encoderConfiguration.minFrameRate = 15;
            encoderConfiguration.frameRate = FRAME_RATE.FRAME_RATE_FPS_30;
            encoderConfiguration.bitrate = 3000;
            encoderConfiguration.dimensions = new VideoDimensions() { width = 720, height = 1280 };
            encoderConfiguration.orientationMode = ORIENTATION_MODE.ORIENTATION_MODE_ADAPTIVE;
            iRtcEngine.SetVideoEncoderConfiguration(encoderConfiguration);
        }
        else
        {
            liveStreamQuad.SetActive(true);
            iRtcEngine.SetClientRole(CLIENT_ROLE.AUDIENCE);
        }

        // set callbacks (optional)
        iRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        iRtcEngine.OnUserJoined = OnUserJoined; //Only fired for broadcasters
        iRtcEngine.OnUserOffline = OnUserOffline;
        //iRtcEngine.EnableDualStreamMode(true);

        // enable video
        iRtcEngine.EnableVideo();
        // allow camera output callback
        iRtcEngine.EnableVideoObserver();

        // join channel
        iRtcEngine.JoinChannel(ChannelName, null, 0);

        isLive = true;

        streamID = iRtcEngine.CreateDataStream(true, true);
                
        //iRtcEngine.OnStreamMessage = OnStreamMessageRecieved;
        //iRtcEngine.OnStreamMessageError = OnStreamMessageError;

    }

    void OnUserOffline(uint uid, USER_OFFLINE_REASON reason) //Only called for host
    {
        HelperFunctions.DevLog("onUserOffline: uid = " + uid + " reason = " + reason);
        OnStreamerLeft?.Invoke();
    }

    public void Leave()
    {
        if (iRtcEngine == null)
            return;

        //if (isChannelCreator)
        //{
        //iRtcEngine.SendStreamMessage(streamID, "CreatorLeft");
        //}

        iRtcEngine.LeaveChannel();
        iRtcEngine.DisableVideoObserver();
        liveStreamQuad.SetActive(false);
        isLive = false;

        ResetVideoSurface();

        //OnStreamDisconnected();
        agoraRTMChatController.LeaveChannel();
    }

    private void ResetVideoSurface()
    {
        if (videoSurfaceRef)
        {
            Destroy(videoSurfaceRef);
            liveStreamQuad.GetComponent<MeshRenderer>().material.mainTexture = null;
            Resources.UnloadUnusedAssets();
        }
    }

    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        HelperFunctions.DevLog("JoinChannelSuccessHandler: uid = " + uid);
        agoraRTMChatController.JoinChannel(channelName);
    }

    void IncrementCount()
    {
        userCount++;
        OnCountIncremented(userCount);
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        HelperFunctions.DevLog("onUserJoined: uid = " + uid + " elapsed = " + elapsed);

        if (!isChannelCreator)
        {
            ResetVideoSurface();

            videoSurfaceRef = liveStreamQuad.GetComponent<VideoSurface>();
            if (!videoSurfaceRef)
            {
                videoSurfaceRef = liveStreamQuad.AddComponent<VideoSurface>();
            }

            videoSurfaceRef.SetForUser(uid);
            videoSurfaceRef.SetEnable(true);
            videoSurfaceRef.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
            videoSurfaceRef.EnableFlipTextureApplyTransform(true, true);
            //videoSurfaceRef.EnableFilpTextureApply(true, true);
            videoSurfaceRef.SetGameFps(frameRate);

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

    private void VideoResolution()
    {
        int width = liveStreamQuad.GetComponent<MeshRenderer>().material.mainTexture.width;
        int height = liveStreamQuad.GetComponent<MeshRenderer>().material.mainTexture.height;
        HelperFunctions.DevLog($"TextureSize = {width} x {height}");
    }

    public void UnloadEngine()
    {
        HelperFunctions.DevLog("calling unloadEngine");

        if (iRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            iRtcEngine = null;
        }

        isLive = false;
    }

    public void SwitchCamera()
    {
        int result = iRtcEngine.SwitchCamera();
        if (result == 0)
            OnCameraSwitched?.Invoke();
    }

    public void ToggleVideo(bool pauseVideo)
    {
        if (iRtcEngine != null)
        {
            if (!pauseVideo)
            {
                iRtcEngine.EnableVideo();
            }
            else
            {
                iRtcEngine.DisableVideo();
            }
        }
    }

    public void ToggleAudio(bool pauseAudio)
    {
        if (iRtcEngine != null)
        {
            if (!pauseAudio)
            {
                iRtcEngine.EnableAudio();
            }
            else
            {
                iRtcEngine.DisableAudio();
            }
        }
    }

    public string GetSdkVersion()
    {
        string ver = IRtcEngine.GetSdkVersion();
        if (ver == "2.9.1.45")
        {
            ver = "2.9.2";  // A conversion for the current internal version#
        }
        else
        {
            if (ver == "2.9.1.46")
            {
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

    void OnApplicationPause(bool paused)
    {
        if (!ReferenceEquals(iRtcEngine, null))
        {
            ToggleVideo(paused);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(iRtcEngine, null))
        {
            UnloadEngine();
        }
    }

    IEnumerator UpdateUsers()
    {
        if (isChannelCreator)
        {
            while (isLive)
            {
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
