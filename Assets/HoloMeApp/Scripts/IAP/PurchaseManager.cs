using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Beem.SSO;
using Zenject;

public class PurchaseManager : MonoBehaviour {
    public Action OnPurchaseStarted;
    public Action OnPurchaseSuccessful;
    public Action OnPurchaseCanceled;
    public Action OnServerPurchasedDataUpdated;

    [SerializeField] PurchaseAPIScriptableObject purchaseAPISO;

    [Space]//TODO change to signal when DI will add
    [SerializeField] GameObject backgroudGO;

    StreamJsonData.Data streamData;
    private WebRequestHandler _webRequestHandler;
    private IAPController _iapController;
    private PurchasesSaveManager _purchasesSaveManager;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler, IAPController iapController, PurchasesSaveManager purchasesSaveManager) {
        _webRequestHandler = webRequestHandler;
        _iapController = iapController;
        _purchasesSaveManager = purchasesSaveManager;
    }

    public void SetPurchaseStreamData(StreamJsonData.Data data) {
        streamData = data;
    }

    public bool IsBought() {
        return streamData == null || streamData.is_bought;
    }

    public void Purchase() {
        if (streamData == null || streamData.is_bought)
            return;
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchasePressed, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, streamData.product_type.product_id }, { AnalyticParameters.ParamProductPrice, streamData.product_type.price.ToString() }, { AnalyticParameters.ParamBroadcasterUserID, streamData.user_id.ToString() } });
        backgroudGO.SetActive(true);
        _iapController.BuyTicket(streamData.product_type.product_id);
        OnPurchaseStarted?.Invoke();
    }

    private void Awake() {
        _iapController.OnPurchaseHandler += OnPurchaseCallBack;
        _iapController.OnPurchaseFailedHandler += OnPurchaseFailCallBack;
        _purchasesSaveManager.OnAllDataSended += AllPurchasedDataSentOnServerCallBack;
        _purchasesSaveManager.OnFailSentToserver += HideBackgroud;
    }

    private void Start() {
        GetProductList();
    }

    #region products request

    private void GetProductList() {
        _webRequestHandler.Get(GetRequestProductURL(), ProductListCallBack, ErrorProductListCallBack, needHeaderAccessToken: false);
    }

    private void ProductListCallBack(long code, string body) {
        ProductJsonData productJsonData = new ProductJsonData();
        try {
            productJsonData = JsonUtility.FromJson<ProductJsonData>(body);

        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);


            body = "{ \"product\" :" + body + "}";

            try {
                productJsonData = JsonUtility.FromJson<ProductJsonData>(body);
            } catch (Exception ex) {
                HelperFunctions.DevLogError(ex.Message);
            }
        }

        List<string> productIdList = new List<string>();

        foreach (var product in productJsonData.product) {
            productIdList.Add(product.product_id);
        }

        _iapController.InitializePurchasing(productIdList);
    }

    private void ErrorProductListCallBack(long code, string body) {
        StartCoroutine(RepeatRequestProduct());
    }

    #endregion

    private void OnPurchaseCallBack(Product product) {
        //streamData.is_bought = true;
        //streamData.InvokeDataUpdated();

        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseSuccessful, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, streamData.product_type.product_id }, { AnalyticParameters.ParamProductPrice, streamData.product_type.price.ToString() }, { AnalyticParameters.ParamBroadcasterUserID, streamData.user_id.ToString() } });
        AnalyticsCleverTapController.Instance.SendChargeEvent(new Dictionary<string, object> { { AnalyticParameters.ParamProductID, streamData.product_type.product_id }, { AnalyticParameters.ParamProductPrice, streamData.product_type.price.ToString() }, { "Currency", "USD" }, }, new List<Dictionary<string, object>> { new Dictionary<string, object> { { AnalyticParameters.ParamProductID, streamData.product_type.product_id }, { AnalyticParameters.ParamProductPrice, streamData.product_type.price.ToString() }, { AnalyticParameters.ParamBroadcasterUserID, streamData.user_id.ToString() } } });

        StreamBillingJsonData streamBillingJsonData = new StreamBillingJsonData();
        streamBillingJsonData.bill.hash = product.receipt;

        CallBacks.onStreamPurchasedInStore?.Invoke(streamData.id);
        _purchasesSaveManager.SendToServer(streamData.id, streamBillingJsonData);

        streamData = null;

        OnPurchaseSuccessful?.Invoke();

    }

    private void HideBackgroud() {
        backgroudGO.SetActive(false);
    }

    private void AllPurchasedDataSentOnServerCallBack() {
        HideBackgroud();
        OnServerPurchasedDataUpdated?.Invoke();
    }

    private void OnPurchaseFailCallBack() {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyPurchaseCancelled, new Dictionary<string, string> { { AnalyticParameters.ParamProductID, streamData.product_type.product_id }, { AnalyticParameters.ParamBroadcasterUserID, streamData.user_id.ToString() } });
        OnPurchaseCanceled?.Invoke();
        HideBackgroud();
    }

    private string GetRequestProductURL() {
        return _webRequestHandler.ServerURLMediaAPI + purchaseAPISO.GetProduct;
    }

    private IEnumerator RepeatRequestProduct() {
        yield return new WaitForSeconds(1);
        GetProductList();
    }
}
