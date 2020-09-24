using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;

public class PnlHomeScreen : MonoBehaviour {
    public enum HomeScreenPageType {
        Default,
        One,
        Two,
        Three,
        FourPlus
    }

    [SerializeField] float tymeToNextRefresh = 60;

    [SerializeField]
    PnlViewingExperience pnlViewingExperience;

    [SerializeField]
    PnlStreamOverlay pnlStreamOverlay;

    [SerializeField]
    AgoraController agoraController;

    [SerializeField]
    HomeScreenLoader homeScreenLoader;

    [SerializeField]
    RectTransform contentShowcaseThumbnails;

    [SerializeField]
    RectTransform contentUserThumbnails;

    [SerializeField]
    GameObject thumbnailShowcasePrefab;

    [SerializeField]
    GameObject thumbnailPrefab;

    [SerializeField] int showcaseCount = 2;

    [SerializeField] bool fetchDataOnFirstEnable; //TODO: this is needed before switching to beem, then everything will be deleted

    [SerializeField] ScrollRect scrollRect;
    private float lastScrollPosition = 1;
    private bool firstLoading = true;

    [SerializeField] UnityEvent OnThumbnailClick;

    private List<GameObject> thumbnails;

    bool initiallaunch;

    void OnEnable() {

        //Clear();
        if (!initiallaunch) {
            initiallaunch = true;
            homeScreenLoader.OnDataFetched.AddListener(DataFetched);
            if (!fetchDataOnFirstEnable)
                return;
        }
        homeScreenLoader.FetchData();
    }

    private void AddThumbnail(bool isShowcase, Texture texture, StreamJsonData.Data data, bool isLive) {

        RectTransform contentThumbnails = isShowcase ? contentShowcaseThumbnails : contentUserThumbnails;
        GameObject newThumbnail;
        if (thumbnailShowcasePrefab == null)
            newThumbnail = Instantiate(thumbnailPrefab, contentThumbnails);
        else
            newThumbnail = Instantiate(isShowcase ? thumbnailShowcasePrefab : thumbnailPrefab, contentThumbnails); //TODO: this (thumbnailShowcasePrefab ?? thumbnailPrefab) is needed before switching to beem, then everything will be deleted
        Texture s = texture;

        var thumbnailItem = newThumbnail.GetComponent<BtnThumbnailItem>();
        thumbnailItem.UpdateThumbnailData(data.stream_s3_url, s, data?.user);
        thumbnailItem.SetLiveState(isLive);

        DateTime dateTime;
        if (DateTime.TryParse(data.start_date, out dateTime)) { //
            thumbnailItem.SetTimePeriod(dateTime);
        }

        thumbnails.Add(newThumbnail);

        switch (data.GetStatus()) {
            case StreamJsonData.Data.Stage.Finished:
                thumbnailItem.SetThumbnailPressAction(_ => {
                    pnlViewingExperience.ActivateForPreRecorded(data.stream_s3_url, null);
                    OnThumbnailClick.Invoke(); //TODO rewrite this after Beem V1
                });
                break;
            case StreamJsonData.Data.Stage.Live:
                thumbnailItem.SetThumbnailPressAction(_ => {
                    agoraController.ChannelName = data.agora_channel;
                    pnlStreamOverlay.OpenAsViewer();
                    OnThumbnailClick.Invoke(); //TODO rewrite this after Beem V1
                });
                break;
        }
    }

    private void Clear() {
        if (thumbnails == null)
            thumbnails = new List<GameObject>();

        foreach (var thumbnail in thumbnails) {
            Destroy(thumbnail);
        }

        thumbnails.Clear();
    }

    private void DataFetched() {
        if (this.isActiveAndEnabled)
            StartCoroutine(AddingFetchedData());
    }


    private IEnumerator AddingFetchedData() {

        if (firstLoading) {
            lastScrollPosition = 1;
            firstLoading = false;
        } else
            lastScrollPosition = scrollRect.verticalNormalizedPosition;

        Clear();

        int showCaseAddedData = 0;

        #region for testing time beem 1

        foreach (var data in homeScreenLoader.streamHomeScreenDataElement) {
            Debug.Log(data.streamJsonData.stream_s3_url);
            if (data.streamJsonData.stream_s3_url.Contains("00000010_BEEM_Jan_intro_holo_7113") || data.streamJsonData.file_name_prefix.Contains("Welcome_Jan")) {
                showCaseAddedData++;
                AddThumbnail(showCaseAddedData <= showcaseCount,
                    data.texture, data.streamJsonData, false);

                homeScreenLoader.streamHomeScreenDataElement.Remove(data);
                break;
            }
        }

        #endregion


        IEnumerable<HomeScreenLoader.DataElement> events =
            homeScreenLoader.eventHomeScreenDataElement.OrderBy(thumbnail => DateTime.Parse(thumbnail.streamJsonData.start_date));

        foreach (var data in events) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= showcaseCount,
                data.texture, data.streamJsonData, true);
            yield return null;
        }

        IEnumerable<HomeScreenLoader.DataElement> lifes =
            homeScreenLoader.liveHomeScreenDataElement.OrderByDescending(thumbnail => DateTime.Parse(thumbnail.streamJsonData.start_date));

        foreach (var data in lifes) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= showcaseCount,
                data.texture, data.streamJsonData, true);
            yield return null;
        }

        IEnumerable<HomeScreenLoader.DataElement> finisheds =
            homeScreenLoader.streamHomeScreenDataElement.OrderByDescending(thumbnail => DateTime.Parse(thumbnail.streamJsonData.start_date));

        foreach (var data in finisheds) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= showcaseCount,
                data.texture, data.streamJsonData, false);
        }

        //TODO rewrite this
        yield return new WaitForEndOfFrame();
        contentShowcaseThumbnails.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
        yield return new WaitForEndOfFrame();
        contentShowcaseThumbnails.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;

        scrollRect.verticalNormalizedPosition = lastScrollPosition;
        Debug.Log(lastScrollPosition);

        yield return new WaitForSeconds(tymeToNextRefresh);

        homeScreenLoader.FetchData();
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
}
