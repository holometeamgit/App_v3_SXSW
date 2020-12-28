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
    [SerializeField] PurchaseAPIScriptableObject purchaseAPISO;
    [SerializeField] WebRequestHandler webRequestHandler;

    [Space]//TODO change to signal when DI will add
    [SerializeField] GameObject backgroudGO;

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
        backgroudGO.SetActive(true);
        OnPurchaseStarted?.Invoke();
    }

    private void Awake() {
        iapController.OnPurchaseHandler += OnPurchaseCallBack;
        iapController.OnPurchaseFailedHandler += OnPurchaseFailCallBack;
        purchasesSaveManager.OnAllDataSended += AllPurchasedDataSentOnServerCallBack;
    }

    private void Start() {
        GetProductList();
    }

    #region products request

    private void GetProductList() {
        webRequestHandler.GetRequest(GetRequestProductURL(), ProductListCallBack, ErrorProductListCallBack);
    }

    private void ProductListCallBack(long code, string body) {
        ProductJsonData productJsonData = new ProductJsonData();
        try {
            productJsonData = JsonUtility.FromJson<ProductJsonData>(body);
            
        } catch (Exception) {
            body = "{ \"products\" :" + body + "}";

            try {
                productJsonData = JsonUtility.FromJson<ProductJsonData>(body);
            } catch (Exception) { }
        }

        List<string> productIdList = new List<string>();

        foreach (var product in productJsonData.products) {
            productIdList.Add(product.product_id);
        }

        iapController.InitializePurchasing(productIdList);
    }

    private void ErrorProductListCallBack(long code, string body) {
        StartCoroutine(RepeatRequestProduct());
    }

    #endregion

    private void OnPurchaseCallBack(Product product) {
        streamData.is_bought = true;
        streamData.InvokeDataUpdated();

        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseSuccessful);

        StreamBillingJsonData streamBillingJsonData = new StreamBillingJsonData();
        streamBillingJsonData.bill.hash = product.receipt;

        purchasesSaveManager.SendToServer(streamData.id, streamBillingJsonData);

        streamData = null;

        OnPurchaseSuccessful?.Invoke();
        backgroudGO.SetActive(false);
    }

    private void AllPurchasedDataSentOnServerCallBack() {
        OnServerPurchasedDataUpdated?.Invoke();
    }

    private void OnPurchaseFailCallBack() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseCancelled);
        OnPurchaseCanceled?.Invoke();
        backgroudGO.SetActive(false);
    }

    private string GetRequestProductURL() {
        return webRequestHandler.ServerURLMediaAPI + purchaseAPISO.GetProduct;
    }

    private IEnumerator RepeatRequestProduct () {
        yield return new WaitForSeconds(1);
        GetProductList();
    }
}
