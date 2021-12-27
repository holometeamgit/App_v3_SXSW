using UnityEngine;
using UnityEngine.UI;
using NatShare;
using DG.Tweening;
using UnityEngine.Video;
using Beem.Video;

public class PnlPostRecord : MonoBehaviour {
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
    private AnimatedTransition _animatedTransition;

    [Space]
    [SerializeField]
    private HologramHandler _hologramHandler;

    static string lastRecordingPath;
    public static string LastRecordingPath { get { return lastRecordingPath; } }
    private Texture2D screenShot;
    private bool screenshotWasTaken;
    public string Code { private get; set; }

    VideoPlayer videoPlayer;
    VideoPlayer VideoPlayer {
        get {

            if (!videoPlayer) {
                videoPlayer = imgPreview.GetComponent<VideoPlayer>();
            }
            return videoPlayer;
        }
    }

    private VideoPlayerController _videoPlayerController;
    private VideoPlayerController videoPlayerController {
        get {

            if (_videoPlayerController == null) {
                _videoPlayerController = FindObjectOfType<VideoPlayerController>();
            }

            return _videoPlayerController;
        }
    }

    public void ActivatePostVideo(string lastRecordPath) {
        gameObject.SetActive(true);
        HelperFunctions.DevLog("Post record video activate called");
        screenshotWasTaken = false;
        btnPreview.gameObject.SetActive(true);

        imgPreview.texture = renderTexture;

        VideoPlayer.enabled = true;
        VideoPlayer.url = lastRecordPath;
        VideoPlayer.Play();

        Activate(null, lastRecordPath);
    }

    public void ActivatePostScreenshot(Sprite sprite, Texture2D screenshotTexture, string lastRecordPath) {
        gameObject.SetActive(true);
        HelperFunctions.DevLog("Post record screenshot activate called");
        VideoPlayer.enabled = false;
        screenshotWasTaken = true;
        screenShot = screenshotTexture;
        btnPreview.gameObject.SetActive(false);
        Activate(sprite, lastRecordPath);
    }

    private void Activate(Sprite sprite, string lastRecordPath) {
        imgSavingCanvasGroup = imgSaving.GetComponent<CanvasGroup>();
        imgDownloadSuccessCanvasGroup = imgDownloadSuccess.GetComponent<CanvasGroup>();
        imgSavingCanvasGroup.alpha = 0;
        imgSaving.gameObject.SetActive(false);
        imgDownloadSuccessCanvasGroup.alpha = 0;
        imgDownloadSuccess.gameObject.SetActive(false);
        btnDownload.interactable = true;
        btnPreview.interactable = true;


        if (sprite != null)
            imgPreview.texture = sprite.texture;

        lastRecordingPath = lastRecordPath;
    }

    /// <summary>
    /// Close Post record
    /// </summary>
    public void Close() {
        PostRecordARConstructor.OnDeactivated?.Invoke();
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        videoPlayerController?.OnResume();
        _animatedTransition.DoMenuTransition(false);
    }

    public void Share() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareHologramMediaPressed, AnalyticParameters.ParamVideoName, _hologramHandler.GetVideoFileName);
        if (screenshotWasTaken) {
            ShareScreenshot();
        } else {
            ShareVideo();
        }
    }

    public void FadeToggleControls(bool show) {
        if (show)
            safeAreaContent.gameObject.SetActive(true);

        safeAreaContent.DOFade(show ? 1 : 0, 0.5f).OnComplete(() => { if (!show) { safeAreaContent.gameObject.SetActive(false); } });
        safeAreaContent.interactable = show;
    }

    #region Video Functions
    public void ShareVideo() {
        if (!string.IsNullOrEmpty(lastRecordingPath)) {
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyVideoShared, AnalyticParameters.ParamVideoName, _hologramHandler.GetVideoFileName);

            new NativeShare().AddFile(lastRecordingPath).Share();
        } else {
            Debug.LogError("Record path was empty");
        }
    }
    #endregion

    public void ShareScreenshot() {
        if (screenShot != null) {
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeySnapshotShared, AnalyticParameters.ParamVideoName, _hologramHandler.GetVideoFileName);

            new NativeShare().AddFile(screenShot).Share();
        } else {
            Debug.LogError("Screenshot was null");
        }
    }

}
