using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerURLAPI", menuName = "Data/API/ServerURLAPI", order = 100)]
public class ServerURLAPIScriptableObject : ScriptableObject {

    public string ProdServerURLAuth = "https://api.holo.me/api-auth";
    public string ProdServerURLMedia = "https://api.holo.me/api-media";
    public string ProdServerProviders = "https://api.holo.me/oauth/providers/";
    public string ProdFirebaseDynamicLink = "https://join.beem.me";
    [Space]
    public string DevServerURLAuth = "https://devholo.me/api-auth";
    public string DevServerURLMedia = "https://devholo.me/api-media";
    public string DevServerProviders = "https://devholo.me/oauth/providers/";
    public string DevFirebaseDynamicLink = "https://join.beem.me";
    [Space]
    [SerializeField]
    private string room = "room";

    public string Room {
        get {
            return room;
        }
    }

    [SerializeField]
    private string app = "App";

    public string App {
        get {
            return app;
        }
    }

    public string ServerURLAuthAPI {
        get {
#if DEV
            return DevServerURLAuth;
#else
            return ProdServerURLAuth;
#endif              
        }
    }
    public string ServerURLMediaAPI {
        get {
#if DEV
            return DevServerURLMedia;
#else
            return ProdServerURLMedia;
#endif
        }
    }

    public string ServerProvidersAPI {
        get {
#if DEV
            return DevServerProviders;
#else
            return ProdServerProviders;
#endif
        }
    }


    public string FirebaseDynamicLinkAPI {
        get {
#if DEV
            return DevFirebaseDynamicLink;
#else
            return ProdFirebaseDynamicLink;
#endif
        }
    }
}
