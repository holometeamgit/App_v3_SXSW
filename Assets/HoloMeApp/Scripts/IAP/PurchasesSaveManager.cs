using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;

public class PurchasesSaveManager : MonoBehaviour {
    public Action OnAllDataSended;
    public Action OnFailSentToserver;

    [SerializeField] UserWebManager userWebManager;
    [SerializeField]
    AuthController authController;
    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField]
    PurchaseAPIScriptableObject purchaseAPISO;
    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    IAPController iapController;

    private bool isBusy;

    public void SendToServer(long id, StreamBillingJsonData streamBillingJsonData) {
        AddData(authController.GetID(), id, streamBillingJsonData);
        CheckSubmittedData();
    }

    private void Awake() {
        userWebManager.OnUserInfoLoaded += CheckSubmittedData;
    }

    private void CheckSubmittedData() {

//        HelperFunctions.DevLog("Check purchased Submitted Data: isBusy " + isBusy);
        if (isBusy) {
            OnFailSentToserver?.Invoke();
            return;
        }

        string uniqName = authController.GetID();

//        HelperFunctions.DevLog("Check purchased Submitted Data: uniqName " + isBusy);
        if (string.IsNullOrWhiteSpace(uniqName)) {
            OnFailSentToserver?.Invoke();
            return;
        }

//        HelperFunctions.DevLog("Check purchased Submitted Data: has data for this user " + !PlayerPrefs.HasKey(uniqName));

        if (!PlayerPrefs.HasKey(uniqName)) {
            OnFailSentToserver?.Invoke();
            return;
        }

        try {
            PurchaseSaveJsonData purchaseSaveJsonData = JsonUtility.FromJson<PurchaseSaveJsonData>(PlayerPrefs.GetString(uniqName));

//            HelperFunctions.DevLog("Check purchased Submitted Data: purchaseSaveElements.Count " + purchaseSaveJsonData.purchaseSaveElements.Count);

            if (purchaseSaveJsonData.purchaseSaveElements.Count > 0) {
                isBusy = true;
                PostData(uniqName, purchaseSaveJsonData.purchaseSaveElements[0].id,
                    purchaseSaveJsonData.purchaseSaveElements[0].streamBillingJsonData,
                    accountManager.GetAccessToken().access);
            } else {
                OnAllDataSended?.Invoke();
            }
        } catch (System.Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void AddData(string uniqName, long id, StreamBillingJsonData streamBillingJsonData) {

//        HelperFunctions.DevLog("AddData purchase data for " + uniqName + " stream id = " + id);
        PurchaseSaveElement purchaseSaveElement = new PurchaseSaveElement(id, streamBillingJsonData);

        RemovePurchaseSaveElement(uniqName, id, streamBillingJsonData);

        PurchaseSaveJsonData purchaseSaveJsonData = new PurchaseSaveJsonData();
        try {
            if (PlayerPrefs.HasKey(uniqName))
                purchaseSaveJsonData = JsonUtility.FromJson<PurchaseSaveJsonData>(PlayerPrefs.GetString(uniqName));

            purchaseSaveJsonData.Add(purchaseSaveElement);

            PlayerPrefs.SetString(uniqName, JsonUtility.ToJson(purchaseSaveJsonData));
            PlayerPrefs.Save();
        } catch (System.Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void RemovePurchaseSaveElement(string uniqName, long id, StreamBillingJsonData streamBillingJsonData) {
        PurchaseSaveElement purchaseSaveElement = new PurchaseSaveElement(id, streamBillingJsonData);

        if (!PlayerPrefs.HasKey(uniqName))
            return;
        try {
            PurchaseSaveJsonData purchaseSaveJsonData = JsonUtility.FromJson<PurchaseSaveJsonData>(PlayerPrefs.GetString(uniqName));

            foreach (var element in purchaseSaveJsonData.purchaseSaveElements.ToArray()) {
                if (element.id == purchaseSaveElement.id)
                    purchaseSaveJsonData.purchaseSaveElements.Remove(element);
            }

            PlayerPrefs.SetString(uniqName, JsonUtility.ToJson(purchaseSaveJsonData));
            PlayerPrefs.Save();
        } catch (System.Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void PostData(string uniqName, long id, StreamBillingJsonData streamBillingJsonData, string accessToken) {
        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(id),
           streamBillingJsonData, WebRequestHandler.BodyType.JSON,
           (code, body) => { OnServerBillingSent(uniqName, id, streamBillingJsonData); isBusy = false; CheckSubmittedData(); },
           (code, body) => { OnServerErrorBillingSent(uniqName, id, streamBillingJsonData); isBusy = false; }, accessToken);
    }

    private void OnServerBillingSent(string uniqName, long id, StreamBillingJsonData streamBillingJsonData) {
        CallBacks.onStreamPurchasedAndUpdateOnServer?.Invoke(id);
        RemovePurchaseSaveElement(uniqName, id, streamBillingJsonData);
    }

    private void OnServerErrorBillingSent(string uniqName, long id, StreamBillingJsonData streamBillingJsonData) {
        StartCoroutine(Rechecking());
    }

    IEnumerator Rechecking() {
        yield return new WaitForSeconds(2);
        CheckSubmittedData();
    }

    private string GetRequestRefreshTokenURL(long id) {
        return webRequestHandler.ServerURLMediaAPI + purchaseAPISO.SendPurchaseHash.Replace("{id}", id.ToString());
    }
}