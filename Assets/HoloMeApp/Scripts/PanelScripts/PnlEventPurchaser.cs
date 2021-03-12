using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using TMPro;
using System;
using NatShare;

public class PnlEventPurchaser : MonoBehaviour {
    //public Action OnPurchased;
    public Action OnServerPurchasedDataUpdate;

    [SerializeField] IAPController iapController;
    [SerializeField] List<Sprite> LockSprites;
    [SerializeField] PurchasesSaveManager purchasesSaveManager;

    [Space]
    [SerializeField] GameObject btnBuyTicket;
    [SerializeField] GameObject btnShare;
    [SerializeField] GameObject purchaseWaitingScreen;
    [SerializeField] Image imageIcon;
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text txtDateOnSale;
    [SerializeField] TMP_Text txtDatePeriod;

    [Space]
    [SerializeField] ShareManager shareManager;

    StreamJsonData.Data data;

    public void Show(StreamJsonData.Data data) {
        if (!data.HasProduct)
            return;

        this.data = data;

        gameObject.SetActive(true);

        btnBuyTicket.gameObject.SetActive(!data.is_bought);
        btnShare.gameObject.SetActive(data.is_bought);
        purchaseWaitingScreen.gameObject.SetActive(false);

        txtName.text = data.user;
        txtDateOnSale.text = data.StartDate.ToString("dd MMM") + (data.is_bought ? "" : " • On sale now");
        if (!data.is_bought) {
            imageIcon.sprite = LockSprites[0];
            txtDatePeriod.text = data.StartDate.ToString("H:mm") + (data.HasEndTime ? "" : " - " + data.EndDate.ToString("H:mm"));
        } else {
            imageIcon.sprite = LockSprites[1];
            txtDatePeriod.text = "This show will be viewable on " + data.StartDate.ToString("d MMMM");
        }
    }

    #region Purchase
    public void Purchase() {

        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchasePressed, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, data.product_type.product_id }, { AnalyticParameters.ParamProductPrice, data.product_type.price.ToString() } });
        purchaseWaitingScreen.gameObject.SetActive(true);
        iapController.BuyTicket(data.product_type.product_id);
    }

    public void ShareStream() {
        shareManager.ShareStream();
    }

    public void Cancel() {
        if (!data.is_bought)
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseCancelled, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, data.product_type.product_id } });
    }

    private void Awake() {
        iapController.OnPurchaseHandler += OnPurchaseCallBack;
        iapController.OnPurchaseFailedHandler += OnPurchaseFailCallBack;
        purchasesSaveManager.OnAllDataSended += AllPurchasedDataSentOnServerCallBack;
    }

    private void OnPurchaseCallBack(Product product) {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseSuccessful, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, data.product_type.product_id }, { AnalyticParameters.ParamProductPrice, data.product_type.price.ToString() } });

        data.is_bought = true;
        data.OnDataUpdated?.Invoke();

        Show(data);

        StreamBillingJsonData streamBillingJsonData = new StreamBillingJsonData();
        streamBillingJsonData.bill.hash = product.receipt;

        Debug.Log("Purchase success: OnPurchaseCallBack " + product.receipt);
        purchasesSaveManager.SendToServer(data.id, streamBillingJsonData);
        //OnPurchased?.Invoke();
    }

    private void AllPurchasedDataSentOnServerCallBack() {
        OnServerPurchasedDataUpdate?.Invoke();
    }

    private void OnPurchaseFailCallBack() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseFailed, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, data.product_type.product_id } });
        Debug.Log("OnPurchaseFailCallBack");
        Show(data);
    }

    #endregion
}
