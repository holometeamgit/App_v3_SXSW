using Beem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
/// <summary>
/// Delete ARMsg Btn
/// </summary>
public class DeleteARMsgBtn : MonoBehaviour, IARMsgDataView {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    private WebRequestHandler _webRequestHandler;

    private UserWebManager _userWebManager;

    private UserWebManager GetUserWebManager {
        get {

            if (_userWebManager == null) {
                _userWebManager = FindObjectOfType<UserWebManager>();
            }

            return _userWebManager;
        }
    }

    private DeleteARMsgController _deleteARMsgController;
    private GalleryController _galleryController;

    private ARMsgJSON.Data currentData;

    [Inject]
    public void Construct(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
    }

    private void Start() {
        _deleteARMsgController = new DeleteARMsgController(_arMsgAPIScriptableObject, _webRequestHandler);
        _galleryController = new GalleryController(_arMsgAPIScriptableObject, _webRequestHandler);
    }

    /// <summary>
    /// On Click
    /// </summary>
    public void OnClick() {
        WarningConstructor.ActivateDoubleButton("Delete this Beem?", "Deleting a Beem is forever, are you sure?", "Delete", "Cancel", () => Delete());
    }

    private void Delete() {
        _deleteARMsgController.DeleteARMessages(currentData.id, OnSuccess);
    }

    private void OnSuccess() {
        _galleryController.GetAllArMessages(onSuccess: Show);
    }

    private void Show(ARMsgJSON data) {
        ARMsgARenaConstructor.OnDeactivatedARena?.Invoke();
        ARenaConstructor.onDeactivate?.Invoke();
        GalleryConstructor.OnShow?.Invoke(data);
    }

    public void Init(ARMsgJSON.Data data) {
        currentData = data;
        if (currentData.user != GetUserWebManager.GetUsername()) {
            gameObject.SetActive(false);
        }
    }
}
