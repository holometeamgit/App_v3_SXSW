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

            Debug.Log("CallSignInWithGoogle");
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticationFinished, taskScheduler);
        }

        private void OnGoogleAuthenticationFinished(Task<GoogleSignInUser> task) {
            if (task.IsCanceled) {
                Debug.LogError("Task was canceled.");
                CallBacks.onFail?.Invoke("User cancelled login");
                return;
            }
            if (task.IsFaulted) {
                IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator();
                if (enumerator.MoveNext()) {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    CallBacks.onFail?.Invoke("Got Error: " + error.Status + " " + error.Message);
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                } else {
                    CallBacks.onFail?.Invoke("Got Unexpected Exception?!?" + task.Exception);
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
                return;
            }

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Credential credential =
                    GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
            _auth.SignInWithCredentialAsync(credential).ContinueWith(signIntask => {
                CheckTask(signIntask, () => CallBacks.onFirebaseSignInSuccess?.Invoke(LogInType.Google), CallBacks.onFail);
            }, taskScheduler);
        }
    }
}