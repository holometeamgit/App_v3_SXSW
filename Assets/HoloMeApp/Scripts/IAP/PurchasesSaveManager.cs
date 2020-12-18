using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PurchasesSaveManager : MonoBehaviour {
    public Action OnAllDataSended;

    [SerializeField] UserWebManager userWebManager;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] PurchaseAPIScriptableObject purchaseAPISO;
    [SerializeField] AccountManager accountManager;

    private bool isSending;

    public void SendToServer(long id, StreamBillingJsonData streamBillingJsonData) {
        AddData(userWebManager.GetUnituniqueName(), id, streamBillingJsonData);
        CheckSubmittedData();
    }

    private void Awake() {
        userWebManager.OnUserInfoLoaded += CheckSubmittedData;
    }

    private void CheckSubmittedData() {
        //        Debug.Log("CheckSubmittedData");

        if (isSending)
            return;

        string uniqName = userWebManager.GetUnituniqueName();
        if (string.IsNullOrWhiteSpace(uniqName))
            return;

        if (!PlayerPrefs.HasKey(uniqName))
            return;

        try {
            PurchaseSaveJsonData purchaseSaveJsonData = JsonUtility.FromJson<PurchaseSaveJsonData>(PlayerPrefs.GetString(uniqName));

            //        Debug.Log("purchaseSaveElements count = " + purchaseSaveJsonData.purchaseSaveElements.Count);

            if (purchaseSaveJsonData.purchaseSaveElements.Count > 0) {
                isSending = true;
                PostData(uniqName, purchaseSaveJsonData.purchaseSaveElements[0].id,
                    purchaseSaveJsonData.purchaseSaveElements[0].streamBillingJsonData,
                    accountManager.GetAccessToken().access);
            } else {
                OnAllDataSended?.Invoke();
            }
        } catch (System.Exception) {
        }
    }

    private void AddData(string uniqName, long id, StreamBillingJsonData streamBillingJsonData) {
        PurchaseSaveElement purchaseSaveElement = new PurchaseSaveElement(id, streamBillingJsonData);

        RemovePurchaseSaveElement(uniqName, id, streamBillingJsonData);

        PurchaseSaveJsonData purchaseSaveJsonData = new PurchaseSaveJsonData();
        try {
            if (PlayerPrefs.HasKey(uniqName))
                purchaseSaveJsonData = JsonUtility.FromJson<PurchaseSaveJsonData>(PlayerPrefs.GetString(uniqName));

            purchaseSaveJsonData.Add(purchaseSaveElement);

            PlayerPrefs.SetString(uniqName, JsonUtility.ToJson(purchaseSaveJsonData));
            PlayerPrefs.Save();
        } catch (System.Exception) { }
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
        } catch (System.Exception) { }
    }

    private void PostData(string uniqName, long id, StreamBillingJsonData streamBillingJsonData, string accessToken) {//, Action OnPurchaseCallBackAfterTrySend) {
        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(id),
           streamBillingJsonData, WebRequestHandler.BodyType.JSON,
           (code, body) => { OnServerBillingSent(uniqName, id, streamBillingJsonData); isSending = false; CheckSubmittedData(); },// OnPurchaseCallBackAfterTrySend.Invoke(); },
           (code, body) => { OnServerErrorBillingSent(uniqName, id, streamBillingJsonData); isSending = false; }, accessToken);
    }

    private void OnServerBillingSent(string uniqName, long id, StreamBillingJsonData streamBillingJsonData) {
        Debug.Log("OnServerBillingSent");
        RemovePurchaseSaveElement(uniqName, id, streamBillingJsonData);
    }

    private void OnServerErrorBillingSent(string uniqName, long id, StreamBillingJsonData streamBillingJsonData) {
        Debug.Log("OnServerErrorBillingSent");

        //RemovePurchaseSaveElement(uniqName, id, streamBillingJsonData);
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