using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Purchase success Event
/// </summary>
public class PurchaseView : MonoBehaviour {

    [Header("on Purchase Success")]
    [SerializeField]
    private UnityEvent onPurchasedSuccess;

    private PurchaseManager _purchaseManager;

    private PurchaseManager purchaseManager {
        get {
            if (_purchaseManager == null) {
                _purchaseManager = FindObjectOfType<PurchaseManager>();
            }

            return _purchaseManager;
        }
    }

    private void OnEnable() {
        if (purchaseManager != null) {
            purchaseManager.OnPurchaseSuccessful += OnPurchaseSuccess;
        }
    }

    private void OnDisable() {
        if (purchaseManager != null) {
            purchaseManager.OnPurchaseSuccessful -= OnPurchaseSuccess;
        }
    }

    private void OnPurchaseSuccess() {
        onPurchasedSuccess?.Invoke();
    }
}
