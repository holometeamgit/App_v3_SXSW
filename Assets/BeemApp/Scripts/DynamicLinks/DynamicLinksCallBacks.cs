using System;
using Firebase.DynamicLinks;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// on Get Short Link
    /// </summary>
    public class DynamicLinksCallBacks {
        public static Action<Uri, SocialMetaTagParameters> onShareSocialLink = delegate { };
        public static Action<Uri> onShareLink = delegate { };
        public static Action onShareAppLink = delegate { };
        public static Action<string> onReceivedDeepLink = delegate { };
    }
}
