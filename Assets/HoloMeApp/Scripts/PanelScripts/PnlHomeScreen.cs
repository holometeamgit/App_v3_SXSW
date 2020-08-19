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

    [SerializeField]
    AnimatedTransition pnlGenericLoading;
    [SerializeField]
    PnlViewingExperience pnlViewingExperience;

    [SerializeField]
    HomeScreenLoader homeScreenLoader;

    [SerializeField]
    RectTransform contentShowcaseThumbnails;

    [SerializeField]
    RectTransform contentUserThumbnails;

    [SerializeField]
    GameObject thumbnailPrefab;

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
            return;
        }
        homeScreenLoader.FetchData();
    }

    private void AddThumbnail(RectTransform contentThumbnails, Texture texture, StreamJsonData.Data data, bool isLive) {
        var newThumbnail = Instantiate(thumbnailPrefab, contentThumbnails);
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
        StartCoroutine(EdingFetchedData());
    }

    private IEnumerator EdingFetchedData() {

        int showCaseAddedData = 0;

        foreach (var data in homeScreenLoader.eventHomeScreenDataElement) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= 2 ? contentShowcaseThumbnails : contentUserThumbnails
                , data.texture, data.streamJsonData, true);
            yield return null;
        }

        foreach (var data in homeScreenLoader.liveHomeScreenDataElement) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= 2 ? contentShowcaseThumbnails : contentUserThumbnails
                , data.texture, data.streamJsonData, true);
            yield return null;
        }

        foreach (var data in homeScreenLoader.streamHomeScreenDataElement) {
            showCaseAddedData++;
            AddThumbnail(showCaseAddedData <= 2 ? contentShowcaseThumbnails : contentUserThumbnails
                , data.texture, data.streamJsonData, false); 
        }

        yield return null;
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
}
