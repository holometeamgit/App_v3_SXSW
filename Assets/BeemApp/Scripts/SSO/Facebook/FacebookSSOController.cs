using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facebook.Unity;
using Firebase.Auth;
using UnityEngine;

namespace Beem.SSO {

    /// <summary>
    /// Sign In and Sign Up via Facebook
    /// </summary>
    public class FacebookSSOController : AbstractFirebaseController {

        private List<string> permissions = new List<string>() { "public_profile", "email" };

        protected override void Subscribe() {
            CallBacks.onSignInFacebook += SignIn;
        }

        protected override void Unsubscribe() {
            CallBacks.onSignInFacebook -= SignIn;
        }

        private void Awake() {
            FBInit();
        }

        private void FBInit() {
            if (!FB.IsInitialized) {
                FB.Init(InitCallback);
            } else {
                FB.ActivateApp();
            }
        }
        private void InitCallback() {
            if (FB.IsInitialized) {
                FB.ActivateApp();
            } else {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void SignIn() {
            if (!FB.IsInitialized) {
                FBInit();
                return;
            }
            FB.LogInWithReadPermissions(permissions, AuthCallback);
        }

        private void AuthCallback(ILoginResult result) {
            if (FB.IsLoggedIn) {
                Debug.Log("Log in Facebook");
                Credential credential = FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                _auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                    CheckTask(task, () => CallBacks.onFirebaseSignInSuccess?.Invoke(LogInType.Facebook), CallBacks.onFail);
                }, taskScheduler);
            } else {
                CallBacks.onFail?.Invoke("User cancelled login");
                Debug.Log("User cancelled login");
            }
        }

    }
}
