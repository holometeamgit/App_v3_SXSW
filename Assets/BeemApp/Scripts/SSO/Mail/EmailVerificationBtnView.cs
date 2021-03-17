using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Beem.SSO {

    /// <summary>
    /// Email Verification Btn View
    /// </summary>
    public class EmailVerificationBtnView : MonoBehaviour {

        [SerializeField]
        private UnityEvent onStartTimer;
        [SerializeField]
        private UnityEvent onFinishTimer;

        private void OnEnable() {
            EmailVerificationTimer.onStartTimer += StartTimer;
            EmailVerificationTimer.onFinishTimer += FinishTimer;

            if (EmailVerificationTimer.IsOver) {
                FinishTimer();
            } else {
                StartTimer();
            }
        }

        private void OnDisable() {
            EmailVerificationTimer.onStartTimer -= StartTimer;
            EmailVerificationTimer.onFinishTimer -= FinishTimer;
        }

        private void StartTimer() {
            onStartTimer?.Invoke();
        }

        private void FinishTimer() {
            onFinishTimer?.Invoke();
        }
    }
}
