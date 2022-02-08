using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// Constructor for opening deep Link for ARMessage
/// </summary>
public class ARMsgDeeplinkConstructor : MonoBehaviour {

    [SerializeField]
    private DeepLinkChecker _popupShowChecker;

    public static Action<ARMsgJSON.Data> OnActivated = delegate { };

    private ContentPlayer _contentPlayer;

    [Inject]
    public void Construct(UserWebManager userWebManager) {
        _contentPlayer = new ContentPlayer(userWebManager);
    }

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
        _popupShowChecker.OnReceivedData(data, onSuccessTask);
    }

    private void ActivateData(ARMsgJSON.Data data) {
        _contentPlayer.PlayARMessage(data);
    }

}
