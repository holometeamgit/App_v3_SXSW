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
    private List<GameObject> _activatedObjects;

    [SerializeField]
    private bool _requireSignIn = true;

    private CancellationTokenSource _cancelTokenSource;
    private CancellationToken _cancellationToken;
    private const int CHECK_COOLDOWN = 5000;

    private AuthController _authController;

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

        foreach (var obj in _activatedObjects) {
            if (obj.activeInHierarchy) {
                return true;
            }
        }

        return false;
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

    /// <summary>
    /// Cancel Request
    /// </summary>

    public void Cancel() {
        if (_cancelTokenSource != null) {
            _cancelTokenSource.Cancel();
            _cancelTokenSource = null;
        }
    }

    private void OnDestroy() {
        Cancel();
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

    /// <summary>
    /// Receive Data
    /// </summary>
    public void OnReceivedData(Action onSuccessTask) {
        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WaitForCanShow().ContinueWith((task) => {

            if (task.IsCanceled) {
                HelperFunctions.DevLog("Previouses deeplink request was interrupted");

            } else if (task.IsCompleted) {
                onSuccessTask?.Invoke();
            }
        }, taskScheduler);
    }
}
