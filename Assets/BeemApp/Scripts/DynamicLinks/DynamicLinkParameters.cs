using Firebase.DynamicLinks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Firebase.DynamicLink {
    /// <summary>
    /// Link Parameters
    /// </summary>
    public class DynamicLinkParameters {

        public string Prefix {
            get {
                return _prefix;
            }
        }

        public string ParameterName {
            get {
                return _parameterName;
            }
        }

        public string Id {
            get {
                return _id;
            }
        }

        public string DesktopUrl {
            get {
                return _desktopURL;
            }
        }

        public SocialMetaTagParameters SocialMetaTagParameters {
            get {
                return _socialMetaTagParameters;
            }
        }

        private string _prefix = default;
        private string _parameterName = default;
        private string _id;
        private string _desktopURL;
        private SocialMetaTagParameters _socialMetaTagParameters;

        public DynamicLinkParameters(string prefix, string parameterName, string id) {
            _prefix = prefix;
            _parameterName = parameterName;
            _id = id;
        }

        public DynamicLinkParameters(string prefix, string parameterName, string id, string desktopURL) {
            _prefix = prefix;
            _parameterName = parameterName;
            _id = id;
            _desktopURL = desktopURL;
        }

        public DynamicLinkParameters(string prefix, string parameterName, string id, string desktopURL, SocialMetaTagParameters socialMetaTagParameters) {
            _prefix = prefix;
            _parameterName = parameterName;
            _id = id;
            _desktopURL = desktopURL;
            _socialMetaTagParameters = socialMetaTagParameters;
        }

    }
}
