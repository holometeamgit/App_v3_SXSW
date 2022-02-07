using System;
using System.Threading.Tasks;
using Firebase.DynamicLinks;
using UnityEngine;

namespace Beem.Firebase.DynamicLink {
    /// <summary>
    /// Deep Link Receiver
    /// </summary>
    public class DynamicLinksController : MonoBehaviour {

        private void OnEnable() {
            FirebaseCallBacks.onInit += Subscribe;
        }

        private void Subscribe() {
            DynamicLinks.DynamicLinkReceived += OnDynamicLink;

        }

        private void OnDisable() {
            DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
            FirebaseCallBacks.onInit -= Subscribe;
        }

        private void OnDynamicLink(string url) {
            DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(url);
            HelperFunctions.DevLog("Received dynamic link {0}", url);
        }

        // Display the dynamic link received by the application.
        private void OnDynamicLink(object sender, EventArgs args) {
            var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
            OnDynamicLink(dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);

#if UNITY_ANDROID
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                    var intent = activity.Call<AndroidJavaObject>("getIntent");
                    intent.Call("removeExtra", "com.google.firebase.dynamiclinks.DYNAMIC_LINK_DATA");
                    intent.Call("removeExtra", "com.google.android.gms.appinvite.REFERRAL_BUNDLE");
                }
            }
#endif
        }

    }
}
