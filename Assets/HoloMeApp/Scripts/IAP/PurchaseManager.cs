using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Beem.SSO;

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
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchasePressed, new Dictionary<string, string> { {AnalyticParameters.ParamProductID, streamData.product_type.product_id}, { AnalyticParameters.ParamProductPrice, streamData.product_type.price.ToString() } } );
        //TODO add chech stream available befor purchaise
        backgroudGO.SetActive(true);
        iapController.BuyTicket(streamData.product_type.product_id);
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
            
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
            body = "{ \"products\" :" + body + "}";

            try {
                productJsonData = JsonUtility.FromJson<ProductJsonData>(body);
            } catch (Exception ex) {
                HelperFunctions.DevLogError(ex.Message);
            }
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
        //streamData.is_bought = true;
        //streamData.InvokeDataUpdated();

        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseSuccessful, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, streamData.product_type.product_id }, {AnalyticParameters.ParamProductPrice, streamData.product_type.price.ToString() } });

        StreamBillingJsonData streamBillingJsonData = new StreamBillingJsonData();
        streamBillingJsonData.bill.hash = product.receipt;

        CallBacks.onStreamPurchasedInStore?.Invoke(streamData.id);
        purchasesSaveManager.SendToServer(streamData.id, streamBillingJsonData);

        streamData = null;

        OnPurchaseSuccessful?.Invoke();
        
    }

    private void AllPurchasedDataSentOnServerCallBack() {
        backgroudGO.SetActive(false);
        OnServerPurchasedDataUpdated?.Invoke();
    }

    private void OnPurchaseFailCallBack() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseCancelled, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, streamData.product_type.product_id } });
        OnPurchaseCanceled?.Invoke();
        backgroudGO.SetActive(false);
    }

    private string GetRequestProductURL() {
        return webRequestHandler.ServerURLMediaAPI + purchaseAPISO.GetProduct;
    }

    private IEnumerator RepeatRequestProduct () {
        yield return new WaitForSeconds(1);
        HelperFunctions.DevLog("Product list Request");
        GetProductList();
    }
}
