using UnityEngine;
using HoloMeSDK;

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
    AudioSource audioSource;

    [SerializeField]
    Material liveStreamMat;

    HoloMe holoMe;

    string videoCode;

    bool hasPlaced;

    void Start()
    {
        holoMe = new HoloMe();
        holoMe.LoopVideo = true;
        placementHandler.OnPlaceDetected = PlayOnPlace;
    }

    public void InitSession()
    {
        //print($"Init Session Code = {code}");

        if (!holoMe.Initialized)
        {
            VideoPlayerUnity videoPlayer = new VideoPlayerUnity();
            holoMe.Init(cameraTransform, videoPlayer, audioSource);
            holoMe.PlaceVideo(new Vector3(1000, 1000, 1000)); //This is the move the hologram out of the way to not effect the fade
            holoMe.EnableAmbientLighting();
            holoMe.HologramTransform.parent.GetComponent<MeshRenderer>().material = liveStreamMat;
            foreach (HologramChild hologramChild in hologramChildren)
            {
                hologramChild.SetParent(holoMe.HologramTransform);
            }
            holoMe.SetScale(0.75f);
        }
    }

    public void PlayIfPlaced(string code)
    {
        HelperFunctions.DevLog("PLAY ON PLACE CALLED code =" + code);

        videoCode = code;

        if (Application.isEditor)
        {
            Debug.LogWarning($"{nameof(PlayOnPlace)} Called Editor Mode");
            PlayOnPlace(new Vector3(0, 0, -10));
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
            holoMe.PlayVideo(videoCode);
            //            holoMe.PlayVideo(HelperFunctions.PersistentDir() + videoCode + ".mp4");
        }
    }

    public void TogglePreRecordedVideoRenderer(bool enable)
    {
        holoMe.HologramTransform.parent.GetComponent<MeshRenderer>().enabled = enable;
    }

    public void StopVideo()
    {
        holoMe.StopVideo();
    }

    public void PauseVideo()
    {
        holoMe.StopVideo();
        //holoMe.PauseVideo();
    }
    
    public void ResumeVideo()
    {
        holoMe.ResumeVideo();
    }
}
