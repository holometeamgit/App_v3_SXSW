using UnityEngine;
using UnityEngine.UI;
using NatShare;
using System.Collections;
using DG.Tweening;
using UnityEngine.Video;

public class PnlPostRecord : MonoBehaviour
{
    [SerializeField]
    RawImage imgPreview;

    [SerializeField]
    RenderTexture renderTexture;

    [SerializeField]
    Image imgSaving;
    CanvasGroup imgSavingCanvasGroup;

    [SerializeField]
    Image imgDownloadSuccess;
    CanvasGroup imgDownloadSuccessCanvasGroup;

    [SerializeField]
    CanvasGroup safeAreaContent;

    [SerializeField]
    Button btnPreview;

    [SerializeField]
    Button btnDownload;

    [SerializeField]
    Button btnShare;

    [SerializeField]
    PermissionGranter permissionGranter;

    [SerializeField]
    GameObject pnlGenericError;

    static string lastRecordingPath;
    public static string LastRecordingPath { get { return lastRecordingPath; } }
    private Texture2D screenShot;
    private bool screenshotWasTaken;
    public string Code { private get; set; }
    const string ShareMessage = "Experience this hologram yourself using the HoloMe app by entering code ";

    VideoPlayer videoPlayer;
    VideoPlayer VideoPlayer {
        get {

            if (!videoPlayer) {
                videoPlayer = imgPreview.GetComponent<VideoPlayer>();
            }
            return videoPlayer;
        }
    }

    private void Start()
    {
        //btnShare.onClick.AddListener(Share);

        imgSavingCanvasGroup = imgSaving.GetComponent<CanvasGroup>();
        imgDownloadSuccessCanvasGroup = imgDownloadSuccess.GetComponent<CanvasGroup>();
    }

    public void ActivatePostVideo(string lastRecordPath)
    {
        HelperFunctions.DevLog("Post record video activate called");
        screenshotWasTaken = false;
        btnPreview.gameObject.SetActive(true);

        imgPreview.texture = renderTexture;

        VideoPlayer.enabled = true;
        VideoPlayer.url = lastRecordPath;
        VideoPlayer.Play();

        Activate(null, lastRecordPath);
    }

    public void ActivatePostScreenshot(Sprite sprite, Texture2D screenshotTexture, string lastRecordPath)
    {
        HelperFunctions.DevLog("Post record screenshot activate called");
        VideoPlayer.enabled = false;
        screenshotWasTaken = true;
        screenShot = screenshotTexture;
        btnPreview.gameObject.SetActive(false);
        Activate(sprite, lastRecordPath);
    }

    private void Activate(Sprite sprite, string lastRecordPath)
    {
        imgSavingCanvasGroup.alpha = 0;
        imgSaving.gameObject.SetActive(false);
        imgDownloadSuccessCanvasGroup.alpha = 0;
        imgDownloadSuccess.gameObject.SetActive(false);
        btnDownload.interactable = true;
        btnShare.interactable = true;
        btnPreview.interactable = true;
        gameObject.SetActive(true);

        if (sprite != null)
            imgPreview.texture = sprite.texture;

        lastRecordingPath = lastRecordPath;
    }

    public void Share()
    {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareVideoPressed);
        if (screenshotWasTaken)
        {
            ShareScreenshot();
        }
        else
        {
            ShareVideo();
        }
    }

    public void FadeToggleControls(bool show)
    {
        if (show)
            safeAreaContent.gameObject.SetActive(true);

        safeAreaContent.DOFade(show ? 1 : 0, 0.5f).OnComplete(() => { if (!show) { safeAreaContent.gameObject.SetActive(false); } });
        safeAreaContent.interactable = show;
    }

    #region Video Functions
    public void ShareVideo()
    {
        if (!string.IsNullOrEmpty(lastRecordingPath))
        {
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyVideoShared);
            using (var payload = new SharePayload())
            {
                //payload.AddText(ShareMessage + Code);
                payload.AddMedia(lastRecordingPath);
            }
        }
        else
        {
            Debug.LogError("Record path was empty");
        }
    }
    #endregion

    public void ShareScreenshot()
    {
        if (screenShot != null)
        {
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeySnapshotShared);
            using (var payload = new SharePayload())
            {
                payload.AddImage(screenShot);
            }
        }
        else
        {
            Debug.LogError("Screenshot was null");
        }
    }

}
