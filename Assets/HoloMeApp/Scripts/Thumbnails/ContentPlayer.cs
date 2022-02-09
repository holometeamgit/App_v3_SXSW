using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.Permissions;
using Beem.ARMsg;
using Zenject;
using WindowManager.Extenject;

/// <summary>
/// Content Player
/// </summary>
public class ContentPlayer {

    private UserWebManager _userWebManager;
    private PurchaseManager _purchaseManager;
    private PermissionController _permissionController = new PermissionController();

    public static string UserName;

    public ContentPlayer(UserWebManager userWebManager, PurchaseManager purchaseManager) {
        _userWebManager = userWebManager;
        _purchaseManager = purchaseManager;
    }

    public ContentPlayer(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
    }

    /// <summary>
    /// Buy Stadium/Prerecorded
    /// </summary>
    /// <param name="data"></param>
    public void Buy(StreamJsonData.Data data) {
        _purchaseManager.SetPurchaseStreamData(data);
        _purchaseManager.Purchase();
    }

    /// <summary>
    /// Share Stream from HomePage
    /// </summary>
    /// <param name="data"></param>
    public void Share(StreamJsonData.Data data) {
        StreamCallBacks.onShareStreamLinkByData?.Invoke(data);
    }

    /// <summary>
    /// Play data
    /// </summary>
    /// <param name="data"></param>
    public void Play(StreamJsonData.Data data) {
        if (data.is_bought && data.IsStarted) {
            if (data.HasStreamUrl) {
                PlayPrerecorded(data);
            } else if (data.HasAgoraChannel) {
                PlayStadium(data);
            }
        } else if (data.HasTeaser) {
            PlayTeaser(data);
        }
    }

    /// <summary>
    /// Play Room
    /// </summary>
    /// <param name="roomJsonData"></param>
    public void PlayRoom(RoomJsonData data) { //TODO split it to other class

        if (data.user == _userWebManager.GetUsername()) {

            GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Ok", null);
            GeneralPopUpData popUpData = new GeneralPopUpData("Viewing as stream host", "Please connect to the stream using a different account", closeButton);
            WarningConstructor.OnShow?.Invoke(popUpData);

            return;
        }

        _permissionController.CheckCameraMicAccess(() => {
            HomeConstructor.OnActivated?.Invoke(false);
            BottomMenuConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onActivatedAsViewer?.Invoke(data.agora_channel, data.id, true);
            UserName = data.user;
        });

    }

    /// <summary>
    /// Play ARMessage
    /// </summary>
    /// <param name="roomJsonData"></param>
    public void PlayARMessage(ARMsgJSON.Data data) { //TODO split it to other class
        _permissionController.CheckCameraMicAccess(() => {
            HomeConstructor.OnActivated?.Invoke(false);
            BottomMenuConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            ARMsgRecordConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForARMessaging?.Invoke(data);
            ARMsgARenaConstructor.OnShow?.Invoke(data);
            UserName = data.user;
        });

    }


    /// <summary>
    /// Play Stadium
    /// </summary>
    /// <param name="roomJsonData"></param>
    private void PlayStadium(StreamJsonData.Data data) { //TODO split it to other class
        if (data.user == _userWebManager.GetUsername()) {

            GeneralPopUpData.ButtonData closeButton = new GeneralPopUpData.ButtonData("Ok", null);
            GeneralPopUpData popUpData = new GeneralPopUpData("Viewing as stream host", "Please connect to the stream using a different account", closeButton);
            WarningConstructor.OnShow?.Invoke(popUpData);

            return;
        }

        if (data.agora_channel == "0" || string.IsNullOrWhiteSpace(data.agora_channel)) {
            return;
        }

        _permissionController.CheckCameraMicAccess(() => {
            DeepLinkStreamConstructor.OnHide?.Invoke();
            HomeConstructor.OnActivated?.Invoke(false);
            BottomMenuConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onActivatedAsViewer?.Invoke(data.agora_channel, data.id.ToString(), false);
            UserName = data.user;
        });
    }

    /// <summary>
    /// Play Prerecorded
    /// </summary>
    /// <param name="roomJsonData"></param>
    private void PlayPrerecorded(StreamJsonData.Data data) { //TODO split it to other class
        _permissionController.CheckCameraMicAccess(() => {
            DeepLinkStreamConstructor.OnHide?.Invoke();
            HomeConstructor.OnActivated?.Invoke(false);
            BottomMenuConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForPreRecorded?.Invoke(data, false);
            PrerecordedVideoConstructor.OnActivated?.Invoke(data);
            UserName = data.user;
        });
    }

    /// <summary>
    /// Play Teaser
    /// </summary>
    /// <param name="data"></param>
    private void PlayTeaser(StreamJsonData.Data data) {
        _permissionController.CheckCameraMicAccess(() => {
            DeepLinkStreamConstructor.OnHide?.Invoke();
            HomeConstructor.OnActivated?.Invoke(false);
            BottomMenuConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForPreRecorded?.Invoke(data, data.HasTeaser);
            PrerecordedVideoConstructor.OnActivated?.Invoke(data);
            UserName = data.user;
            _purchaseManager.SetPurchaseStreamData(data);
        });
    }
}
