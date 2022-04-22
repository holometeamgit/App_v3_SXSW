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
    private WebRequestHandler _webRequestHandler;

    private List<IARMsgDataView> _arMsgDataViews;
    private List<IUserWebManager> _userWebManagerViews;
    private List<IWebRequestHandlerView> _webRequestHandlerViews;

    [Inject]
    public void Construct(HologramHandler hologramHandler, UserWebManager userWebManager, WebRequestHandler webRequestHandler) {
        _hologramHandler = hologramHandler;
        _userWebManager = userWebManager;
        _webRequestHandler = webRequestHandler;
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="streamData">Stream Json data</param>
    public void Init(ARMsgJSON.Data arMsgJSON) {

        gameObject.SetActive(true);
        _arMsgDataViews = GetComponentsInChildren<IARMsgDataView>().ToList();

        _arMsgDataViews.ForEach(x => x.Init(arMsgJSON));

        _userWebManagerViews = GetComponentsInChildren<IUserWebManager>().ToList();

        _userWebManagerViews.ForEach(x => x.Init(_userWebManager));

        _webRequestHandlerViews = GetComponentsInChildren<IWebRequestHandlerView>().ToList();

        _webRequestHandlerViews.ForEach(x => x.Init(_webRequestHandler));

        _hologramHandler.SetOnPlacementUIHelperFinished(OnPlacementCompleted);

        businessButtons.ForEach(x => x.SetActive(IsBusinessProfile));

        usualButtons.ForEach(x => x.SetActive(!IsBusinessProfile));
    }

    private bool IsBusinessProfile {
        get {
            return true;
        }
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

