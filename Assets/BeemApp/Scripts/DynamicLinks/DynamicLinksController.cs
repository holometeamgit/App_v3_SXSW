using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beem.SSO;
using Firebase.DynamicLinks;
using UnityEngine;

namespace Beem.Firebase.DynamicLink {
    /// <summary>
    /// Deep Link Receiver
    /// </summary>
    public class DynamicLinksController : AbstractFirebaseController {

        private const string APPSTORE_ID = "1532446771";

        protected override void Subscribe() {
            DynamicLinks.DynamicLinkReceived += OnDynamicLink;
            DynamicLinksCallBacks.onCreateShortLink += CreateShortLink;

        }

        protected override void Unsubscribe() {
            DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
            DynamicLinksCallBacks.onCreateShortLink -= CreateShortLink;
        }

        // Display the dynamic link received by the application.
        private void OnDynamicLink(object sender, EventArgs args) {
            var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
            DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
            Debug.LogFormat("Received dynamic link {0}",
                            dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
        }

        private void CreateShortLink(string prefix, string id) {
            string baseLink = prefix + "/" + id;

            var components = new DynamicLinkComponents(
         // The base Link.
         new Uri(baseLink),
         // The dynamic link URI prefix.
         prefix) {
                IOSParameters = new IOSParameters(Application.identifier) {
                    AppStoreId = APPSTORE_ID
                },
                AndroidParameters = new AndroidParameters(Application.identifier)
            };

            var options = new DynamicLinkOptions {
                PathLength = DynamicLinkPathLength.Short
            };

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            DynamicLinks.GetShortLinkAsync(components, options).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("GetShortLinkAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("GetShortLinkAsync encountered an error: " + task.Exception);
                    return;
                }

                // Short Link has been created.
                ShortDynamicLink link = task.Result;
                Debug.LogFormat("Generated short link: {0}", link.Url);
                DynamicLinksCallBacks.onGetShortLink?.Invoke(link.Url);
            }, taskScheduler);
        }
    }
}
