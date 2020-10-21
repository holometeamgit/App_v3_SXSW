using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using TMPro;

public class PnlEventPurchaser : MonoBehaviour
{
    [SerializeField] IAPController iapController;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] PurchaseAPIScriptableObject purchaseAPISO;
    [SerializeField] List<Sprite> LockSprites;

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
            txtDatePeriod.text = data.StartDate.ToString("H:m") + (data.HasEndTime ? "" : " - " + data.EndDate.ToString("H:m"));
        } else {
            txtDatePeriod.text = "This show will be viewable on " + data.StartDate.ToString("d MMMM");
        }
    }

    //TODO v3 move region  to not monobehaviour sctript

    #region Purchase
    public void Purchase() {
        //TODO v3 Check if it is already purchased first to receive a request from the server.
        //Then try to buy. After the purchase, he will make sure to make sure that they have written to the server.
        purchaseWaitingScreen.gameObject.SetActive(true);
        iapController.BuyTicket(data.product_type.product_id);
    }

    private void Awake() {
        iapController.OnPurchaseHandler += OnPurchaseCallBack;
        iapController.OnPurchaseFailedHandler += OnPurchaseFailCallBack;
    }


    //TODO обновление cтатуса
    //статус сохраняем в playerpref и отправляем запрос на сервер, если сервер ответил, то скачиваем конкретный thu заново и обновляем данные.
    //если прервалось то до скачивани домашней страцы в первую очередь отпрвяем все покупки на сервер и ждём подтврждение по каждой,после чистим данные
    //при покупке ждать ответа от магазина, пользователь ничего не может нажать!

    private void OnPurchaseCallBack(Product product) {
        //webRequestHandler.PostRequest();
        data.is_bought = true;
        data.OnDataUpdated.Invoke();

        Show(data);

        StreamBillingJsonData streamBillingJsonData = new StreamBillingJsonData();
        streamBillingJsonData.bill.hash = product.definition.storeSpecificId;

        Debug.Log("OnPurchaseCallBack " + product.definition.storeSpecificId);

        webRequestHandler.PostRequest(GetRequestRefreshTokenURL(),
           streamBillingJsonData, WebRequestHandler.BodyType.JSON,
           (code, body) => OnServerBillingSent(),
           (code, body) => OnServerErrorBillingSent());
    }

    private void OnPurchaseFailCallBack() {
        Debug.Log("OnPurchaseFailCallBack");
        Show(data);
    }

    private void OnServerBillingSent() {
        Debug.Log("OnServerBillingSent");
    }

    private void OnServerErrorBillingSent() {
        Debug.Log("OnServerErrorBillingSent");
    }

    private string GetRequestRefreshTokenURL() {
        return webRequestHandler.ServerURLMediaAPI + purchaseAPISO.SendPurchaseHash.Replace("{id}", data.id.ToString());
    }

    #endregion
}
