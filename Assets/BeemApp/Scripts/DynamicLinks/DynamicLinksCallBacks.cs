﻿using System;
using Firebase.DynamicLinks;

namespace Beem.Firebase.DynamicLink {

    /// <summary>
    /// on Get Short Link
    /// </summary>
    public class DynamicLinksCallBacks {
        public static Action<string> onReceivedDeepLink = delegate { };
    }
}
