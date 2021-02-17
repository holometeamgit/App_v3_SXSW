using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using System;

namespace Beem.SSO
{
    public class BackEndTokenController {
        public void GetToken(FirebaseUser user, Action<string> onSuccess, Action<string> onFail) {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            user.TokenAsync(true).ContinueWith(taskTokenID => {
                if (taskTokenID.IsCanceled) {
                    onFail?.Invoke("TokenAsync was canceled.");
                }

                if (taskTokenID.IsFaulted) {
                    onFail?.Invoke("TokenAsync encountered an error: " + taskTokenID.Exception);
                }

                onSuccess?.Invoke(taskTokenID.Result);
            }, taskScheduler);
        }
    }
}
