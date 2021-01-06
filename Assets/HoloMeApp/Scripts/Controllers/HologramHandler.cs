using UnityEngine;
using HoloMeSDK;
using System.Linq;
using System;

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

    public void PlayIfPlaced(string url)
    {
        HelperFunctions.DevLog("PLAY ON PLACE CALLED code =" + url);

        videoURL = url;

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
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPerformanceLoaded, AnalyticParameters.ParamVideoName, GetVideoFileName);
        }
    }

    /// <summary>
    /// This is required for analytics stream name tracking
    /// </summary>
    public void AssignStreamName(string streamName)
    {
        videoURL = streamName;
    }

    public void TogglePreRecordedVideoRenderer(bool enable)
    {
        holoMe.HologramTransform.parent.GetComponent<MeshRenderer>().enabled = enable;
    }

    public void StopVideo()
    {
        float percentageViewed = Mathf.Round(Mathf.Clamp((float)(((float)AnalyticsController.Instance.GetElapsedTime(hologramViewDwellTimer) / videoPlayer.GetClipLength()) * 100), 0, 100));
        print("Clip Length " + videoPlayer.GetClipLength());
        print("Percentage Watched = " + percentageViewed + "%");
        AnalyticsController.Instance.StopTimer(hologramViewDwellTimer, percentageViewed);
        AnalyticsController.Instance.SendCustomEvent(percentageViewed >=99 ? AnalyticKeys.KeyPerformanceEnded: AnalyticKeys.KeyPerformanceNotEnded, AnalyticParameters.ParamVideoName, GetVideoFileName);
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
