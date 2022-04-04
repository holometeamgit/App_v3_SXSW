//Check app version before app start
//from v4 just force update requirements for app 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Beem {

    public class VersionChecker {
        public Action onSentNeedUpdateApp;
        public Action onSentCanUse;

        GeneralAppAPIScriptableObject _generalAppAPIScriptableObject;
        private WebRequestHandler _webRequestHandler;

        private const int DELAY_REPEAT = 2000;
        private const string DEFAULT_MIN_VERSION = "0";

        public VersionChecker(GeneralAppAPIScriptableObject generalAppAPIScriptableObject, WebRequestHandler webRequestHandler) {
            _generalAppAPIScriptableObject = generalAppAPIScriptableObject;
            _webRequestHandler = webRequestHandler;
        }

        //[Inject]
        public void Constructor(GeneralAppAPIScriptableObject generalAppAPIScriptableObject, WebRequestHandler webRequestHandler) {
            _generalAppAPIScriptableObject = generalAppAPIScriptableObject;
            _webRequestHandler = webRequestHandler;
        }

        public void RequestVersion() {
            _webRequestHandler.Get(GetRequestURL(), VersionCallBack, ErrorVersionCallBack, false);
        }

        private void VersionCallBack(long code, string body) {

            HelperFunctions.DevLog("VersionCallBack " + body);

            AppVersionJsonData versionData = JsonParser.CreateFromJSON<AppVersionJsonData>(body);
            if (versionData == null)
                RepeatCheckVersion();

            if (versionData.versions.Count == 0) {
                onSentCanUse?.Invoke();
                return;
            }

            string currentMinVersionWithForceUpdate = DEFAULT_MIN_VERSION;
            foreach (var version in versionData.versions) {

#if UNITY_IOS
                if (version.platform != AppVersionJsonData.IOS_PLATFORM) {
                    continue;
                }
#elif UNITY_ANDROID
                if (version.platform != AppVersionJsonData.ANDROID_PLATFORM) {
                    continue;
                }
#endif
                if (CompareVersions(version.min_support_version, currentMinVersionWithForceUpdate) > 0 && version.forced_update) {
                    currentMinVersionWithForceUpdate = version.min_support_version;
                }
            }
            if (CompareVersions(currentMinVersionWithForceUpdate, Application.version) > 0)
                onSentNeedUpdateApp?.Invoke();
            else
                onSentCanUse?.Invoke();
        }

        private void ErrorVersionCallBack(long code, string body) {
            RepeatCheckVersion();
        }

        private void RepeatCheckVersion() {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Delay(DELAY_REPEAT).ContinueWith(_ => RequestVersion(), taskScheduler);
        }

        private int CompareVersions(string version1, string version2) {
            string[] subversions1 = version1.Split('.');
            string[] subversions2 = version2.Split('.');

            int countSubversion = Mathf.Min(subversions1.Length, subversions2.Length);

            for (int i = 0; i < countSubversion; i++) {
                int version1Number = 0;
                int version2Number = 0;

                int.TryParse(subversions1[i], out version1Number);
                int.TryParse(subversions2[i], out version2Number);
                if (version1Number > version2Number) {
                    return 1;
                } else if (version1Number < version2Number) {
                    return -1;
                }
            }

            return 0;
        }

        private string GetRequestURL() {
            return _webRequestHandler.ServerURLAuthAPI + _generalAppAPIScriptableObject.Version;
        }
    }
}