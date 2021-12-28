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
    //filter for downloadable thumbnails 
    [SerializeField] ThumbnailPriorityScriptableObject thumbnailPriority;
    [SerializeField] int pageSize = 10;

    //ThumbnailsDataFetcher take json pages with thumbnails 
    private ThumbnailsDataFetcher thumbnailsDataFetcher;

    private bool dataLoaded;
    private bool initialized;
    private bool needRefresh;


    [Space]
    //controller uithumbnails 
    [SerializeField]
    private UIThumbnailsController _uiThumbnailsController;
    //ThumbnailWebDownloadManager need for data fetcher
    [SerializeField]
    private ThumbnailWebDownloadManager _thumbnailWebDownloadManager;

    public void SetDefaultState() {
        scrollRect.verticalNormalizedPosition = 1;
    }

    private void Awake() {
        pullRefreshController.OnRefresh += RefreshItems;
        pullRefreshController.OnReachedBottom += GetNextPage;

        InitFetcher();
        CallBacks.onStreamsContainerUpdated += DataUpdateCallBack;

        _uiThumbnailsController.OnUpdated += UIUpdated;
        //add ref to data list from fetcher for ui Thumbnails Controller
        _uiThumbnailsController.SetStreamJsonData(thumbnailsDataFetcher.GetDataList());
        _uiThumbnailsController.OnPlayFromUser += OnPlayCallBack;

        CallBacks.onSignOut += ClearData;
    }

    private void InitFetcher() {
        if (thumbnailsDataFetcher != null)
            return;

        thumbnailsDataFetcher =
            new ThumbnailsDataFetcher(thumbnailPriority.ThumbnailPriority,
            _thumbnailWebDownloadManager, pageSize: pageSize);

        thumbnailsDataFetcher.OnAllDataLoaded += AllDataLoaded;
    }

    private void DataUpdateCallBack() {
        if (!isActiveAndEnabled)
            return;
        initialized = true;
        pullRefreshController.StopBottomRefreshing = false;
        _uiThumbnailsController.UpdateData();
    }

    private void UIUpdated() {
        EndingUIUpdate();
    }

    private void RefreshItems() {
        needRefresh = false;
        Resources.UnloadUnusedAssets();
        dataLoaded = false;
        _uiThumbnailsController.LockToPressElements();
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
        _uiThumbnailsController.RemoveUnnecessary();
    }

    private void ClearData() {
        thumbnailsDataFetcher.ClearData();
        _uiThumbnailsController.UpdateData();
        initialized = false;
        SetDefaultState();
        needRefresh = true;
    }

    private void OnPlayCallBack(string user) {
        HomeScreenConstructor.OnActivated?.Invoke(false);
        SettingsConstructor.OnActivated?.Invoke(false);
        MenuConstructor.OnActivated?.Invoke(false);
        ChangeUsernameConstructor.OnActivated?.Invoke(false);
        ChangePasswordConstructor.OnActivated?.Invoke(false);
    }

    private void EndingUIUpdate() {
        if (!isActiveAndEnabled)
            return;
        pullRefreshController.EndRefreshing();
    }

    private void OnEnable() {
        SetDefaultState();
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
        if (thumbnailsDataFetcher != null) {
            thumbnailsDataFetcher.OnAllDataLoaded -= AllDataLoaded;
        }
    }
}
