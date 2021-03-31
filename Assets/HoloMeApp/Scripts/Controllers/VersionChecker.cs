using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VersionChecker : MonoBehaviour {
    public Action OnNeedUpdateApp;
    public Action OnCanUse;

    [SerializeField] GeneralAppAPIScriptableObject generalAppAPIScriptableObject;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] float delayRepeat = 2;

    private const string DEFAULT_MIN_VERSION = "0";

    public void RequestVersion() {
        webRequestHandler.GetRequest(GetRequestURL(), VersionCallBack, ErrorVersionCallBack);
    }

    //v4 just force update
    private void VersionCallBack(long code, string body) {

        HelperFunctions.DevLog("VersionCallBack " + body);

        AppVersionJsonData versionData = JsonParser.CreateFromJSON<AppVersionJsonData>(body);
        if (versionData == null)
            StartCoroutine(RepeatCheckVersion());

        if (versionData.versions.Count == 0) {
            OnCanUse?.Invoke();
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
            OnNeedUpdateApp?.Invoke();
        else
            OnCanUse?.Invoke();
    }

    private void ErrorVersionCallBack(long code, string body) {
        StartCoroutine(RepeatCheckVersion());
    }

    IEnumerator RepeatCheckVersion() {
        yield return new WaitForSeconds(delayRepeat);
        RequestVersion();
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
        return webRequestHandler.ServerURLAuthAPI + generalAppAPIScriptableObject.Version;
    }
}
