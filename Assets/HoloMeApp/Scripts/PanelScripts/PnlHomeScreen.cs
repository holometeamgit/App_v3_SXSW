using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PnlHomeScreen : MonoBehaviour
{
    public enum HomeScreenPageType {
        Default,
        One,
        Two,
        Three,
        FourPlus
    }

    [SerializeField] float tymeToNextRefresh = 12;

    [SerializeField]
    PnlViewingExperience pnlViewingExperience;

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

    private List<GameObject> thumbnails;
    private AnimatedTransition animatedTransition;

    bool initiallaunch;

    void OnEnable() {
        if(animatedTransition == null) {
            animatedTransition = GetComponent<AnimatedTransition>();
        }

        Clear();
        if (!initiallaunch) {
            initiallaunch = true;
            homeScreenLoader.OnDataFetched.AddListener(DataFetched);
            if(!fetchDataOnFirstEnable)
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
        thumbnailItem.UpdateThumbnailData(data.stream_s3_url, s);
        thumbnailItem.SetLiveState(isLive);

        DateTime dateTime;
        if (DateTime.TryParse(data.end_date, out dateTime)) {
            thumbnailItem.SetTimePeriod(dateTime);
        }

        thumbnails.Add(newThumbnail);

        thumbnailItem.SetThumbnailPressAction(_ => {
            pnlViewingExperience.ActivateForPreRecorded(data.stream_s3_url, null);
            animatedTransition.DoMenuTransition(false);
            });
    }

    private void Clear() {
        if (thumbnails == null)
            thumbnails = new List<GameObject>();

        foreach(var thumbnail in thumbnails) {
            Destroy(thumbnail);
        }

        thumbnails.Clear();
    }

    private void DataFetched() {
        if(this.isActiveAndEnabled)
            StartCoroutine(AddingFetchedData());
    }

    private IEnumerator AddingFetchedData() {

        int showCaseAddedData = 0;

        foreach (var data in homeScreenLoader.eventHomeScreenDataElement) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= showcaseCount,
                data.texture, data.streamJsonData, true);
            yield return null;
        }

        foreach (var data in homeScreenLoader.liveHomeScreenDataElement) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= showcaseCount,
                data.texture, data.streamJsonData, true);
            yield return null;
        }

        foreach (var data in homeScreenLoader.streamHomeScreenDataElement) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= showcaseCount,
                data.texture, data.streamJsonData, false); 
        }

        yield return new WaitForSeconds(tymeToNextRefresh);

        Clear();
        homeScreenLoader.FetchData();
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
}
