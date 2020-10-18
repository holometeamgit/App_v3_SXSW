using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class ThumbnailsPurchaser : MonoBehaviour
{
    [SerializeField] IAPController iapController;
    List<StreamJsonData.Data> dataList;

    public void SetStreamJsonData(List<StreamJsonData.Data> data) {
        dataList = data;
    }

    private void Awake() {
        iapController.OnPurchaseHandler += OnPurchaseCallBack;
        iapController.OnPurchaseFailedHandler += OnPurchaseFail;
    }

    private void OnPurchaseCallBack(Product product) {
        foreach(var data in dataList) {
            if (data.product_type != null && data.product_type.product_id == product.definition.id) //TODO it might be better to make another comparison
                Debug.Log("TODO Update UI");
        }
    }

    private void OnPurchaseFail() {

    }
}
