﻿using Firebase.DynamicLinks;
using System;
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

    [Space]

    [SerializeField]
    private string firebaseDynamicLink = "https://join.beem.me";
    public string FirebaseDynamicLink {
        get {
            return firebaseDynamicLink;
        }
    }

    [SerializeField]
    private string firebaseStaticLink = "https://us-central1-test-36ceb.cloudfunctions.net";

    public string FirebaseStaticLink {
        get {
            return firebaseStaticLink;
        }
    }

    [SerializeField]
    private string firebaseAddUser = "/addusername/";

    public string FirebaseAddUser {
        get {
            return firebaseAddUser;
        }
    }

    [SerializeField]
    private string firebaseProfile = "/profile/";

    public string FirebaseProfile {
        get {
            return firebaseProfile;
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
    private string streamId = "streamId";

    public string StreamId {
        get {
            return streamId;
        }
    }

    [SerializeField]
    private string username = "username";

    public string Username {
        get {
            return username;
        }
    }

    [SerializeField]
    private string app = "/App/";

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

}
