using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PnlOpenHomeMenu : MonoBehaviour
{
    public UnityEvent OnEnabled;
    public UnityEvent OnDisabled;
    public Button btnOnEnableInvoke;

    private void OnEnable()
    {
        OnEnabled?.Invoke();
        btnOnEnableInvoke?.onClick?.Invoke();
    }

    private void OnDisable() {
        //RoomTutorialConstructor.OnActivated?.Invoke(false);
        //StreamOverlayConstructor.onDeactivate?.Invoke();
        OnDisabled?.Invoke();
        StreamOverlayConstructor.onDeactivate?.Invoke();
    }
}
