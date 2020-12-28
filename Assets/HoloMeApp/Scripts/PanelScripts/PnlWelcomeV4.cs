using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TODO all this class temp
public class PnlWelcomeV4 : MonoBehaviour {
    [Serializable]
    public class Providers {
        public List<string> providers;

        public Providers() {
            providers = new List<string>();
        }
    }

    [SerializeField]
    WebRequestHandler webRequestHandler;
    [SerializeField]
    GameObject LogInFBGO;

    [Space]
    [SerializeField]
    AppleAccountManager AppleAccountManager;
    [SerializeField]
    GameObject LogInAppleGO;

    [Space]
    [SerializeField]
    GameObject LogInGoogleGO;

    private void OnEnable() {
        webRequestHandler.GetRequest(webRequestHandler.ServerProvidersAPI, EnableFB, (key, body) => { }, null);
    }

    private void EnableFB(long key, string body) {
        try {
            Providers providers = JsonUtility.FromJson<Providers>(body);
            if (providers != null) {
                LogInFBGO.SetActive(providers.providers.Contains("fb"));
                LogInAppleGO.SetActive(providers.providers.Contains("apple"));
                LogInGoogleGO.SetActive(providers.providers.Contains("google"));
            }
        } catch (Exception e) { }
    }
}
