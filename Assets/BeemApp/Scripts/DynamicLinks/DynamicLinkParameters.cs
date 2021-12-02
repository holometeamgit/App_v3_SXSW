using Firebase.DynamicLinks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Firebase.DynamicLink {
    /// <summary>
    /// Link Parameters
    /// </summary>
    public class DynamicLinkParameters {

        public enum Query {
            username,
            message,
            live,
            prerecorded
        }

        public enum Folder {
            room,
            message,
            live,
            prerecorded
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

        public string Username {
            get {
                return _username;
            }
        }

        public string DynamicLinkURL {
            get {
                return _dynamicLinkURL;
            }
        }

        public Query FolderName {
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
        private string _username;
        private string _dynamicLinkURL;
        private Query _folderName;
        private SocialMetaTagParameters _socialMetaTagParameters;

        public DynamicLinkParameters(string prefix, string dynamicLinkURL, Query folderName, string id) {
            _prefix = prefix;
            _id = id;
            _dynamicLinkURL = dynamicLinkURL;
            _folderName = folderName;
        }

        public DynamicLinkParameters(string prefix, string dynamicLinkURL, Query folderName, string username, string id, SocialMetaTagParameters socialMetaTagParameters) {
            _prefix = prefix;
            _username = username;
            _id = id;
            _dynamicLinkURL = dynamicLinkURL;
            _folderName = folderName;
            _socialMetaTagParameters = socialMetaTagParameters;
        }

    }
}
