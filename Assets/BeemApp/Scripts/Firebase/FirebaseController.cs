using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using UnityEngine;

namespace Beem.Firebase {

    /// <summary>
    /// Main Firebase Class
    /// </summary>
    public class FirebaseController : MonoBehaviour {

        private void Awake() {
            Initialize();
        }

        private void Initialize() {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                if (task.Result == DependencyStatus.Available) {
                    FirebaseCallBacks.onInit?.Invoke();
                } else {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                }
            }, taskScheduler);
        }
    }
}
