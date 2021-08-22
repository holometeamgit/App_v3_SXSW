using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
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

        private static CancellationTokenSource _cancelTokenSource;
        private static CancellationToken _cancelToken;

        private const int TIME_REFRESH = 500;
        private static bool _isDisposed; 

        /// <summary>
        /// Cansel async Release
        /// </summary>
        public static void Cancel() {
            if (!_isDisposed && _cancelTokenSource != null && !_cancelTokenSource.IsCancellationRequested)
                _cancelTokenSource.Cancel();
        }

        /// <summary>
        /// Release Timer
        /// </summary>
        /// <param name="onEnd"></param>
        ///
        public static void Release(int second = 60) {

            Cancel();

            HelperFunctions.DevLog("Release " + DateTime.Now + " " + second);

            currentDateTime = DateTime.Now.AddSeconds(second);

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;

            _isDisposed = false;

            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            StartReleaseAsync(_cancelToken, second).ContinueWith((taskWebRequestData) => {
                _isDisposed = true;
                _cancelTokenSource.Dispose();
                currentDateTime = DateTime.Now;
            }, taskScheduler);
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

        private static async Task StartReleaseAsync(CancellationToken token, int second = 60) {
            TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            try {
                await ReleaseAsync(token, second);
            } catch (OperationCanceledException ex){
                HelperFunctions.DevLog("OperationCanceledException");
            }

        }

        private static async Task ReleaseAsync(CancellationToken token, int second = 60) {
            onStartTimer?.Invoke();

            HelperFunctions.DevLog("ReleaseAsync onStartTimer");

            while (GetTimeLeft().TotalSeconds > 0) {
                //HelperFunctions.DevLog("ReleaseAsync " + GetTimeLeft().TotalSeconds);
                if (token.IsCancellationRequested) {
                    HelperFunctions.DevLog("Wait resend verification interrupted");
                    token.ThrowIfCancellationRequested();
                }
                await Task.Delay(TIME_REFRESH);
            }

            HelperFunctions.DevLog("ReleaseAsync onFinishTimer");

            onFinishTimer?.Invoke();

        }

    }
}
