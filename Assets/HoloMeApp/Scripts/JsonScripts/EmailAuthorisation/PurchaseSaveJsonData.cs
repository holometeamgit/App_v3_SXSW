using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PurchaseSaveJsonData {
    public List<PurchaseSaveElement> purchaseSaveElements;

    public PurchaseSaveJsonData() {
        purchaseSaveElements = new List<PurchaseSaveElement>();
    }

    public void Add(PurchaseSaveElement purchaseSaveElement) {
        RemoveElement(purchaseSaveElement.id);
        purchaseSaveElements.Add(purchaseSaveElement);
    }

    public void RemoveElement(long id) {
        foreach (var element in purchaseSaveElements) {
            if (element.id == id) {
                purchaseSaveElements.Remove(element);
                break;
            }
        }
    }
}

[Serializable]
public class PurchaseSaveElement {
    public long id;
    public StreamBillingJsonData streamBillingJsonData;

    public PurchaseSaveElement() {
        streamBillingJsonData = new StreamBillingJsonData();
    }

    public PurchaseSaveElement(long id, StreamBillingJsonData streamBillingJsonData) {
        this.id = id;
        this.streamBillingJsonData = streamBillingJsonData;
    }
}