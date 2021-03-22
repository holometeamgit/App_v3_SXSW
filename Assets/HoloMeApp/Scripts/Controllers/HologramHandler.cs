using UnityEngine;
using HoloMeSDK;
using System.Linq;
using System;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class HologramHandler : MonoBehaviour
{
    [SerializeField]
    Transform cameraTransform;

    [SerializeField]
    PlacementHandler placementHandler;

    [SerializeField]
    ServerDataHandler serverDataHandler;

    [SerializeField]
    HologramChild[] hologramChildren;

    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    Material liveStreamMat;

    string hologramViewDwellTimer = nameof(hologramViewDwellTimer);

    HoloMe holoMe;

    string videoURL;
    long broadcasterID;

    public string GetVideoFileName
    {
        get
        {
            if (string.IsNullOrEmpty(videoURL))
            {
                return "";
            }
            return videoURL.Split('/').Last();
        }
    }


    bool hasPlaced;
    bool isPreRecorded;
    VideoPlayerUnity videoPlayer;

    void Start()
    {
        holoMe = new HoloMe
        {
            LoopVideo = true
        };

        placementHandler.OnPlaceDetected = PlayOnPlace;

        var focusSquareV2 = placementHandler as FocusSquareV2;
        if (focusSquareV2 != null)
        {
            focusSquareV2.HoloMe = holoMe;
        }
    }

    public void InitSession()
    {
        //print($"Init Session Code = {code}");

        if (!holoMe.Initialized)
        {
            videoPlayer = new VideoPlayerUnity();
            //videoPlayer.OnPrepared += ()=> Debug.Log("PREPARED!");
            holoMe.Init(cameraTransform, videoPlayer, audioSource, liveStreamMat);
            holoMe.PlaceVideo(new Vector3(1000, 1000, 1000)); //This is the move the hologram out of the way to not effect the fade
            holoMe.EnableAmbientLighting();
            foreach (HologramChild hologramChild in hologramChildren)
            {
                hologramChild.SetParent(holoMe.HologramTransform);
            }
            holoMe.SetScale(0.75f);
            videoPlayer.SetOnReadyEvent(() => AnalyticsController.Instance.StartTimer(hologramViewDwellTimer, $"{AnalyticKeys.KeyHologramViewPercentage} ({GetVideoFileName})"));
        }
    }

    public void PlayIfPlaced(string url, long broadcasterID)
    {
        HelperFunctions.DevLog("PLAY ON PLACE CALLED code =" + url);

        videoURL = url;
        this.broadcasterID = broadcasterID;

        if (Application.isEditor)
        {
            Debug.LogWarning($"{nameof(PlayOnPlace)} Called Editor Mode");
            PlayOnPlace(new Vector3(0, -.5f, 2.5f));
        }
        else if (hasPlaced)
        {
            PlayVideo();
        }
    }

    private void PlayOnPlace(Vector3 position)
    {
        //Debug.Log($"Play on Place called {videoCode}");
        if (!hasPlaced || Application.isEditor)
        {
            hasPlaced = true;
            PlayVideo();
        }
        foreach (HologramChild hologramChild in hologramChildren)
        {
            hologramChild.UpdateOffset(position);
        }

        holoMe.PlaceVideo(position);
    }

    private void PlayVideo()
    {
        if (holoMe.IsPaused)
        {
            holoMe.ResumeVideo();
        }
        else
        {
            holoMe.PlayVideo(videoURL);
            //holoMe.PlayVideo(HelperFunctions.PersistentDir() + videoCode + ".mp4");
            //AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPerformanceLoaded, AnalyticParameters.ParamVideoName, GetVideoFileName);
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPerformanceLoaded, new Dictionary<string, string>{ { AnalyticParameters.ParamVideoName, GetVideoFileName }, {AnalyticParameters.ParamBroadcasterUserID, broadcasterID.ToString() }});
        }
    }

    public void ForcePlay()
    {
        holoMe.PlayVideo();
    }

    /// <summary>
    /// This is required for analytics stream name tracking and sharing
    /// </summary>
    public void AssignStreamName(string streamName)
    {
        videoURL = streamName;
    }

    public void StartTrackingStream()
    {
        AnalyticsController.Instance.StartTimer(hologramViewDwellTimer, $"{AnalyticKeys.KeyHologramLiveViewTime} ({videoURL})");
    }

    public void TogglePreRecordedVideoRenderer(bool enable)
    {
        isPreRecorded = enable;
        holoMe.HologramTransform.parent.GetComponent<MeshRenderer>().enabled = enable;
    }

    public void StopVideo()
    {
        if (isPreRecorded)
        {
            float percentageViewed = Mathf.Round(Mathf.Clamp((float)(((float)AnalyticsController.Instance.GetElapsedTime(hologramViewDwellTimer) / videoPlayer.GetClipLength()) * 100), 0, 100));
            HelperFunctions.DevLog("Clip Length " + videoPlayer.GetClipLength());
            HelperFunctions.DevLog("Percentage Watched = " + percentageViewed + "%");
            AnalyticsController.Instance.StopTimer(hologramViewDwellTimer, percentageViewed, new Dictionary<string, string> { { AnalyticParameters.ParamBroadcasterUserID, broadcasterID.ToString() } });
            AnalyticsController.Instance.SendCustomEvent(percentageViewed >= 99 ? AnalyticKeys.KeyPerformanceEnded : AnalyticKeys.KeyPerformanceNotEnded, new Dictionary<string, string> { { AnalyticParameters.ParamVideoName, GetVideoFileName }, { AnalyticParameters.ParamBroadcasterUserID, broadcasterID.ToString() } });
        }
        else
        {
            AnalyticsController.Instance.StopTimer(hologramViewDwellTimer);
        }
        holoMe.StopVideo();
    }

    public void PauseVideo()
    {
        AnalyticsController.Instance.PauseTimer(hologramViewDwellTimer);
        holoMe.PauseVideo();
    }

    public void ResumeVideo()
    {
        AnalyticsController.Instance.ResumeTimer(hologramViewDwellTimer);
        holoMe.ResumeVideo();
    }

    public void SetOnPlacementUIHelperFinished(Action action)
    {
        var focusSquareV2 = placementHandler as FocusSquareV2;
        focusSquareV2.OnPlacementUIHelperFinished += action;
    }
}
