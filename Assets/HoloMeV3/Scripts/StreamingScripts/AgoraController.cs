using agora_gaming_rtc;
using System;
using System.Collections;
using UnityEngine;

public class AgoraController : MonoBehaviour
{
    [SerializeField]
    string appId = "de596f86fdde42e8a7f7a39b15ad3c82";

    [SerializeField]
    GameObject liveStreamQuad;

    IRtcEngine iRtcEngine;

    public string ChannelName { get; set; }

    bool isChannelCreator;
    bool isLive;
    int userCount;
    int streamID;
    public Action<int> OnCountIncremented;
    public Action OnStreamerLeft;

    VideoSurface videoSurfaceRef;

    public void Start()
    {
        LoadEngine(appId);
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

        isChannelCreator = channelCreator;

        iRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);

        if (isChannelCreator)
        {
            iRtcEngine.SetClientRole(CLIENT_ROLE.BROADCASTER);
            var encoderConfiguration = new VideoEncoderConfiguration();
            encoderConfiguration.degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_BALANCED;
            encoderConfiguration.minFrameRate = 15;
            encoderConfiguration.frameRate = FRAME_RATE.FRAME_RATE_FPS_60;
            encoderConfiguration.bitrate = 5000;
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

        // enable video
        iRtcEngine.EnableVideo();
        // allow camera output callback
        iRtcEngine.EnableVideoObserver();

        // join channel
        iRtcEngine.JoinChannel(ChannelName, null, 0);

        isLive = true;

        streamID = iRtcEngine.CreateDataStream(true, true);
        iRtcEngine.OnStreamMessage = OnStreamMessageRecieved;
        //iRtcEngine.OnStreamMessageError = ;

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

        if (isChannelCreator)
        {
            //iRtcEngine.SendStreamMessage(streamID, "CreatorLeft");
        }

        iRtcEngine.LeaveChannel();
        iRtcEngine.DisableVideoObserver();
        liveStreamQuad.SetActive(false);
        isLive = false;

        ResetVideoSurface();
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
            videoSurfaceRef.EnableFilpTextureApply(true, true);
            videoSurfaceRef.SetGameFps(30);

            //liveStreamQuad.GetComponent<LiveStreamGreenCalculator>().StartBackgroundRemoval();

            //Invoke("VideoResolution", 3);
        }
    }

    private void VideoResolution()
    {
        int width = liveStreamQuad.GetComponent<MeshRenderer>().material.mainTexture.width;
        int height = liveStreamQuad.GetComponent<MeshRenderer>().material.mainTexture.height;
        HelperFunctions.DevLog($"TextureSize = {width} x {height}");
    }

    public void OnStreamMessageRecieved(uint userId, int streamId, string data, int length)
    {
        HelperFunctions.DevLog($"Message recieved {data}");
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
        //This will require a screensized or raw image to place camera feed too via adding a videosurface
        //to use 

        iRtcEngine.SwitchCamera();
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
}
