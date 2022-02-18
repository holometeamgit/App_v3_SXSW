using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Delete ARMsg Btn
/// </summary>
public class DeleteARMsgBtn : MonoBehaviour, IARMsgDataView {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    private WebRequestHandler _webRequestHandler;

    private WebRequestHandler GetWebRequestHandler {
        get {

            if (_webRequestHandler = null) {
                _webRequestHandler = FindObjectOfType<WebRequestHandler>();
            }

            return _webRequestHandler;
        }
    }

    private DeleteARMsgController _deleteARMsgController;

    private ARMsgJSON.Data currentData;

    private void Start() {
        _deleteARMsgController = new DeleteARMsgController(_arMsgAPIScriptableObject, GetWebRequestHandler);
    }

    /// <summary>
    /// On Click
    /// </summary>
    public void OnClick() {
        WarningConstructor.ActivateDoubleButton("Delete this beem?", "Deleting a beem is forever, are you sure?", "Delete", "Cancel", () => Delete());
    }

    private void Delete() {
        _deleteARMsgController.DeleteARMessages(currentData.id, OnSuccess);
    }

    private void OnSuccess() {
        ARMsgARenaConstructor.OnDeactivatedARena?.Invoke();
        ARenaConstructor.onDeactivate?.Invoke();
        MenuConstructor.OnActivated?.Invoke(true);
        HomeScreenConstructor.OnActivated?.Invoke(true);
    }

    public void Init(ARMsgJSON.Data data) {
        currentData = data;
    }
}
