using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ARMsgDeeplinkConstructor : MonoBehaviour {

    [SerializeField]
    private PopupShowChecker _popupShowChecker;

    public static Action<ARMsgJSON.Data> OnActivated = delegate { };

    private CancellationTokenSource _showCancellationTokenSource;
    private CancellationToken _showCancellationToken;
    private const int CHECK_COOLDOWN = 5000;

    private void OnEnable() {
        OnActivated += Activate;
    }

    private void OnDisable() {
        OnActivated -= Activate;
    }

    private void Activate(ARMsgJSON.Data data) {
        OnReceivedARMessageData(data, ActivateData);
    }

    private void OnReceivedARMessageData(ARMsgJSON.Data data, Action<ARMsgJSON.Data> onSuccessTask) {
        if (_showCancellationTokenSource != null) {
            _showCancellationTokenSource.Cancel();
            _showCancellationTokenSource.Dispose();
        }

        _showCancellationTokenSource = new CancellationTokenSource();
        _showCancellationToken = _showCancellationTokenSource.Token;


        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WaitForCanShow().ContinueWith((task) => {

            if (task.IsCanceled) {
                HelperFunctions.DevLog("Previouses room deeplink request was interrupted");

            } else if (task.IsCompleted) {
                onSuccessTask?.Invoke(data);
            }
        }, taskScheduler);
    }

    private async Task WaitForCanShow() {
        while (!_popupShowChecker.CanShow()) {
            if (_showCancellationToken.IsCancellationRequested) {
                _showCancellationToken.ThrowIfCancellationRequested();
            }
            await Task.Delay(CHECK_COOLDOWN);
        }
    }

    private void ActivateData(ARMsgJSON.Data data) {
        StreamCallBacks.onPlayARMessage?.Invoke(data);
    }

}
