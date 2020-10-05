using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PnlBroadcasterProfile : MonoBehaviour
{
    [SerializeField] AccountManager accountManager; 
    [SerializeField] AnimatedTransition menuProfileBurger;
    [SerializeField] AnimatedTransition menuUserProfileBurger;
    [Space]
    [SerializeField] GameObject broadcasterProfile;
    [Space]
    [SerializeField] Button menuBtn;

    [SerializeField]
    BroadcasterScreenLoader broadcasterScreenLoader;

    [SerializeField]
    PnlViewingExperience pnlViewingExperience;

    [SerializeField]//TODO get it in future from account or broadcaster user page
    string userName;

    [SerializeField]
    GameObject thumbnailPrefab;

    [SerializeField]
    RectTransform contentUserThumbnails;

    bool initiallaunch;

    private List<GameObject> thumbnails;

    public void ShowMenu() {
        var accauntType = accountManager.GetAccountType();
        menuProfileBurger.DoMenuTransition(accauntType == AccountManager.AccountType.Broadcater);
        menuUserProfileBurger.DoMenuTransition(accauntType == AccountManager.AccountType.Subscriber);
        menuBtn.gameObject.SetActive(false);
    }

    private void OnEnable() {
        //menuProfileBurger.DoMenuTransition(false);
        //menuUserProfileBurger.DoMenuTransition(false);

        menuBtn.gameObject.SetActive(true);

        Clear();
        if (!initiallaunch) {
            initiallaunch = true;
            broadcasterScreenLoader.OnDataFetched.AddListener(DataFetched);
            return;
        } else {
            broadcasterScreenLoader.FetchDataFromBeginning(userName);
        }
    }

    private void OnDisable() {
        menuBtn.gameObject.SetActive(false);
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
        StartCoroutine(AddingFetchedData());
    }

    private IEnumerator AddingFetchedData() {

        int showCaseAddedData = 0;

        foreach (var data in broadcasterScreenLoader.dataElements) {
            showCaseAddedData++;
            AddThumbnail(contentUserThumbnails,
                data.texture,
                data.streamJsonData, data.streamJsonData.GetStatus() != StreamJsonData.Data.Stage.Finished);
            yield return null;
        }

        yield return null;
    }

    private void AddThumbnail(RectTransform contentThumbnails, Texture texture, StreamJsonData.Data data, bool isLive) {
        var newThumbnail = Instantiate(thumbnailPrefab, contentThumbnails);
        Texture s = texture;

        var thumbnailItem = newThumbnail.GetComponent<BtnThumbnailItem>();
        thumbnailItem.UpdateThumbnailData(data.stream_s3_url, s);
        thumbnailItem.SetLiveState(isLive);

        thumbnailItem.SetTimePeriod(data.EndDate);

        thumbnails.Add(newThumbnail);

        thumbnailItem.SetThumbnailPressAction(_ => {
            this.gameObject.SetActive(false);
            pnlViewingExperience.ActivateForPreRecorded(data.stream_s3_url, null);
        });
    }
}
