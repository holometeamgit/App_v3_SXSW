using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;

public class AppleAccountManager : MonoBehaviour
{
    [SerializeField]
    AccountManager accountManager;
    [SerializeField]
    WebRequestHandler webRequestHandler;

    [Space]
    [SerializeField]
    AuthorizationAPIScriptableObject authorizationAPI;

    void Awake() {
        Init();
    }

    private void Init() {
        
    }

}
