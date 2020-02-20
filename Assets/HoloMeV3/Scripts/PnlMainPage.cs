using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PnlMainPage : MonoBehaviour
{
    [SerializeField]
    PnlFetchingData pnlFetchingData;

    [SerializeField]
    RectTransform contentShowcaseThumbnails;

    [SerializeField]
    RectTransform contentUserThumbnails;

    [SerializeField]
    ThumbnailDownloadManager thumbnailDownloadManager;

    [SerializeField]
    GameObject thumbnailPrefab;

    [SerializeField]
    VerticalLayoutGroup verticalLayoutGroup;

    bool hasFetchedData;
    bool initiallaunch;

    void OnEnable()
    {
        if (!initiallaunch)
        {
            initiallaunch = true;
            return;
        }

        if (!hasFetchedData)
        {
            pnlFetchingData.Activate(FetchThumbnailData);
            hasFetchedData = true;
        }
    }

    private void FetchThumbnailData()
    {
        thumbnailDownloadManager.OnThumbnailsDataDownloaded -= OnThumbnailsDownloaded;
        thumbnailDownloadManager.OnThumbnailsDataDownloaded += OnThumbnailsDownloaded;
        thumbnailDownloadManager.DownloadVideoThumbnails();
    }

    void OnThumbnailsDownloaded()
    {
        for (int i = 0; i < thumbnailDownloadManager.videoThumbnailShowcaseJsonDatas.Count; i++)
        {
            var newThumbnail = Instantiate(thumbnailPrefab, contentShowcaseThumbnails);
            Sprite s = LoadSprite(HelperFunctions.PersistentDir() + thumbnailDownloadManager.videoThumbnailShowcaseJsonDatas[i].imageURL);
            newThumbnail.GetComponent<BtnThumbnailItem>().UpdateThumbnailData(thumbnailDownloadManager.videoThumbnailShowcaseJsonDatas[i].code, s);
        }

        for (int i = 0; i < thumbnailDownloadManager.videoThumbnailUserJsonDatas.Count; i++)
        {
            var newThumbnail = Instantiate(thumbnailPrefab, contentUserThumbnails);
            Sprite s = LoadSprite(HelperFunctions.PersistentDir() + thumbnailDownloadManager.videoThumbnailUserJsonDatas[i].imageURL);
            newThumbnail.GetComponent<BtnThumbnailItem>().UpdateThumbnailData(thumbnailDownloadManager.videoThumbnailUserJsonDatas[i].code, s);
        }

        StartCoroutine(RefreshLayoutGroup());
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }

    IEnumerator RefreshLayoutGroup()
    {
        verticalLayoutGroup.enabled = false;
        yield return new WaitForEndOfFrame();
        verticalLayoutGroup.enabled = true;
    }
}
