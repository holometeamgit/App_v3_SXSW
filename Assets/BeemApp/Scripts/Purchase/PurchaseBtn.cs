using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class for purchase calling
/// </summary>
public class PurchaseBtn : MonoBehaviour, IPointerDownHandler {

    private PurchaseManager _purchaseManager;

    private void OnEnable() {
        Init();
    }

    private void Init() {
        _purchaseManager = FindObjectOfType<PurchaseManager>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        _purchaseManager.Purchase();
    }
}
