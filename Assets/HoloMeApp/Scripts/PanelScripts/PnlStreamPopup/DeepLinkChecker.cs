using Beem.SSO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// checks requirements for opening PnlRoomPopup
/// </summary>
public class DeepLinkChecker : MonoBehaviour {

    [SerializeField]
    private bool _requireSignIn = true;

    [SerializeField]
    private bool _requireHomeScreen = true;

    private AuthController _authController;

    private CancellationTokenSource _cancelTokenSource;
    private CancellationToken _cancellationToken;
    private const int CHECK_COOLDOWN = 5000;

    [Inject]
    public void Construct(AuthController authController) {
        _authController = authController;
    }

    /// <summary>
    /// Condition for DeepLink PopUp
    /// </summary>
    /// <returns></returns>

    public bool CanShow() {

        if (_requireSignIn) {
            if (!_authController.HasUser()) {
                return false;
            }
        }

        if (_requireHomeScreen) {
            if (!BottomMenuConstructor.IsActive) {
                return false;
            }
        }

        return true;
    }

    private async Task WaitForCanShow() {
        _cancelTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancelTokenSource.Token;
        try {
            while (!CanShow() && !_cancellationToken.IsCancellationRequested) {
                await Task.Delay(CHECK_COOLDOWN);
            }
        } finally {
            if (_cancelTokenSource != null) {
                _cancelTokenSource.Dispose();
                _cancelTokenSource = null;
            }
        }
    }

    private void OnDestroy() {
        if (_cancelTokenSource != null) {
            _cancelTokenSource.Cancel();
            _cancelTokenSource = null;
        }
    }

    /// <summary>
    /// Receive Data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="onSuccessTask"></param>
    public void OnReceivedData<T>(T data, Action<T> onSuccessTask) {
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WaitForCanShow().ContinueWith((task) => {

            if (task.IsCanceled) {
                HelperFunctions.DevLog("Previouses deeplink request was interrupted");

            } else if (task.IsCompleted) {
                onSuccessTask?.Invoke(data);
            }
        }, taskScheduler);
    }
}
