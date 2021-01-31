using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VersionChecker : MonoBehaviour
{
    public Action OnNeedUpdateApp;
    public Action OnCanUse;

    [SerializeField] GeneralAppAPIScriptableObject generalAppAPIScriptableObject;
    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] float delayRepeat = 2;

    public void RequestVersion() {
        webRequestHandler.GetRequest(GetRequestURL(), VersionCallBack, ErrorVersionCallBack);
    }

    //v4 just force update
    private void VersionCallBack(long code, string body) {
        AppVersionJsonData versionData = JsonParser.CreateFromJSON<AppVersionJsonData>(body);
        if(versionData == null)
            StartCoroutine(RepeatCheckVersion());

        if (versionData.versions.Count == 0) {
            OnCanUse?.Invoke();
            return;
        }

        AppVersionJsonData.Version minWithForceUpdateAppVersion = versionData.versions[0];
        float currentMinVersionWithForceUpdate = 0;
        float.TryParse(minWithForceUpdateAppVersion.min_support_version, out currentMinVersionWithForceUpdate);
        foreach (var version in versionData.versions) {
            float currentVersion = 0;

            float.TryParse(minWithForceUpdateAppVersion.min_support_version, out currentVersion);

            if (currentVersion > currentMinVersionWithForceUpdate && version.forced_update) {
                minWithForceUpdateAppVersion = version;
                currentMinVersionWithForceUpdate = currentVersion;
            }
        }

        float appVersion = 0;
        float.TryParse(Application.version, out appVersion);

        if (currentMinVersionWithForceUpdate > appVersion)
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

    private string GetRequestURL() {
        return webRequestHandler.ServerURLAuthAPI + generalAppAPIScriptableObject.Version;
    }
}
