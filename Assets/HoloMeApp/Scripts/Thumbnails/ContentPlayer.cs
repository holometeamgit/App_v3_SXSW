using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.Permissions;


/// <summary>
/// Content Player
/// </summary>
public class ContentPlayer : MonoBehaviour {

    [SerializeField]
    private WebRequestHandler _webRequestHandler;
    [SerializeField]
    private PurchaseManager _purchaseManager;
    [SerializeField]
    private PermissionController _permissionController;

    public static Action<string> OnPlayFromUser;

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
    private void PlayRoom(RoomJsonData data) { //TODO split it to other class
        _permissionController.CheckCameraMicAccess(() => {
            MenuConstructor.OnActivated?.Invoke(false);
            HomeScreenConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onActivatedAsViewer?.Invoke(data.agora_channel, data.id, true);
            OnPlayFromUser?.Invoke(data.user);
        });

    }

    /// <summary>
    /// Play Stadium
    /// </summary>
    /// <param name="roomJsonData"></param>
    private void PlayStadium(StreamJsonData.Data data) { //TODO split it to other class
        if (data.agora_channel == "0" || string.IsNullOrWhiteSpace(data.agora_channel)) {
            return;
        }

        _permissionController.CheckCameraMicAccess(() => {
            StreamCallBacks.onCloseStreamPopUp?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            HomeScreenConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            StreamOverlayConstructor.onActivatedAsViewer?.Invoke(data.agora_channel, data.id.ToString(), false);
            OnPlayFromUser?.Invoke(data.user);
        });
    }

    /// <summary>
    /// Play Prerecorded
    /// </summary>
    /// <param name="roomJsonData"></param>
    private void PlayPrerecorded(StreamJsonData.Data data) { //TODO split it to other class
        _permissionController.CheckCameraMicAccess(() => {
            StreamCallBacks.onCloseStreamPopUp?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            HomeScreenConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForPreRecorded?.Invoke(data, false);
            PrerecordedVideoConstructor.OnActivated?.Invoke(data);
            OnPlayFromUser?.Invoke(data.user);
        });
    }

    /// <summary>
    /// Play Teaser
    /// </summary>
    /// <param name="data"></param>
    private void PlayTeaser(StreamJsonData.Data data) {
        _permissionController.CheckCameraMicAccess(() => {
            StreamCallBacks.onCloseStreamPopUp?.Invoke();
            MenuConstructor.OnActivated?.Invoke(false);
            HomeScreenConstructor.OnActivated?.Invoke(false);
            SettingsConstructor.OnActivated?.Invoke(false);
            ARenaConstructor.onActivateForPreRecorded?.Invoke(data, data.HasTeaser);
            PrerecordedVideoConstructor.OnActivated?.Invoke(data);
            OnPlayFromUser?.Invoke(data.user);
            _purchaseManager.SetPurchaseStreamData(data);
        });
    }

    private void Awake() {
        StreamCallBacks.onPlayRoom += PlayRoom;
    }

    private void OnDestroy() {
        StreamCallBacks.onPlayRoom -= PlayRoom;
    }
}
