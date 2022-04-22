using Beem.UI;
using UnityEngine;

/// <summary>
/// Delete ARMsg Btn
/// </summary>
public class DeleteARMsgBtn : MonoBehaviour, IARMsgDataView, IWebRequestHandlerView, IUserWebManager {
    [SerializeField]
    private ARMsgAPIScriptableObject _arMsgAPIScriptableObject;

    private WebRequestHandler _webRequestHandler;
    private UserWebManager _userWebManager;

    private DeleteARMsgController _deleteARMsgController;
    private GetAllARMessageController _galleryController;

    private ARMsgJSON.Data currentData;

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
        BlindOptionsConstructor.OnHide?.Invoke();
    }

    public void Init(ARMsgJSON.Data data) {
        currentData = data;
    }

    public void Init(WebRequestHandler webRequestHandler) {
        _webRequestHandler = webRequestHandler;
        _deleteARMsgController = new DeleteARMsgController(_arMsgAPIScriptableObject, _webRequestHandler);
        _galleryController = new GetAllARMessageController(_arMsgAPIScriptableObject, _webRequestHandler);
    }

    public void Init(UserWebManager userWebManager) {
        _userWebManager = userWebManager;
        gameObject.SetActive(currentData.user == _userWebManager.GetUsername());
    }
}
