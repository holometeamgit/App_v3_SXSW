﻿using agora_gaming_rtc;
using System.Collections;
using UnityEngine;

public class AgoraController : MonoBehaviour
{
    [SerializeField]
    string appId = "de596f86fdde42e8a7f7a39b15ad3c82";

    [SerializeField]
    GameObject liveStreamQuad;

    IRtcEngine iRtcEngine;

    public static string ChannelName { get; private set; }

    bool isChannelCreator;
    bool isLive;
    int userCount;

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

        iRtcEngine = IRtcEngine.GetEngine(appId);

        if (Debug.isDebugBuild || Application.isEditor)
            iRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
        else
            iRtcEngine.SetLogFilter(LOG_FILTER.CRITICAL);

        liveStreamQuad.SetActive(false);
    }

    public void JoinOrCreateChannel(string channelName, bool channelCreator)
    {
        if (iRtcEngine == null)
            return;

        isChannelCreator = channelCreator;
        ChannelName = channelName;

        iRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);

        if (isChannelCreator)
        {
            iRtcEngine.SetClientRole(CLIENT_ROLE.BROADCASTER);
        }
        else
        {
            liveStreamQuad.SetActive(true);
            iRtcEngine.SetClientRole(CLIENT_ROLE.AUDIENCE);
        }

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

        isLive = true;

        //int streamID = iRtcEngine.CreateDataStream(true, true);
        //iRtcEngine.OnStreamMessage = OnStreamMessageRecieved;
        //iRtcEngine.OnStreamMessageError= ;

    }

    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);

        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Object.Destroy(go);
        }
    }

    public void Leave()
    {
        if (iRtcEngine == null)
            return;


        if (isChannelCreator)
        {
            //iRtcEngine.SendStreamMessage(0, "CreatorLeft");
        }

        // leave channel
        iRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        iRtcEngine.DisableVideoObserver();
        liveStreamQuad.SetActive(false);
        isLive = false;
    }


    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
    }

    private void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);

        if (isChannelCreator)
            userCount++;

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = MakeImageSurface(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
            videoSurface.SetGameFps(30);
        }

    }

    public VideoSurface MakeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;

        // to be renderered onto
        //go.AddComponent<RawImage>();

        // make the object draggable
        //go.AddComponent<UIElementDragger>();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            go.transform.parent = canvas.transform;
        }
        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        //float xPos = Random.Range(Offset - Screen.width / 2f, Screen.width / 2f - Offset);
        //float yPos = Random.Range(Offset, Screen.height / 2f - Offset);
        //go.transform.localPosition = new Vector3(xPos, yPos, 0f);
        go.transform.localScale = new Vector3(3f, 4f, 1f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    public void OnStreamMessageRecieved(uint userId, int streamId, string data, int length)
    {

    }


    public void UnloadEngine()
    {
        Debug.Log("calling unloadEngine");

        if (iRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            iRtcEngine = null;
        }

        isLive = false;
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
