using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlHomeScreen : MonoBehaviour
{
    public enum HomeScreenPageType {
        Default,
        One,
        Two,
        Three,
        FourPlus
    }

    [SerializeField]
    PnlFetchingData pnlFetchingData;

    bool hasFetchedData;
    bool initiallaunch;

    void OnEnable() {
        if (!initiallaunch) {
            initiallaunch = true;
            return;
        }
        FetchData();
    }

    private void FetchData() {
        if (!hasFetchedData) {
             pnlFetchingData.Activate(FetchThumbnailData);
        }
    }

    private void FetchThumbnailData() {
   /*     PnlGenericLoading.DoMenuTransition(true);
        thumbnailDownloadManager.OnThumbnailsDataDownloaded -= OnThumbnailsDownloaded;
        thumbnailDownloadManager.OnThumbnailsDataDownloaded += OnThumbnailsDownloaded;
        thumbnailDownloadManager.DownloadVideoThumbnails();*/
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
