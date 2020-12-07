using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerURLAPI", menuName = "Data/API/ServerURLAPI", order = 100)]
public class ServerURLAPIScriptableObject : ScriptableObject {

    public string ProdServerURLAuth = "https://api.holo.me/api-auth";
    public string ProdServerURLMedia = "https://api.holo.me/api-media";
    public string ProdServerProviders = "https://api.holo.me/oauth/providers/";
    [Space]
    public string DevServerURLAuth = "https://devholo.me/api-auth";
    public string DevServerURLMedia = "https://devholo.me/api-media";
    public string DevServerProviders = "https://devholo.me/oauth/providers/";


    public string ServerURLAuthAPI {
        get {
            return Debug.isDebugBuild ? DevServerURLAuth : ProdServerURLAuth; }
    }
    public string ServerURLMediaAPI { 
        get {return Debug.isDebugBuild? DevServerURLMedia : ProdServerURLMedia; }
    }

    public string ServerProvidersAPI {
        get { return Debug.isDebugBuild ? DevServerProviders : ProdServerProviders; }
    }
}
