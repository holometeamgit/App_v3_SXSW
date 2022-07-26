using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Beem.UI;
using Beem.Permissions;
using Zenject;

/// <summary>
/// Window for ARMsg
/// </summary>
public class ARMsgWindow : MonoBehaviour {

    [SerializeField]
    private List<GameObject> businessButtons;

    [SerializeField]
    private List<GameObject> usualButtons;

    private HologramHandler _hologramHandler;
    private UserWebManager _userWebManager;
    private BusinessProfileManager _businessProfileManager;
    private WebRequestHandler _webRequestHandler;

    private List<IARMsgDataView> _arMsgDataViews;
    private List<IUserWebManagerView> _userWebManagerViews;
    private List<IBusinessProfileManagerView> _businessProfileManagerViews;
    private List<IWebRequestHandlerView> _webRequestHandlerViews;

    [Inject]
    public void Construct(HologramHandler hologramHandler, UserWebManager userWebManager, BusinessProfileManager businessProfileManager, WebRequestHandler webRequestHandler) {
        _hologramHandler = hologramHandler;
        _userWebManager = userWebManager;
        _businessProfileManager = businessProfileManager;
        _webRequestHandler = webRequestHandler;
    }

    private void SuccessedBusinessProfile(BusinessProfileJsonData businessProfileData) {
        ShowBusiness(businessProfileData != null);
    }

    private void FailedBusinessProfile(WebRequestError error) {
        ShowBusiness(false);
    }


    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamData">Stream Json data</param>
    public void Init(ARMsgJSON.Data arMsgJSON) {

        gameObject.SetActive(true);
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(arMsgJSON));

        _userWebManagerViews = GetComponentsInChildren<IUserWebManagerView>().ToList();

        _userWebManagerViews.ForEach(x => x.Init(_userWebManager));

        _businessProfileManagerViews = GetComponentsInChildren<IBusinessProfileManagerView>().ToList();

        _businessProfileManagerViews.ForEach(x => x.Init(_businessProfileManager));

        _webRequestHandlerViews = GetComponentsInChildren<IWebRequestHandlerView>().ToList();

        _webRequestHandlerViews.ForEach(x => x.Init(_webRequestHandler));

        _hologramHandler.SetOnPlacementUIHelperFinished(OnPlacementCompleted);

        ShowBusiness(false);

        if (arMsgJSON.ext_content_data != null && arMsgJSON.ext_content_data.Count > 0) {
            _businessProfileManager.GetMyData(SuccessedBusinessProfile, FailedBusinessProfile);
        }
    }

    private void ShowBusiness(bool status) {
        businessButtons.ForEach(x => x.SetActive(status));

        usualButtons.ForEach(x => x.SetActive(!status));
    }

    private void OnPlacementCompleted() {
        RecordARConstructor.OnActivated?.Invoke(true);
    }

    /// <summary>
    /// Deactivate
    /// </summary>
    public void Deactivate() {
        RecordARConstructor.OnActivated?.Invoke(false);
        gameObject.SetActive(false);
    }
}

