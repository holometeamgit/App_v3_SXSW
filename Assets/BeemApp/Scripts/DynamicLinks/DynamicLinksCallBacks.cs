using System;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// on Get Short Link
    /// </summary>
    public class DynamicLinksCallBacks {
        public static Action<Uri> onGetShortLink = delegate { };
        public static Action<string> onReceivedDeepLink = delegate { };
    }
}
