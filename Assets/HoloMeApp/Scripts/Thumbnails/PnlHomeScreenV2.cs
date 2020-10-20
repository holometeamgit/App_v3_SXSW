using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PnlHomeScreenV2 : MonoBehaviour
{

    [SerializeField] UIPullRefreshScrollController pullRefreshController;
    [SerializeField] UIThumbnailsController uiThumbnailsController;
    [SerializeField] ThumbnailPriorityScriptableObject thumbnailPriority;
    [SerializeField] ThumbnailWebDownloadManager thumbnailWebDownloadManager;
    [SerializeField] PnlEventPurchaser pnlEventPurchaser;


    [Space]
    [SerializeField] int pageSize = 10;

    private ThumbnailsDataFetcher thumbnailsDataFetcher;

    bool dataLoaded;
    bool initialized;

    public UnityEvent OnPlay;

    private void Awake() {
        pullRefreshController.OnRefresh += RefreshItems;
        pullRefreshController.OnReachedBottom += GetNextPage;

        thumbnailsDataFetcher =
            new ThumbnailsDataFetcher(thumbnailPriority.ThumbnailPriority,
            thumbnailWebDownloadManager, pageSize: pageSize );

        thumbnailsDataFetcher.OnAllDataLoaded += AllDataLoaded;
        thumbnailsDataFetcher.OnDataUpdated += DataUpdateCallBack;

        uiThumbnailsController.OnUpdated += UIUpdated;
        uiThumbnailsController.SetStreamJsonData(thumbnailsDataFetcher.GetDataList());
        uiThumbnailsController.OnPlay += OnPlayCallBack;
    }

    private void DataUpdateCallBack() {
        initialized = true;
        pullRefreshController.StopBottomRefreshing = false;
        uiThumbnailsController.UpdateData();
    }

    private void UIUpdated() {
        StartCoroutine(EndingUIUpdate());
    }

    private void RefreshItems() {
        Debug.Log("RefreshItems");
        dataLoaded = false;
        //pullRefreshController.StopBottomRefreshing = false;
        thumbnailsDataFetcher.RefreshData();
    }

    private void GetNextPage() {
        if (!initialized)
            RefreshItems();
        else {
            if (!dataLoaded)
                thumbnailsDataFetcher.GetNextPage();
        }
    }

    private void AllDataLoaded() {
        Debug.Log(" AllDataLoaded Data loaded");
        dataLoaded = true;
        pullRefreshController.StopBottomRefreshing = true;
        pullRefreshController.EndRefreshing();
        uiThumbnailsController.RemoveUnnecessary();
    }

    private void OnDisable() {
        StopAllCoroutines();
        pullRefreshController.EndRefreshing();
    }

    private void OnPlayCallBack(StreamJsonData.Data data) {
        OnPlay.Invoke();
        pnlEventPurchaser.Show(data);
        //TODO подписаться на то что купили, если купили, то обновить данные для кокретно thu через uithucontroller
    }

    IEnumerator EndingUIUpdate() {
        pullRefreshController.RefreshLayout();
        yield return null;
        //yield return new WaitForSeconds(0.1f);
        Debug.Log("IEnumerator EndingUIUpdate");
        pullRefreshController.EndRefreshing();
    }
}
