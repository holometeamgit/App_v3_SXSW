using UnityEngine;
using UnityEngine.Events;

public class PnlFetchingData : MonoBehaviour
{
    [SerializeField]
    ServerDataHandler serverDataHandler;

    [SerializeField]
    AnimatedTransition animatedTransition;

    [SerializeField]
    VersionMismatchCheck versionMismatchCheck;

    [SerializeField]
    UnityEvent OnDataReceviedEvent;

    bool waitingForInternet;
    private bool initialStartup = true;

    public void Activate(UnityAction onComplete)
    {
        OnDataReceviedEvent.RemoveAllListeners();
        OnDataReceviedEvent.AddListener(onComplete);
        gameObject.SetActive(true);
    }

    void OnEnable()
    {
        if (initialStartup)
            return;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            waitingForInternet = true;
        }
        else
        {
            waitingForInternet = false;
            TryGetData();
        }
    }

    void Update()
    {
        if (waitingForInternet)
        {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                TryGetData();
                waitingForInternet = false;
            }
        }
    }

    void TryGetData()
    {
        serverDataHandler.OnDictionaryPopulated -= OnDataReceived;
        serverDataHandler.OnDictionaryPopulated += OnDataReceived;
        serverDataHandler.PopulateVideoDictionary();
    }

    void OnDataReceived()
    {
        CheckVersion();
    }

    private void CheckVersion()
    {
        versionMismatchCheck.OnVersionPassed -= OnVersionPassed;
        versionMismatchCheck.OnVersionPassed += OnVersionPassed;
        versionMismatchCheck.CompareVersion();
        animatedTransition.DoMenuTransition(false);
    }

    void OnVersionPassed()
    {
        OnDataReceviedEvent?.Invoke();
    }

    private void OnDisable()
    {
        initialStartup = false;
    }
}
