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

    public static ServerFileData feedData;
    public static string FeedJsonName = "FeedVideo.json";

    int incrementValue = -1;

    FeedVideosCollection feedVideosCollection;
    public void Activate()
    {
        gameObject.SetActive(true);
        pnlLoading.gameObject.SetActive(true);
        s3Handler.DownloadGeneric(feedData.FileName, feedData, OnDataReturned);
        btnRight.onClick.AddListener(() => ChangeURLIndex(false));
        btnLeft.onClick.AddListener(() => ChangeURLIndex(true));
    }

    void OnDataReturned(bool success)
    {
        if (success)
        {
            pnlLoading.GetComponent<AnimatedTransition>().DoMenuTransition(false);
            feedVideosCollection = JsonUtility.FromJson<FeedVideosCollection>(JsonParser.ParseFileName(feedData.FileName));
            ChangeURLIndex(false);
        }
        else
        {
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
