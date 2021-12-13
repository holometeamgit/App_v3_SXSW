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

    private CancellationTokenSource _showCancellationTokenSource;
    private CancellationToken _showCancellationToken;

    private ARMsgJSON.Data currentData;
    private const int CHECK_COOLDOWN = 5000;

    private void Activate(bool status) {
        _pnlARMessagesSteps?.SetActive(status);
    }


    private void ActivateARena(ARMsgJSON.Data data) {
        OnReceivedRoomData(data, ActivateData);
    }

    private void ActivateData(ARMsgJSON.Data data) {
        currentData = data;

        _pnlViewingExperience.ActivateForARMessaging(data);

        _pnlARMessages.Init(data);

        _uiThumbnailsController.OnPlayFromUser?.Invoke(data.user);
    }

    private async Task WaitForCanShow() {
        while (!_popupShowChecker.CanShow()) {
            if (_showCancellationToken.IsCancellationRequested) {
                _showCancellationToken.ThrowIfCancellationRequested();
            }
            await Task.Delay(CHECK_COOLDOWN);
        }
    }

    private void OnReceivedRoomData(ARMsgJSON.Data data, Action<ARMsgJSON.Data> onSuccessTask) {
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

    private void DeactivateARena() {
        CallBacks.OnActivateGenericErrorDoubleButton?.Invoke("Before you go...",
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
