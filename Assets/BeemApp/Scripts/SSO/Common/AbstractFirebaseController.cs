using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Beem.SSO {

    /// <summary>
    /// Abstract Controller for firebase
    /// </summary>
    public abstract class AbstractFirebaseController : MonoBehaviour {

        protected FirebaseAuth _auth;

        protected abstract void Subscribe();

        protected abstract void Unsubscribe();

        protected void OnDestroy() {
            Unsubscribe();
        }

        /// <summary>
        /// Init Firebase
        /// </summary>
        /// <param name="auth"></param>
        public void InitializeFirebase(FirebaseAuth auth) {
            _auth = auth;
            Subscribe();
        }

        protected void CheckTask(Task<FirebaseUser> task, Action onSuccess = null, Action<string> onFail = null) {
            Debug.Log(task.Result.ProviderId);
            Debug.Log(task.Result.UserId);
            Debug.Log(task.Result);

            Firebase.Auth.FirebaseUser user = _auth.CurrentUser;
            user.TokenAsync(true).ContinueWith(taskTokenID => {
                if (taskTokenID.IsCanceled) {
                    Debug.LogError("TokenAsync was canceled.");
                    return;
                }

                if (taskTokenID.IsFaulted) {
                    Debug.LogError("TokenAsync encountered an error: " + taskTokenID.Exception);
                    return;
                }

                string idToken = taskTokenID.Result;
                Debug.Log(idToken);

                // Send token to your backend via HTTPS
                // ...
            });


            TaskManager firebaseTask = new TaskManager();
            firebaseTask.CheckTask(task, onSuccess, onFail);
        }
    }
}
