using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TODO all this class temp
public class PnlWelcomeV4 : MonoBehaviour
{
    [Serializable]
    public class Providers {
        public List<string> providers;

        public Providers() {
            providers = new List<string>();
        }
    }

    [SerializeField] WebRequestHandler webRequestHandler;
    [SerializeField] GameObject LogInFBGO;

    private void OnEnable() {
        webRequestHandler.GetRequest(webRequestHandler.ServerProvidersAPI, EnableFB, (key, body) => { }, null);
    }

    private void EnableFB(long key, string body) {
        //        Debug.Log(body);
        //        body = body.Replace("\"fb\"", "");
        //        Debug.Log(body);
        try {
            Providers providers = JsonUtility.FromJson<Providers>(body);
            if (providers != null)
                LogInFBGO.SetActive(providers.providers.Contains("fb"));
        } catch (Exception e) { }
    }
}
