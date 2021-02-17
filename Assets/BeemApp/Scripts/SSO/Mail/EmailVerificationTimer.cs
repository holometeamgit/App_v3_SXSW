using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Beem.SSO {

    /// <summary>
    /// Email Timer
    /// </summary>
    public class EmailVerificationTimer {

        private static DateTime currentDateTime;

        public static Action onStartTimer;

        public static Action onFinishTimer;

        /// <summary>
        /// Release Timer
        /// </summary>
        /// <param name="onEnd"></param>
        public static async void Release(int second = 60) {
            currentDateTime = DateTime.Now.AddSeconds(second);
            onStartTimer?.Invoke();
            TimeSpan timeSpan = GetTimeLeft(); ;
            await Task.Delay(timeSpan);
            onFinishTimer?.Invoke();
        }

        /// <summary>
        /// Get Time Left
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetTimeLeft() {
            return currentDateTime.Subtract(DateTime.Now);
        }

        /// <summary>
        /// Time Is Over
        /// </summary>
        public static bool IsOver {
            get {
                return DateTime.Now.CompareTo(currentDateTime) > 0;
            }
        }

    }
}
