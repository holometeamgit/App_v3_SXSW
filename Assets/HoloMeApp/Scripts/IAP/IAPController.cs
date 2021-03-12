using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPController : MonoBehaviour, IStoreListener {

    public Action<Product> OnPurchaseHandler;
    public Action OnPurchaseFailedHandler;

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    private List<string> productIdList;

    //    public static string kProductIDConsumable = "FirstTProduct";

    public void BuyTicket(string productId) {
        BuyProductID(productId);
    }

    public void InitializePurchasing(List<string> productIdList) {
        // If we have already connected to Purchasing ...
        if (IsInitialized()) {
            // ... we are done here.
            return;
        }

        this.productIdList = productIdList;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var productId in productIdList) {
#if UNITY_IOS
            builder.AddProduct(productId, ProductType.Consumable);
#elif UNITY_ANDROID
        builder.AddProduct(productId.ToLower(), ProductType.Consumable);
#endif
        }

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized() {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    void BuyProductID(string productId) {
#if UNITY_ANDROID
        productId = productId.ToLower();
#endif

        // If Purchasing has been initialized ...
        if (IsInitialized()) {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            HelperFunctions.DevLog("Purchasing has been initialized");
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase) {
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases() {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized()) {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer) {

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
            });
        }
        // Otherwise ...
        else {
            // We are not running on an Apple device. No work is necessary to restore purchases.
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        // Purchasing has succeeded initializing. Collect our Purchasing references.

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error) {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {

        if (productIdList.Contains(args.purchasedProduct.definition.id)) {
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            HelperFunctions.DevLog("Consumable item has been successfully purchased " + args.purchasedProduct);
            OnPurchaseHandler?.Invoke(args.purchasedProduct);
        }
        else {
            HelperFunctions.DevLog("Fial: ProcessPurchase ");
            OnPurchaseFailedHandler?.Invoke();
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        OnPurchaseFailedHandler?.Invoke();
    }
}