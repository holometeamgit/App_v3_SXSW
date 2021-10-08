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

            HelperFunctions.DevLogError("CreateShortLink");
            string baseLink = string.Empty;
            string urlFormat = string.Empty;

            if (dynamicLinkParameters.ParameterName == DynamicLinkParameters.Parameter.username) {
                baseLink = dynamicLinkParameters.DesktopUrl + "?" + dynamicLinkParameters.ParameterName + "=" + dynamicLinkParameters.ParameterId;

                var components = new DynamicLinkComponents(new Uri(baseLink), dynamicLinkParameters.Prefix);

                urlFormat = components.LongDynamicLink.AbsoluteUri;

                HelperFunctions.DevLogError(urlFormat);

                urlFormat += $"&ofl={baseLink}";
                urlFormat += $"&ifl={baseLink}";
                urlFormat += $"&ipfl={baseLink}";
                urlFormat += $"&afl={baseLink}";
                urlFormat += $"&apn={Application.identifier}";
                urlFormat += $"&ibi={Application.identifier}";
                urlFormat += $"&isi={APPSTORE_ID}";
                urlFormat += $"&st={dynamicLinkParameters.SocialMetaTagParameters.Title}";
                urlFormat += $"&si={dynamicLinkParameters.SocialMetaTagParameters.ImageUrl}";
                urlFormat += $"&sd={dynamicLinkParameters.SocialMetaTagParameters.Description}";
            } else {
                baseLink = dynamicLinkParameters.Prefix + "?" + dynamicLinkParameters.ParameterName + "=" + dynamicLinkParameters.ParameterId;

                var components = new DynamicLinkComponents(new Uri(baseLink), dynamicLinkParameters.Prefix);

                urlFormat = components.LongDynamicLink.AbsoluteUri;

                HelperFunctions.DevLogError(urlFormat);

                urlFormat += $"&ofl={dynamicLinkParameters.DesktopUrl}";
                urlFormat += $"&ifl={dynamicLinkParameters.DesktopUrl}";
                urlFormat += $"&ipfl={dynamicLinkParameters.DesktopUrl}";
                urlFormat += $"&afl={dynamicLinkParameters.DesktopUrl}";
                urlFormat += $"&apn={Application.identifier}";
                urlFormat += $"&ibi={Application.identifier}";
                urlFormat += $"&isi={APPSTORE_ID}";
                urlFormat += $"&st={dynamicLinkParameters.SocialMetaTagParameters.Title}";
                urlFormat += $"&si={dynamicLinkParameters.SocialMetaTagParameters.ImageUrl}";
                urlFormat += $"&sd={dynamicLinkParameters.SocialMetaTagParameters.Description}";
            }

            HelperFunctions.DevLogError(urlFormat);

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
