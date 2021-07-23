using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using Firebase.Auth;

namespace Beem.SSO {
    /// <summary>
    /// Sign In and Sign Up via Google
    /// </summary>
    public class GoogleSSOController : AbstractFirebaseController {

        //private GoogleSignIn googleSignIn;
        private const string WEB_CLIENT_ID = "233061171188-67n8vv3f0kvnk7fhujm98kmthvc4mqtq.apps.googleusercontent.com";
        private GoogleSignInConfiguration configuration;

        protected override void Subscribe() {
            CallBacks.onSignInGoogle += SignInWithGoogle;
        }

        protected override void Unsubscribe() {
            CallBacks.onSignInGoogle -= SignInWithGoogle;
        }

        private void Awake() {
#if !UNITY_EDITOR
            Initialize();
#endif
        }

        protected void Initialize() {
            configuration = new GoogleSignInConfiguration {
                RequestIdToken = true,
                // Copy this value from the google-service.json file.
                // oauth_client with type == 3
                WebClientId = WEB_CLIENT_ID
            };
            //googleSignIn = GoogleSignIn.DefaultInstance;
        }


        private void SignInWithGoogle() {
            try {

                HelperFunctions.DevLog("CallSignInWithGoogle");
                GoogleSignIn.Configuration = configuration;
                GoogleSignIn.Configuration.UseGameSignIn = false;
                GoogleSignIn.Configuration.RequestIdToken = true;

                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticationFinished, taskScheduler);
            } catch (Exception e) {
                HelperFunctions.DevLogError(e.Message);
                CallBacks.onFail?.Invoke(e.Message);
            }
        }

        private void OnGoogleAuthenticationFinished(Task<GoogleSignInUser> task) {
            if (task.IsCanceled) {
                HelperFunctions.DevLogError("User canceled login");
                CallBacks.onFail?.Invoke(string.Empty);
                return;
            }
            if (task.IsFaulted) {
                HelperFunctions.DevLogError("Task was Failed: " + task.Exception);
                CallBacks.onFail?.Invoke(string.Empty);
                return;
            }

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Credential credential =
                    GoogleAuthProvider.GetCredential(task.Result.IdToken, task.Result.AuthCode);
            _auth.SignInWithCredentialAsync(credential).ContinueWith(signIntask => {
                CheckTask(signIntask, () => CallBacks.onFirebaseSignInSuccess?.Invoke(LogInType.Google), CallBacks.onFail);
            }, taskScheduler);
        }
    }
}