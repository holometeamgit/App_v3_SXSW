using System;
using System.Collections;
using System.Collections.Generic;
using Beem.SSO;
using Firebase.DynamicLinks;
using UnityEngine;

namespace Beem.Firebase.DynamicLink {
    /// <summary>
    /// Deep Link Receiver
    /// </summary>
    public class DynamicLinksController : MonoBehaviour {

        private void OnEnable() {
            FirebaseCallBacks.onInit += InitializeFirebase;
        }

        private void OnDisable() {
            FirebaseCallBacks.onInit -= InitializeFirebase;
            DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
        }

        private void InitializeFirebase() {
            DynamicLinks.DynamicLinkReceived += OnDynamicLink;
        }

        // Display the dynamic link received by the application.
        private void OnDynamicLink(object sender, EventArgs args) {
            var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
            DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
            Debug.LogFormat("Received dynamic link {0}",
                            dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
        }
    }
}
