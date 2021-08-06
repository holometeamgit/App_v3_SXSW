using CleverTap;
using UnityEngine;

namespace Beem.Firebase.FCM {
    /// <summary>
    /// Deep Link Receiver
    /// </summary>
    public class FCMController : MonoBehaviour {


        private void OnEnable() {
            FirebaseCallBacks.onInit += OnInit;
        }

        protected void OnDisable() {
            FirebaseCallBacks.onInit -= OnInit;
        }

        private void OnInit() {
#if UNITY_ANDROID
            CleverTapBinding.CreateNotificationChannel("Default", "Default", "Default", 1, true);
#elif UNITY_IOS
            CleverTapBinding.RegisterPush();
#endif
        }

    }
}
