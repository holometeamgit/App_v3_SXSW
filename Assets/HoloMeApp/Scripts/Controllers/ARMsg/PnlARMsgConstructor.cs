using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.ARMsg;
using Beem.Permissions;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// Constructor for PnlARMessage
/// </summary>
public class PnlARMsgConstructor : MonoBehaviour {
    [SerializeField]
    private UIThumbnailsController _uiThumbnailsController;
    [SerializeField]
    private PermissionController _permissionController;
    [SerializeField]
    private PnlARMessages _pnlARMessages;
    [SerializeField]
    private GameObject _pnlARMessagesSteps;
    [SerializeField]
    private PnlViewingExperience _pnlViewingExperience;
    [SerializeField]
    private PopupShowChecker _popupShowChecker;
    [SerializeField]
    private PopupShowChecker _arMessagePopupChecker;

    private CancellationTokenSource _showCancellationTokenSource;
    private CancellationToken _showCancellationToken;

    private ARMsgJSON.Data currentData;
    private const int CHECK_COOLDOWN = 5000;

    private void Activate(bool status) {
        Debug.LogError("Activate");
        _pnlARMessagesSteps?.SetActive(status);
    }


    private void ActivateARena(ARMsgJSON.Data data) {
        Debug.LogError("ActivateARena");
        OnReceivedARMessageData(data, ActivateData);
    }

    private void ActivateData(ARMsgJSON.Data data) {
        Debug.LogError("ActivateData");
        currentData = data;

        Debug.LogError("_pnlViewingExperience");
        _pnlViewingExperience.ActivateForARMessaging(data);

        Debug.LogError("_pnlARMessages");
        _pnlARMessages.Init(data);

        Debug.LogError("OnPlayFromUser");
        _uiThumbnailsController.OnPlayFromUser?.Invoke(data.user);

        Debug.LogError("OnActivated 0");
        CallBacks.OnActivated?.Invoke(false);

        Debug.LogError("OnCancelAllARMsgActions");
        CallBacks.OnCancelAllARMsgActions?.Invoke();
        Debug.LogError("OnCancelAllARMsgActions2");
    }

    private async Task WaitForCanShow() {
        Debug.LogError($"WaitForCanShow, _popupShowChecker.CanShow() = {_popupShowChecker.CanShow()}, _arMessagePopupChecker.CanShow() = {_arMessagePopupChecker.CanShow()}");
        while (!_popupShowChecker.CanShow() && !_arMessagePopupChecker.CanShow()) {
            Debug.LogError($"_popupShowChecker.CanShow() = {_popupShowChecker.CanShow()}, _arMessagePopupChecker.CanShow() = {_arMessagePopupChecker.CanShow()}");
            if (_showCancellationToken.IsCancellationRequested) {
                _showCancellationToken.ThrowIfCancellationRequested();
            }
            await Task.Delay(CHECK_COOLDOWN);
        }
    }

    private void OnReceivedARMessageData(ARMsgJSON.Data data, Action<ARMsgJSON.Data> onSuccessTask) {
        Debug.LogError("OnReceivedARMessageData");
        if (_showCancellationTokenSource != null) {
            _showCancellationTokenSource.Cancel();
            _showCancellationTokenSource.Dispose();
        }

        _showCancellationTokenSource = new CancellationTokenSource();
        _showCancellationToken = _showCancellationTokenSource.Token;


        TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        WaitForCanShow().ContinueWith((task) => {

            if (task.IsCanceled) {
                Debug.LogError("IsCanceled");
                HelperFunctions.DevLog("Previouses room deeplink request was interrupted");

            } else if (task.IsCompleted) {
                Debug.LogError("IsCompleted");
                onSuccessTask?.Invoke(data);

            } else {
                Debug.LogError("IsFailed");
            }
        }, taskScheduler);
    }

    private void DeactivateARena() {
        Debug.LogError("DeactivateARena");
        WarningConstructor.ActivateDoubleButton("Before you go...",
           "If you exit you could lose your AR message if you don't share the link.",
           "Copy link and exit", "Return",
           () => {
               GUIUtility.systemCopyBuffer = currentData.share_link;
               Close();
           },
           null,
           false);

    }

    private void Close() {
        Debug.LogError("Close");
        _pnlViewingExperience.StopExperience();
        _pnlARMessages.Deactivate();
        MenuConstructor.OnActivated?.Invoke(true);
        HomeScreenConstructor.OnActivated?.Invoke(true);
    }


    private void OnEnable() {
        CallBacks.OnActivatedARena += ActivateARena;
        CallBacks.OnDeactivatedARena += DeactivateARena;
        CallBacks.OnActivated += Activate;
    }

    private void OnDisable() {
        CallBacks.OnActivatedARena -= ActivateARena;
        CallBacks.OnDeactivatedARena -= DeactivateARena;
        CallBacks.OnActivated -= Activate;
    }
}
