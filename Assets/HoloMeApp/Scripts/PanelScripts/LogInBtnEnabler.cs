using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.SSO;

public class LogInBtnEnabler : MonoBehaviour {
    [Serializable]
    public class Providers {
        public List<string> providers;

        public Providers() {
            providers = new List<string>();
        }
    }

    [SerializeField]
    GameObject LogInAppleGO;
    [SerializeField]
    GameObject LogInGoogleGO;

    [Space]
    [SerializeField]
    private WebRequestHandler _webRequestHandler;

    public void CheckPlatform() {
#if !UNITY_IOS
        LogInAppleGO.SetActive(false);
#endif
    }

    private void EnableFB(long key, string body) {
        try {
            Providers providers = JsonUtility.FromJson<Providers>(body);
            if (providers != null) {
#if UNITY_IOS
                LogInAppleGO.SetActive(providers.providers.Contains("apple"));
#endif
                LogInGoogleGO.SetActive(providers.providers.Contains("google"));
            }
        } catch (Exception e) {
            HelperFunctions.DevLogError(e.Message);
        }
    }

    private void OnEnable() {
        CheckPlatform();
        _webRequestHandler.Get(_webRequestHandler.ServerProvidersAPI, EnableFB, (key, body) => { }, needHeaderAccessToken: false);
    }
}
