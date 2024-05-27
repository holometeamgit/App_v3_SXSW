using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

/// <summary>
/// Purchase success Event
/// </summary>
public class PurchaseView : MonoBehaviour {

    [Header("on Purchase Success")]
    [SerializeField]
    private UnityEvent onPurchasedSuccess;

    [Inject] private PurchaseManager _purchaseManager;

    // [Inject]
    // public void Construct(PurchaseManager purchaseManager) {
    //     _purchaseManager = purchaseManager;
    // }

    private void OnEnable() {
        if (_purchaseManager != null) {
            _purchaseManager.OnPurchaseSuccessful += OnPurchaseSuccess;
        }
    }

    private void OnDisable() {
        if (_purchaseManager != null) {
            _purchaseManager.OnPurchaseSuccessful -= OnPurchaseSuccess;
        }
    }

    private void OnPurchaseSuccess() {
        onPurchasedSuccess?.Invoke();
    }
}
