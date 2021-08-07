using System;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// on Get Short Link
    /// </summary>
    public class DynamicLinksCallBacks {
        public static Action<DynamicLinkParameters, string> onCreateShortLink = delegate { };
        public static Action<Uri, string> onGetShortLink = delegate { };
        public static Action onShareAppLink = delegate { };
        public static Action<string> onReceivedDeepLink = delegate { };
    }
}
