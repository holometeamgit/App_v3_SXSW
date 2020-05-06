using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PnlFeed : MonoBehaviour
{
    [SerializeField]
    GameObject pnlLoading;

    [SerializeField]
    S3Handler s3Handler;

    [SerializeField]
    VideoPlayer videoPlayer;

    [SerializeField]
    Button btnRight;

    [SerializeField]
    Button btnLeft;

    [SerializeField]
    GameObject GenericLoadingGO;

    [SerializeField]
    PnlGenericError pnlGenericError;

    public static ServerFileData feedData;
    public static string FeedJsonName = "FeedVideo.json";

    int incrementValue = -1;

    FeedVideosCollection feedVideosCollection;

    bool feedDataRecieved;

    public void Activate()
    {
        gameObject.SetActive(true);
        pnlLoading.gameObject.SetActive(true);
        s3Handler.DownloadGeneric(feedData.FileName, feedData, OnDataReturned);
        btnRight.onClick.AddListener(() => ChangeURLIndex(false));
        btnLeft.onClick.AddListener(() => ChangeURLIndex(true));

        videoPlayer.errorReceived += (videoplayer, errorString) => pnlGenericError.ActivateSingleButton("Playback Error", "Please check internet connectivity", "Try Again", () => s3Handler.DownloadGeneric(feedData.FileName, feedData, OnDataReturned));
    }

    void OnDataReturned(bool success)
    {
        if (success)
        {
            pnlLoading.GetComponent<AnimatedTransition>().DoMenuTransition(false);
            if (!feedDataRecieved)
            {
                feedVideosCollection = JsonUtility.FromJson<FeedVideosCollection>(JsonParser.ParseFileName(feedData.FileName));
                ChangeURLIndex(false);
                feedDataRecieved = true;
            }
        }
        else
        {
            pnlGenericError.ActivateSingleButton("Error", "Please check internet connectivity", "Try Again", () => s3Handler.DownloadGeneric(feedData.FileName, feedData, OnDataReturned));
            Debug.LogError("Feed json failed to download " + feedData.FileName);
        }
    }

    private void ChangeURLIndex(bool decrement)
    {
        int feedCount = feedVideosCollection.feedVideos.Length;
        int highestIndex = feedCount - 1;

        if (feedCount == 0)
        {
            return;
        }

        incrementValue += decrement ? -1 : 1;

        if (incrementValue > highestIndex)
            incrementValue = 0;
        if (incrementValue < 0)
            incrementValue = highestIndex;

        UpdateVideoURL(feedVideosCollection.feedVideos[incrementValue].URL);
    }

    private void UpdateVideoURL(string url)
    {
        videoPlayer.url = url;
    }

    private void Update()
    {
        GenericLoadingGO?.SetActive(!videoPlayer.isPrepared);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            pnlGenericError.ActivateSingleButton("No Internet Access", "Please check internet connectivity", "Try Again", () => s3Handler.DownloadGeneric(feedData.FileName, feedData, OnDataReturned));
        }
    }
}

[System.Serializable]
public struct FeedVideosCollection
{
    public FeedVideos[] feedVideos;
}

[System.Serializable]
public struct FeedVideos
{
    public string URL;
}
