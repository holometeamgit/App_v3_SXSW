using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;

namespace Beem.SSO {

    /// <summary>
    /// Controller for Auth
    /// </summary>
    public class AuthController : MonoBehaviour {
        [Header("Firebase")]
        [SerializeField]
        private DependencyStatus _dependencyStatus;
        [Header("Controllers")]
        [SerializeField]
        private List<AbstractFirebaseController> abstractFirebaseControllers = new List<AbstractFirebaseController>();
        private FirebaseAuth _auth;

        private void Awake() {
            Initialize();
        }

        private void Initialize() {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                _dependencyStatus = task.Result;
                if (_dependencyStatus == DependencyStatus.Available) {
                    InitializeFirebase();
                } else {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + _dependencyStatus);
                }
            }, taskScheduler);
        }

        private void InitializeFirebase() {
            _auth = FirebaseAuth.DefaultInstance;
            foreach (AbstractFirebaseController item in abstractFirebaseControllers) {
                item.InitializeFirebase(_auth);
            }
        }

    }
}
