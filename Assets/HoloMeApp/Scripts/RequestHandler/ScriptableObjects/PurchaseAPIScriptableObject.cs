using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HelpURL("https://devholo.me/docs/auth/?urls.primaryName=Media#/")]
[CreateAssetMenu(fileName = "PurchaseAPI", menuName = "Data/API/PurchaseAPI")]
public class PurchaseAPIScriptableObject : ScriptableObject {
    [Header("Purchase")]
    [Tooltip("Send hash for store")]
    public string SendPurchaseHash = "/stream/{id}/bill/";

    [Tooltip("Get products list")]
    public string GetProduct = "/product/";
}
