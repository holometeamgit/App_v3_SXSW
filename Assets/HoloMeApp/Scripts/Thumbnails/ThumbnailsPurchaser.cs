using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class ThumbnailsPurchaser : MonoBehaviour {
    [SerializeField] IAPController iapController;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] PurchaseAPIScriptableObject purchaseAPISO;

    public void Purchase(string productID) {
        iapController.BuyTicket(productID);
    }

    private void Awake() {
        iapController.OnPurchaseHandler += OnPurchaseCallBack;
        iapController.OnPurchaseFailedHandler += OnPurchaseFail;
    }



    //TODO обновление 3-х статусов
    //статус после покупки обновляется только когда мы отправили информацию о покупке,
    //потом запросили у сервера и только после этого повторного запроса мы обновляем на UI об покупке 
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
