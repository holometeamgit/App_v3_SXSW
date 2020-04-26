using agora_gaming_rtc;
using UnityEngine;

public class AgoraController : MonoBehaviour
{
    [SerializeField]
    string appId = "de596f86fdde42e8a7f7a39b15ad3c82";

    IRtcEngine iRtcEngine;

    public void Start()
    {
        LoadEngine(appId);
    }

    void LoadEngine(string appId)
    {
        if (iRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        iRtcEngine = IRtcEngine.GetEngine(appId);

        if (Debug.isDebugBuild || Application.isEditor)
            iRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
        else
            iRtcEngine.SetLogFilter(LOG_FILTER.CRITICAL);
    }

    public void JoinChannel(string channelName)
    {
        if (iRtcEngine == null)
            return;

        // set callbacks (optional)
        iRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        iRtcEngine.OnUserJoined = OnUserJoined;
        iRtcEngine.OnUserOffline = OnUserOffline;

        // enable video
        iRtcEngine.EnableVideo();
        // allow camera output callback
        iRtcEngine.EnableVideoObserver();

        // join channel
        iRtcEngine.JoinChannel(channelName, null, 0);

        //int streamID = iRtcEngine.CreateDataStream(true, true);
        //iRtcEngine.OnStreamMessage = OnStreamMessageRecieved;
        //iRtcEngine.OnStreamMessageError= ;
        //iRtcEngine.SendStreamMessage("");
    }

    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        // this is called in main thread
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Object.Destroy(go);
        }
    }

    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
        GameObject textVersionGameObject = GameObject.Find("VersionText");
        textVersionGameObject.GetComponent<Text>().text = "SDK Version : " + getSdkVersion();
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(30);
        }
    }

    public void OnStreamMessageRecieved(uint userId, int streamId, string data, int length)
    {

    }

    public void Leave()
    {
        if (iRtcEngine == null)
            return;

        // leave channel
        iRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        iRtcEngine.DisableVideoObserver();
    }

    public void UnloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (iRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            iRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
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

    void OnApplicationPause(bool paused)
    {
        if (!ReferenceEquals(iRtcEngine, null))
        {
            EnableVideo(paused);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(iRtcEngine, null))
        {
            UnloadEngine();
        }
    }

}
