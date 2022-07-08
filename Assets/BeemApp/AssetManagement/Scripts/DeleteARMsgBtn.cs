using Beem.UI;
using UnityEngine;
using Zenject;

/// <summary>
/// Delete ARMsg Btn
/// </summary>
public class DeleteARMsgBtn : MonoBehaviour, IARMsgDataView, IUserWebManagerView {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    private UserWebManager _userWebManager;

    private ARMsgJSON.Data currentData;

    SignalBus _signalBus;

    /// <summary>
    /// On Click
    /// </summary>
    public void OnClick() {
        WarningConstructor.ActivateDoubleButton("Delete this Beem?", "Deleting a Beem is forever, are you sure?", "Delete", "Cancel", () => Delete());
    }

    public void Init(ARMsgJSON.Data data) {
        currentData = data;
    }

    public void Init(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
        gameObject.SetActive(currentData.user == _userWebManager.GetUsername());
    }

    private void Delete() {
        _signalBus.Fire(new DeleteARMsgSignal() { idARMsg = currentData.id } );
    }

    private void Show(GetAllArMessagesSuccesSignal signal) {
        ARMsgARenaConstructor.OnDeactivatedARena?.Invoke();
        ARenaConstructor.onDeactivate?.Invoke();
        GalleryConstructor.OnShow?.Invoke(signal.arMsgJSON);
        BlindOptionsConstructor.Hide();
    }

    private void OnEnable() {
        if(_signalBus == null) {
            _signalBus = FindObjectOfType<SignalBusMonoBehaviour>().SignalBus;
        }
        _signalBus.Subscribe<GetAllArMessagesSuccesSignal>(Show);
    }

    private void OnDisable() {
        _signalBus.Unsubscribe<GetAllArMessagesSuccesSignal>(Show);
    }

}
