using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using TMPro;
using System;
using NatShare;

public class PnlEventPurchaser : MonoBehaviour
{
    public Action OnPurchased;

    [SerializeField] IAPController iapController;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] PurchaseAPIScriptableObject purchaseAPISO;
    [SerializeField] List<Sprite> LockSprites;
    [SerializeField] AccountManager accountManager;

    [Space]
    [SerializeField] GameObject btnBuyTicket;
    [SerializeField] GameObject btnShare;
    [SerializeField] GameObject purchaseWaitingScreen;
    [SerializeField] Image imageIcon;
    [SerializeField] TMP_Text txtName;
    [SerializeField] TMP_Text txtDateOnSale;
    [SerializeField] TMP_Text txtDatePeriod;

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
        txtDateOnSale.text = data.StartDate.ToString("dd MMM") + (data.is_bought ? "" : " • On sale now") ;
        if (!data.is_bought) {
            imageIcon.sprite = LockSprites[0];
            txtDatePeriod.text = data.StartDate.ToString("H:mm") + (data.HasEndTime ? "" : " - " + data.EndDate.ToString("H:mm"));
        } else {
            imageIcon.sprite = LockSprites[1];
            txtDatePeriod.text = "This show will be viewable on " + data.StartDate.ToString("d MMMM");
        }
    }

    //TODO v3 move region  to not monobehaviour sctript

    #region Purchase
    public void Purchase() {
        //TODO v3 Check if it is already purchased first to receive a request from the server.
        //Then try to buy. After the purchase, he will make sure to make sure that they have written to the server.
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchasePressed);
        purchaseWaitingScreen.gameObject.SetActive(true);
        iapController.BuyTicket(data.product_type.product_id);
    }

    public void ShareStream() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareEventPressed);

        using (var payload = new SharePayload()) {
            string appName = "Beem";
            string iosLink = "https://apps.apple.com/us/app/beem/id1532446771?ign-mpt=uo%3D2";
            string androidLink = "https://play.google.com/store/apps/details?id=com.HoloMe.Beem";
            string appLink;
            switch (Application.platform) {
                case RuntimePlatform.IPhonePlayer:
                    appLink = iosLink;
                    appName = "Beem+";
                    break;

                case RuntimePlatform.Android:
                    appLink = androidLink;
                    appName = "Beem";
                    break;

                default:
                    appLink = iosLink + " - " + androidLink;
                    break;
            }

            string message = $"Click the link below to download the {appName} app which lets you experience human holograms using augmented reality: ";
            payload.AddText(message + appLink);
        }
    }

    public void Cancel() {
        if(!data.is_bought)
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseCancelled);
    }

    private void Awake() {
        iapController.OnPurchaseHandler += OnPurchaseCallBack;
        iapController.OnPurchaseFailedHandler += OnPurchaseFailCallBack;
    }

    private void OnPurchaseCallBack(Product product) {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseSuccessful);

        data.is_bought = true;
        data.OnDataUpdated.Invoke();

        Show(data);

        StreamBillingJsonData streamBillingJsonData = new StreamBillingJsonData();
        streamBillingJsonData.bill.hash = product.receipt;

        Debug.Log("OnPurchaseCallBack " + product.receipt);
        Debug.Log(GetRequestRefreshTokenURL()) ;

        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(),
           streamBillingJsonData, WebRequestHandler.BodyType.JSON,
           (code, body) => OnServerBillingSent(),
           (code, body) => OnServerErrorBillingSent(code, body), accountManager.GetAccessToken().access);
    }

    private void OnPurchaseFailCallBack() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseFailed);
        Debug.Log("OnPurchaseFailCallBack");
        Show(data);
    }

    private void OnServerBillingSent() {
        Debug.Log("OnServerBillingSent");
        OnPurchased?.Invoke();
    }

    private void OnServerErrorBillingSent(long code, string body) {
        Debug.Log("OnServerErrorBillingSent " + code + " " + body);
        OnPurchased?.Invoke();
    }

    private string GetRequestRefreshTokenURL() {
        return webRequestHandler.ServerURLMediaAPI + purchaseAPISO.SendPurchaseHash.Replace("{id}", data.id.ToString());
    }

    #endregion
}
