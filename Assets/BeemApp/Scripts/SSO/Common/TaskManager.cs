using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace Beem.SSO {

    /// <summary>
    /// Task Manager
    /// </summary>
    public class TaskManager {

        /// <summary>
        /// Check Task with parameter
        /// </summary>
        /// <param name="task"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        public void CheckTask<T>(Task<T> task, Action onSuccess = null, Action<string> onFail = null) {
            if (task.IsCanceled) {
                Debug.LogError("Task was canceled.");
                onFail?.Invoke("Cancel");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("Task encountered an error: " + task.Exception);
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                onFail?.Invoke(errorCode.ToString());
                return;
            }
            onSuccess?.Invoke();
        }

        /// <summary>
        /// Check Task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        public void CheckTask(Task task, Action onSuccess = null, Action<string> onFail = null) {
            if (task.IsCanceled) {
                Debug.LogError("Task was canceled.");
                onFail?.Invoke("Cancel");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("Task encountered an error: " + task.Exception);
                FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                onFail?.Invoke(errorCode.ToString());
                return;
            }
            onSuccess?.Invoke();
        }
    }
}
