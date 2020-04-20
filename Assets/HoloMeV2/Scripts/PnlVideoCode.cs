using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class PnlVideoCode : MonoBehaviour
{
    [SerializeField]
    ServerDataHandler serverDataHandler;

    [SerializeField]
    RectTransform codeBox;
    CanvasGroup codeCanvasGroup;

    [SerializeField]
    RectTransform downloadFailRT;
    [SerializeField]
    CanvasGroup downloadFailedCanvasGroup;

    [SerializeField]
    Image imgLoading;
    [SerializeField]
    CanvasGroup imgLoadingCanvasGroup;

    [SerializeField]
    AnimatedTransition animatedTransition;

    [SerializeField]
    TextMeshProUGUI txtIncorrectCode;

    [SerializeField]
    TextMeshProUGUI txtEnterCode;

    [SerializeField]
    TextMeshProUGUI txtDownloadPercentage;

    [SerializeField]
    DotManager dotManager;

    [SerializeField]
    GameObject btnBurger;

    [SerializeField]
    PnlViewingExperience pnlViewingExperience;

    [SerializeField]
    PnlFetchingData pnlFetchingData;

    [SerializeField]
    PnlGenericError pnlGenericError;

    [SerializeField]
    PnlPostRecord pnlPostRecord;

    //[SerializeField]
    //CanvasGroup imgSampleCodesCanvasGroup;

    [SerializeField]
    PnlMainPage pnlMainPage;

    enum DataDownloadState { DownloadingJson, DownloadingLogo, DownloadingVideo };
    DataDownloadState dataDownloadState = DataDownloadState.DownloadingVideo;

    VideoJsonData videoJsonData;
    string validCode;

    private void Awake()
    {
        dotManager.OnPassCodeEntered += CheckMatch;
        serverDataHandler.AssignDownloadProgressAction(x => txtDownloadPercentage.text = ((int)(x * 100)).ToString() + "%");
        serverDataHandler.AssignDownloadCompleteAction(() =>
        {
            OnDownloadComplete();
        });
        serverDataHandler.AssignDownloadFailedAction(() => DisplayDownloadFailedMessage());

        codeCanvasGroup = codeBox.GetComponent<CanvasGroup>();
    }

    private void OnDownloadComplete()
    {
        switch (dataDownloadState)
        {
            case DataDownloadState.DownloadingJson:
                videoJsonData = JsonParser.CreateFromJSON<VideoJsonData>(JsonParser.ParseFileName(validCode));
                if (!DoesVideoExist(videoJsonData.videoCode))//Check if the code in the JSON file exists as a video on the server
                {
                    break;
                }
                validCode = videoJsonData.videoCode;
                dataDownloadState = DataDownloadState.DownloadingLogo;
                serverDataHandler.DownloadSupplementaryFile(videoJsonData.logoImage);
                break;
            case DataDownloadState.DownloadingLogo:
                dataDownloadState = DataDownloadState.DownloadingVideo;
                serverDataHandler.DownloadOrPlayLocalVideo(videoJsonData.videoCode);
                break;
            case DataDownloadState.DownloadingVideo:
                ActivateViewingExperience();
                break;
        }
    }

    bool DoesVideoExist(string code)
    {
        if (serverDataHandler.GetVideoData(code) == null && !HelperFunctions.DoesFileExist(code)) //Check if code is valid before proceeding
        {
            ShowErrorVideoCodeDoesntExist();
            return false;
        }
        return true;
    }

    void ShowErrorVideoCodeDoesntExist()
    {
        //animatedTransition.DoMenuTransition(false);
        pnlGenericError.ActivateSingleButton(message: "Video Code Not Found", onBackPress: () => gameObject.SetActive(true));
    }

    void ShowErrorFilesMissing()
    {
        //animatedTransition.DoMenuTransition(false);
        pnlGenericError.ActivateSingleButton(message: "Missing Related Files", onBackPress: () => gameObject.SetActive(true));
    }

    private void ActivateViewingExperience()
    {
        animatedTransition.DoMenuTransition(false);
        var currentCode = validCode;
        pnlViewingExperience.ActivateSelf(currentCode, videoJsonData);
        pnlMainPage.GetComponent<AnimatedTransition>().DoMenuTransition(false);
    }

    private void DisplayDownloadFailedMessage()
    {
        codeCanvasGroup.interactable = false;
        codeCanvasGroup.DOFade(0, .5f);
        ToggleSampleCodeSection(false, true);
        imgLoadingCanvasGroup.DOFade(0, .5f);
        downloadFailRT.gameObject.SetActive(true);
        downloadFailedCanvasGroup.DOFade(1, .5f).OnComplete(
            () =>
            {
                codeCanvasGroup.DOFade(1, .5f).SetDelay(3)
                    .OnStart(
                        () =>
                        {
                            dotManager.ClearText();
                            downloadFailedCanvasGroup.DOFade(0, .5f).OnComplete(
                                () =>
                                {
                                    imgLoading.gameObject.SetActive(false);
                                    imgLoadingCanvasGroup.alpha = 0;

                                    downloadFailRT.gameObject.SetActive(false);
                                    btnBurger.SetActive(true);
                                    ToggleSampleCodeSection(true, true);
                                });
                        })
                    .OnComplete(
                        () =>
                        {
                            codeCanvasGroup.interactable = true;
                            codeBox.gameObject.SetActive(true);
                        });
            });
    }

    private void ResetPanel()
    {
        txtDownloadPercentage.text = "";
        btnBurger.SetActive(true);

        imgLoading.gameObject.SetActive(false);
        imgLoadingCanvasGroup.alpha = 0;

        codeBox.gameObject.SetActive(true);
        ToggleCodeInputSection(true);

        downloadFailRT.gameObject.SetActive(false);
        downloadFailedCanvasGroup.alpha = 0;

        ToggleSampleCodeSection(true);

        dotManager.ToggleBoxSprites(false);
        dotManager.ClearText();
        ToggleIncorrectCodeText(false);
    }

    public void Open()
    {
        ResetPanel();

        //if (validCode == null)
        //{
        gameObject.SetActive(true);
        TryFetchVideoData();
        return;
        //}

        //if (videoJsonData != null)
        //{
        //    if (serverDataHandler.GetVideoData(videoJsonData.videoCode) == null)
        //    {
        //        gameObject.SetActive(true);
        //        TryFetchVideoData();
        //        return; //Return if JSON file's video doesn't exist
        //    }
        //}

        //pnlViewingExperience.ActivateSelf(validCode, videoJsonData);
        //pnlMainPage.GetComponent<AnimatedTransition>().DoMenuTransition(false);
    }

    public void OpenWithCode(string code)
    {
        ResetPanel();
        pnlViewingExperience.StopExperience();
        gameObject.SetActive(true);
        ToggleCodeInputSection(false);
        ToggleSampleCodeSection(false);

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogError("Code was empty");
            ToggleCodeInputSection(true);
        }

        if (HelperFunctions.DoesFileExist(code))
        {
            EnableLoadingAnimation();
            StartCoroutine(DelayCodeCheck(code));
        }
        else
        {
            CheckMatch(code);
        }
    }

    void ToggleCodeInputSection(bool enable)
    {
        codeCanvasGroup.alpha = enable ? 1 : 0;
    }

    void ToggleSampleCodeSection(bool enable, bool fade = false)
    {
        //imgSampleCodesCanvasGroup.blocksRaycasts = enable;
        //imgSampleCodesCanvasGroup.interactable = enable;
        //if (fade)
        //{
        //    imgSampleCodesCanvasGroup.DOFade(enable ? 1 : 0, .5f);
        //}
        //else
        //{
        //    imgSampleCodesCanvasGroup.alpha = enable ? 1 : 0;
        //}
    }

    IEnumerator DelayCodeCheck(string code)
    {
        yield return new WaitForSeconds(3);
        CheckMatch(code);
    }

    public void ToggleIncorrectCodeText(bool show)
    {
        txtIncorrectCode.gameObject.SetActive(show);
        txtEnterCode.gameObject.SetActive(!show);
    }

    public bool TryFetchVideoData(UnityAction Callback = null)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (!serverDataHandler.DataReceived)
            {
                //animatedTransition.DoMenuTransition(false);

                if (Callback != null)
                {
                    pnlFetchingData.Activate(Callback);
                }
                else
                {
                    pnlFetchingData.gameObject.SetActive(true);
                }
                return false;
            }
        }
        return true;
    }

    public void CheckMatch(string code)
    {
        code = code.ToLower(); //Fixed capitilisation of first letter if applicable

        if (HelperFunctions.DoesFileExist(code))
        {
            if (HelperFunctions.IsFileJSON(code)) //If JSON check if all files exist
            {
                if (HelperFunctions.AllJSONFilesLocallyAvailable(code))
                {
                    StartVideoPlayback(code);
                    return;
                }
                else
                {
                    ShowErrorFilesMissing();
                    return;
                }
            }
            else
            {
                StartVideoPlayback(code); //Play if video
                return;
            }
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //print("NO INTERNET");
            serverDataHandler.InvokeDownloadFailedEvent();
            return;
        }

        //Fetch Video Data if failed early one and internet access if available, rechecks code after data has been received
        if (!TryFetchVideoData(() => CheckMatch(code)))
        {
            return;
        }

        var videoData = serverDataHandler.GetVideoData(code);
        if (videoData == null)
        {
            PrefsCodeHistory.RemoveCode(code);
            StartCoroutine(WrongCodeRoutine());
        }
        else
        {
            StartVideoPlayback(code);
        }
    }

    private void StartVideoPlayback(string code)
    {
        Analytics.CustomEvent("CodeEntered", new Dictionary<string, object> { { "Code", code } });

        pnlPostRecord.Code = code;

        var videoData = serverDataHandler.GetVideoData(code);
        if (videoData != null) //This is the path for when online
        {
            if (videoData.FileName.Contains(HelperFunctions.EXTJSON))
            {
                dataDownloadState = DataDownloadState.DownloadingJson;
            }
            else
            {
                dataDownloadState = DataDownloadState.DownloadingVideo;
            }
        }
        else //Offline path
        {
            if (HelperFunctions.IsFileJSON(code))
            {
                dataDownloadState = DataDownloadState.DownloadingJson;
            }
            else
            {
                dataDownloadState = DataDownloadState.DownloadingVideo;
            }

            if (!DoesVideoExist(code)) //Show missing file error if file doesn't exist, this isn't really neccessary as files are checked beforehand but for the poup it's here
            {
                Debug.LogError("Code wasn't found in " + nameof(StartVideoPlayback) + " " + code);
                return;
            }
        }

        videoJsonData = null;
        validCode = code;
        PrefsCodeHistory.UpdatePrefsString(code);

        ToggleSampleCodeSection(false, true);
        EnableLoadingAnimation();

        codeCanvasGroup.interactable = false;
        codeCanvasGroup.DOFade(0, .5f).OnComplete(() =>
        {
            pnlViewingExperience.StopExperience();
            codeBox.gameObject.SetActive(false);
            codeCanvasGroup.interactable = true;
            serverDataHandler.DownloadOrPlayLocalVideo(code);
        });
    }

    private void EnableLoadingAnimation()
    {
        btnBurger.SetActive(false);
        imgLoading.gameObject.SetActive(true);
        imgLoadingCanvasGroup.DOFade(1, .5f);
    }

    public void IncorrectCodeShake()
    {
        txtIncorrectCode.GetComponent<RectTransform>().DOShakeAnchorPos(.25f, new Vector3(40, 0, 0), 80);
    }

    IEnumerator WrongCodeRoutine()
    {
        ToggleCodeInputSection(true);
        ToggleSampleCodeSection(true);
        ToggleIncorrectCodeText(true);
        IncorrectCodeShake();
        dotManager.ToggleBoxSprites(true);
        yield return new WaitForSeconds(2);
        dotManager.ToggleBoxSprites(false);
        dotManager.ClearText();
        dotManager.ActivateTextField();
        ToggleIncorrectCodeText(false);
    }

    private void OnDisable()
    {
        ResetPanel();
    }
}
