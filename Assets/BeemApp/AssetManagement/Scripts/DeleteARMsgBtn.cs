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
    [SerializeField]
    private WebRequestHandler _webRequestHandler;

    private DeleteARMsgController _deleteARMsgController;

    private ARMsgJSON.Data currentData;

    private void Start() {
        _deleteARMsgController = new DeleteARMsgController(_arMsgAPIScriptableObject, _webRequestHandler);
    }

    /// <summary>
    /// On Click
    /// </summary>
    public void OnClick() {
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
