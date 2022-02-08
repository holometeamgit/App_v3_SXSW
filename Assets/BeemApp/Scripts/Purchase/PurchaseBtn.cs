using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

/// <summary>
/// Class for purchase calling
/// </summary>
public class PurchaseBtn : MonoBehaviour, IPointerDownHandler {


    private PurchaseManager _purchaseManager;

    [Inject]
    public void Construct(PurchaseManager purchaseManager) {
        _purchaseManager = purchaseManager;
    }

    public void OnPointerDown(PointerEventData eventData) {
        _purchaseManager.Purchase();
    }
}
