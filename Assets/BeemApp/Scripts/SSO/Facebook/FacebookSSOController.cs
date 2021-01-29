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

        public void SignIn() {
            Debug.Log("Facebooc log in new 1 " + FB.IsInitialized);
            if (!FB.IsInitialized) {
                FBInit();
                return;
            }
            Debug.Log("Facebooc log in new 2 " + FB.IsInitialized);
            FB.LogInWithReadPermissions(permissions, AuthCallback);
        }

        private void AuthCallback(ILoginResult result) {
            if (FB.IsLoggedIn) {
                Credential credential = FacebookAuthProvider.GetCredential(AccessToken.CurrentAccessToken.TokenString);
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                _auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                    CheckTask(task, CallBacks.onSignInSuccess, CallBacks.onFail);
                }, taskScheduler);
            } else {
                Debug.Log("User cancelled login");
            }
        }

    }
}
