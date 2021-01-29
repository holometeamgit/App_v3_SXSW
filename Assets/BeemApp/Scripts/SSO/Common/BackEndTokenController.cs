using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
namespace Beem.SSO
{
    public class BackEndTokenController {
        public string GetToken(FirebaseUser user) {
            string idToken = string.Empty;
            user.TokenAsync(true).ContinueWith(taskTokenID => {
                if (taskTokenID.IsCanceled) {
                    Debug.LogError("TokenAsync was canceled.");
                }

                if (taskTokenID.IsFaulted) {
                    Debug.LogError("TokenAsync encountered an error: " + taskTokenID.Exception);
                }

                idToken = taskTokenID.Result;
                Debug.Log(idToken);
            });

            return idToken;
        }
    }
}
