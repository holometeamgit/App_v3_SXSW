using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Beem.SSO;
using System.Threading.Tasks;

public class PnlHomeScreenV2 : MonoBehaviour {
    [SerializeField] ScrollRect scrollRect;
    //Pull refresh
    [SerializeField] UIPullRefreshScrollController pullRefreshController;
    //controller uithumbnails 
    [SerializeField] UIThumbnailsController uiThumbnailsController;
    //filter for downloadable thumbnails 
    [SerializeField] ThumbnailPriorityScriptableObject thumbnailPriority;
    //ThumbnailWebDownloadManager need for data fetcher  
    [SerializeField] ThumbnailWebDownloadManager thumbnailWebDownloadManager;

    [Space]
    [SerializeField] int pageSize = 10;

    private const int DELAY_TIME_REFRESH_LAYOUT = 50;

    //ThumbnailsDataFetcher take json pages with thumbnails 
    private ThumbnailsDataFetcher thumbnailsDataFetcher;

    bool dataLoaded;
    bool initialized;
    bool needRefresh;

    public UnityEvent OnPlay;

    public UnityEvent OnAllDataLoaded;

    public void SetDefaultState() {
        scrollRect.verticalNormalizedPosition = 1;
    }

    private void Awake() {
        pullRefreshController.OnRefresh += RefreshItems;
        pullRefreshController.OnReachedBottom += GetNextPage;

        InitFetcher();
        CallBacks.onStreamsContainerUpdated += DataUpdateCallBack;

        uiThumbnailsController.OnUpdated += UIUpdated;
        //add ref to data list from fetcher for ui Thumbnails Controller
        uiThumbnailsController.SetStreamJsonData(thumbnailsDataFetcher.GetDataList());
        uiThumbnailsController.OnPlayFromUser += OnPlayCallBack;

        CallBacks.onSignOut += ClearData;
    }

    private void InitFetcher() {
        if (thumbnailsDataFetcher != null)
            return;

        thumbnailsDataFetcher =
            new ThumbnailsDataFetcher(thumbnailPriority.ThumbnailPriority,
            thumbnailWebDownloadManager, pageSize: pageSize);

        thumbnailsDataFetcher.OnAllDataLoaded += AllDataLoaded;
    }

    private void DataUpdateCallBack() {
        if (!isActiveAndEnabled)
            return;
        initialized = true;
        pullRefreshController.StopBottomRefreshing = false;
        uiThumbnailsController.UpdateData();
    }

    private void UIUpdated() {
        EndingUIUpdate();
    }

    private void RefreshItems() {
        needRefresh = false;
        Resources.UnloadUnusedAssets();
        dataLoaded = false;
        uiThumbnailsController.LockToPressElements();
        InitFetcher();
        thumbnailsDataFetcher.RefreshData();
    }

    private void GetNextPage() {
        if (!initialized) {
            RefreshItems();
        } else if (!dataLoaded) {
            InitFetcher();
            thumbnailsDataFetcher.GetNextPage();
        }
    }

    private void AllDataLoaded() {
        dataLoaded = true;
        pullRefreshController.StopBottomRefreshing = true;
        pullRefreshController.EndRefreshing();
        uiThumbnailsController.RemoveUnnecessary();
        OnAllDataLoaded.Invoke();//temp
    }

    private void ClearData() {
        thumbnailsDataFetcher.ClearData();
        uiThumbnailsController.UpdateData();
        initialized = false;
        SetDefaultState();
        needRefresh = true;
    }

    private void OnPlayCallBack(string user) {
        OnPlay.Invoke();
    }

    private void EndingUIUpdate() {
        if (!isActiveAndEnabled)
            return;
        pullRefreshController.EndRefreshing();
    }

    private void OnEnable() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyHomeScreen);
        pullRefreshController.EndRefreshing();
        if (needRefresh)
            RefreshItems();
    }

    private void OnDisable() {
        StopAllCoroutines();
        pullRefreshController.EndRefreshing();
    }

    private void OnDestroy() {
        CallBacks.onStreamsContainerUpdated -= DataUpdateCallBack;
        if(thumbnailsDataFetcher != null) {
            thumbnailsDataFetcher.OnAllDataLoaded -= AllDataLoaded;
        }
    }
}
