using Firebase.DynamicLinks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Firebase.DynamicLink {
    /// <summary>
    /// Link Parameters
    /// </summary>
    public class DynamicLinkParameters {

        public enum Parameter {
            streamId,
            username
        }

        public string Prefix {
            get {
                return _prefix;
            }
        }

        public string ParameterId {
            get {
                return _parameterId;
            }
        }

        public string DynamicLinkURL {
            get {
                return _dynamicLinkURL;
            }
        }


        public Parameter ParameterName {
            get {
                return _parameterName;
            }
        }

        public SocialMetaTagParameters SocialMetaTagParameters {
            get {
                return _socialMetaTagParameters;
            }
        }

        private string _prefix = default;
        private string _parameterId;
        private string _dynamicLinkURL;
        private Parameter _parameterName;
        private SocialMetaTagParameters _socialMetaTagParameters;

        public DynamicLinkParameters(string prefix, string dynamicLinkURL, Parameter parameterName, string parameterId) {
            _prefix = prefix;
            _parameterId = parameterId;
            _dynamicLinkURL = dynamicLinkURL;
            _parameterName = parameterName;
        }

        public DynamicLinkParameters(string prefix, string dynamicLinkURL, Parameter parameterName, string parameterId, SocialMetaTagParameters socialMetaTagParameters) {
            _prefix = prefix;
            _parameterId = parameterId;
            _dynamicLinkURL = dynamicLinkURL;
            _parameterName = parameterName;
            _socialMetaTagParameters = socialMetaTagParameters;
        }

    }
}
