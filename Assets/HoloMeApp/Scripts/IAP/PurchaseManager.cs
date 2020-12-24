using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaseManager : MonoBehaviour
{
    public Action OnPurchaseStarted;
    public Action OnPurchaseSuccessful;
    public Action OnPurchaseCanceled;
    public Action OnServerPurchasedDataUpdated;

    [SerializeField] IAPController iapController;
    [SerializeField] PurchasesSaveManager purchasesSaveManager;

    StreamJsonData.Data streamData;

    public void SetPurchaseStreamData(StreamJsonData.Data data) {
        streamData = data;
    }

    public bool IsBought() {
        return streamData == null || streamData.is_bought;
    }

    public void Purchase() {

        if (streamData == null || streamData.is_bought)
            return;
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchasePressed);
        //TODO add chech steam available befo purchaise 
        iapController.BuyTicket(streamData.product_type.product_id);
        OnPurchaseStarted?.Invoke();
    }

    private void Awake() {
        iapController.OnPurchaseHandler += OnPurchaseCallBack;
        iapController.OnPurchaseFailedHandler += OnPurchaseFailCallBack;
        purchasesSaveManager.OnAllDataSended += AllPurchasedDataSentOnServerCallBack;
    }

    private void OnPurchaseCallBack(Product product) {
        streamData.is_bought = true;
        streamData.InvokeDataUpdated();

        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseSuccessful);

        StreamBillingJsonData streamBillingJsonData = new StreamBillingJsonData();
        streamBillingJsonData.bill.hash = product.receipt;

        purchasesSaveManager.SendToServer(streamData.id, streamBillingJsonData);

        streamData = null;

        OnPurchaseSuccessful?.Invoke();
    }

    private void AllPurchasedDataSentOnServerCallBack() {
        OnServerPurchasedDataUpdated?.Invoke();
    }

    private void OnPurchaseFailCallBack() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseCancelled);
        OnPurchaseCanceled?.Invoke();
    }
}
