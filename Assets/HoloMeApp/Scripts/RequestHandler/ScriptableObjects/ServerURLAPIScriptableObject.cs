using Firebase.DynamicLinks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerURLAPI", menuName = "Data/API/ServerURLAPI", order = 100)]
public class ServerURLAPIScriptableObject : ScriptableObject {

    public string ProdServerURLAuth = "https://api.holo.me/api-auth";
    public string ProdServerURLMedia = "https://api.holo.me/api-media";
    public string ProdServerProviders = "https://api.holo.me/oauth/providers/";
    [SerializeField]
    private string prodFirebaseDynamicLink = "https://join.beem.me";
    public string ProdFirebaseDynamicLink {
        get {
            return prodFirebaseDynamicLink;
        }
    }
    [Space]
    public string DevServerURLAuth = "https://devholo.me/api-auth";
    public string DevServerURLMedia = "https://devholo.me/api-media";
    public string DevServerProviders = "https://devholo.me/oauth/providers/";
    [SerializeField]
    private string devFirebaseDynamicLink = "https://us-central1-test-36ceb.cloudfunctions.net";
    public string DevFirebaseDynamicLink {
        get {
            return devFirebaseDynamicLink;
        }
    }

    [SerializeField]
    private string firebaseAddUserStaticLink = "https://us-central1-test-36ceb.cloudfunctions.net/addusername";

    public string FirebaseAddUserStaticLink {
        get {
            return firebaseAddUserStaticLink;
        }
    }

    [SerializeField]
    private string url = "https://beem.me";
    public string Url {
        get {
            return url;
        }
    }
    [SerializeField]
    private string room = "room";

    public string Room {
        get {
            return room;
        }
    }

    [SerializeField]
    private string stream = "stream";

    public string Stream {
        get {
            return stream;
        }
    }

    [SerializeField]
    private string notificationAccess = "NotificationAccess";

    public string NotificationAccess {
        get {
            return notificationAccess;
        }
    }

    [SerializeField]
    private string app = "App";

    public string App {
        get {
            return app;
        }
    }

    [SerializeField]
    private string logoLink = "https://s3.eu-west-2.amazonaws.com/prod.previews/Logo-Deep-Link-black.png";
    public string LogoLink {
        get {
            return logoLink;
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

    public string FirebaseDynamicLink {
        get {
#if DEV
            return DevFirebaseDynamicLink;
#else
            return ProdFirebaseDynamicLink;
#endif
        }
    }

}
