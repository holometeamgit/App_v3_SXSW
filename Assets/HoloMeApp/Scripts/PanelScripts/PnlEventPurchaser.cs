using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PnlEventPurchaser : MonoBehaviour
{
    [SerializeField] IAPController iapController;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] PurchaseAPIScriptableObject purchaseAPISO;

    StreamJsonData.Data data;

    public void Show(StreamJsonData.Data data) {
        if (!data.HasProduct)
            return;

        gameObject.SetActive(true);
    }

    public void Purchase() {
        iapController.BuyTicket(data.product_type.product_id);
    }

    private void Awake() {
        iapController.OnPurchaseHandler += OnPurchaseCallBack;
        iapController.OnPurchaseFailedHandler += OnPurchaseFail;
    }



    //TODO обновление cтатуса
    // статус сохраняем в playerpref и отправляем запрос на сервер, если сервер ответил, то скачиваем конкретный thu заново и обновляем данные.
    //если прервалось то до скачивани домашней страцы в первую очередь отпрвяем все покупки на сервер и ждём подтврждение по каждой,после чистим данные
    //при покупке ждать ответа от магазина, пользователь ничего не может нажать!

    private void OnPurchaseCallBack(Product product) {
        //product.definition.id /// не понятно id продукта отправлять или thumbnail? 
        //webRequestHandler.PostRequest();

        Debug.Log(product.definition.storeSpecificId);
    }

    private void OnPurchaseFail() {

    }

    private string GetRequestRefreshTokenURL(string thumbnailID) {
        return "";
    }
}
