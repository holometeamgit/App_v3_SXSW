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
            slug,
            username
        }

        public enum Folder {
            stream,
            room
        }

        public string Prefix {
            get {
                return _prefix;
            }
        }

        public string Id {
            get {
                return _id;
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

        public Folder FolderName {
            get {
                return _folderName;
            }
        }

        public SocialMetaTagParameters SocialMetaTagParameters {
            get {
                return _socialMetaTagParameters;
            }
        }

        private string _prefix = default;
        private string _id;
        private string _dynamicLinkURL;
        private Parameter _parameterName;
        private Folder _folderName;
        private SocialMetaTagParameters _socialMetaTagParameters;

        public DynamicLinkParameters(string prefix, string dynamicLinkURL, Parameter parameterName, string id) {
            _prefix = prefix;
            _id = id;
            _dynamicLinkURL = dynamicLinkURL;
            _parameterName = parameterName;
        }

        public DynamicLinkParameters(string prefix, string dynamicLinkURL, Parameter parameterName, string id, SocialMetaTagParameters socialMetaTagParameters) {
            _prefix = prefix;
            _id = id;
            _dynamicLinkURL = dynamicLinkURL;
            _parameterName = parameterName;
            _socialMetaTagParameters = socialMetaTagParameters;
        }

        public DynamicLinkParameters(string prefix, string dynamicLinkURL, Folder folderName, string id) {
            _prefix = prefix;
            _id = id;
            _dynamicLinkURL = dynamicLinkURL;
            _folderName = folderName;
        }

        public DynamicLinkParameters(string prefix, string dynamicLinkURL, Folder folderName, string id, SocialMetaTagParameters socialMetaTagParameters) {
            _prefix = prefix;
            _id = id;
            _dynamicLinkURL = dynamicLinkURL;
            _folderName = folderName;
            _socialMetaTagParameters = socialMetaTagParameters;
        }

    }
}
