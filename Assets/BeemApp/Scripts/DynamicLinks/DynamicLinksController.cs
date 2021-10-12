using System;
using System.Threading.Tasks;
using Firebase.DynamicLinks;
using UnityEngine;

namespace Beem.Firebase.DynamicLink {
    /// <summary>
    /// Deep Link Receiver
    /// </summary>
    public class DynamicLinksController : MonoBehaviour {

        private const string APPSTORE_ID = "1532446771";

        private void OnEnable() {
            FirebaseCallBacks.onInit += Subscribe;
        }

        protected void Subscribe() {
            DynamicLinks.DynamicLinkReceived += OnDynamicLink;
            DynamicLinksCallBacks.onCreateShortLink += CreateShortLink;

        }

        protected void OnDisable() {
            DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
            DynamicLinksCallBacks.onCreateShortLink -= CreateShortLink;
            FirebaseCallBacks.onInit -= Subscribe;
        }

        // Display the dynamic link received by the application.
        private void OnDynamicLink(object sender, EventArgs args) {
            var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
            DynamicLinksCallBacks.onReceivedDeepLink?.Invoke(dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
            Debug.LogFormat("Received dynamic link {0}",
                            dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);

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

        private void CreateShortLink(DynamicLinkParameters dynamicLinkParameters) {

            string dynamicLink = string.Empty;
            string urlFormat = string.Empty;
            string desktopLink = string.Empty;
            string baseLink = string.Empty;

            if (dynamicLinkParameters.ParameterName == DynamicLinkParameters.Parameter.username) {
                dynamicLink = dynamicLinkParameters.DynamicLinkURL + "?" + dynamicLinkParameters.ParameterName + "=" + dynamicLinkParameters.ParameterId;

                desktopLink = dynamicLink;

                baseLink = dynamicLink;

                var components = new DynamicLinkComponents(new Uri(dynamicLink), dynamicLinkParameters.Prefix);

                urlFormat = components.LongDynamicLink.AbsoluteUri;
            } else {
                dynamicLink = dynamicLinkParameters.Prefix + "?" + dynamicLinkParameters.ParameterName + "=" + dynamicLinkParameters.ParameterId;
                desktopLink = dynamicLinkParameters.DynamicLinkURL;

                var components = new DynamicLinkComponents(new Uri(dynamicLink), dynamicLinkParameters.Prefix);

                urlFormat = components.LongDynamicLink.AbsoluteUri;
            }

            LinkBuilder.AndroidParameters androidLinkBuilder = new LinkBuilder.AndroidParameters(Application.identifier, baseLink);
            LinkBuilder.iOSParameters iOSLinkBuilder = new LinkBuilder.iOSParameters(Application.identifier, baseLink, APPSTORE_ID);
            LinkBuilder.DesktopParameters desktopLinkBuilder = new LinkBuilder.DesktopParameters(desktopLink);
            LinkBuilder.SocialMetaTagParameters socialMetaTagBuilder = new LinkBuilder.SocialMetaTagParameters(dynamicLinkParameters.SocialMetaTagParameters.Title, dynamicLinkParameters.SocialMetaTagParameters.Description, dynamicLinkParameters.SocialMetaTagParameters.ImageUrl.ToString());

            LinkBuilder linkBuilder = new LinkBuilder(androidLinkBuilder, iOSLinkBuilder, desktopLinkBuilder, socialMetaTagBuilder);
            urlFormat += linkBuilder.Get;

            Uri uri = new Uri(urlFormat);

            var options = new DynamicLinkOptions {
                PathLength = DynamicLinkPathLength.Unguessable
            };

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            DynamicLinks.GetShortLinkAsync(uri, options).ContinueWith(task => {
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
                DynamicLinksCallBacks.onGetShortLink?.Invoke(link.Url, dynamicLinkParameters.SocialMetaTagParameters);
            }, taskScheduler);
        }
    }
}
