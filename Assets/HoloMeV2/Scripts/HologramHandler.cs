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
    LogoCanvas logoCanvas;

    [SerializeField]
    AudioSource audioSource;

    HoloMe holoMe;

    string videoCode;

    bool hasPlaced;

    void Start()
    {
        holoMe = new HoloMe();
        holoMe.LoopVideo = true;
        placementHandler.OnPlaceDetected = PlayOnPlace;
    }

    public void InitSession(string code)
    {
        //print($"Init Session Code = {code}");

        if (!holoMe.Initialized)
        {
            holoMe.Init(cameraTransform);
            holoMe.UseAudioSource(audioSource);
            holoMe.PlaceVideo(new Vector3(1000, 1000, 1000)); //This is the move the hologram out of the way to not effect the fade
            holoMe.EnableAmbientLighting();
            logoCanvas.SetParent(holoMe.HologramTransform);
        }

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
        logoCanvas.UpdateOffset(position);
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
            holoMe.PlayVideo(HelperFunctions.PersistentDir() + videoCode + ".mp4");
        }
    }

    public void StopVideo()
    {
        holoMe.StopVideo();
    }

    public void PauseVideo()
    {
        holoMe.PauseVideo();
    }

    public void ResumeVideo()
    {
        holoMe.ResumeVideo();
    }
}
