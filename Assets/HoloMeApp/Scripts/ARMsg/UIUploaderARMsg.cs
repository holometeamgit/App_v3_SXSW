using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;
using System.Threading.Tasks;

/// <summary>
/// UIUploaderARMsg uploading ARMsg
/// </summary>
public class UIUploaderARMsg : MonoBehaviour {
    [SerializeField]
    private UnityEvent OnARMsgUpdloadedEvent;
    [SerializeField]
    private GalleryBtn _galleryBtn;

    private Coroutine _coroutine;

    private void Awake() {
        CallBacks.OnARMsgUpdloaded += OnARMsgUpdloaded;
    }

    private void OnEnable() {
        CallBacks.OnUpdloadingUIOpened?.Invoke();
    }

    private void OnDestroy() {
        CallBacks.OnARMsgUpdloaded -= OnARMsgUpdloaded;
    }

    private void OnARMsgUpdloaded() {
        WarningConstructor.Deactivate();

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(OnARMsgUpdloadedDoing());
    }

    private IEnumerator OnARMsgUpdloadedDoing() {
        yield return null;
        MenuConstructor.OnActivated?.Invoke(true);
        OnARMsgUpdloadedEvent?.Invoke();

        
        ShowPopupGoToGallery();

        _coroutine = null;
    }

    private void ShowPopupGoToGallery() {
        WarningConstructor.ActivateDoubleButton("Capture\ncomplete!",
            "Your hologram is now processing and we will\n" +
            "notify you when it’s ready to preview",
            "go to your gallery", "close",
             () => {
                 OpenGallery();
             },
            () => {
            }, false);
    }

    private void ShowPopupTurnOnNotification() {
        WarningConstructor.ActivateDoubleButton("Capture complete!",
            "Get notified when your Beem is ready",
            "close", "Turn on notifications",
            () => {

            }, null, false);
    }

    private void OpenGallery() {
        _galleryBtn.OnClick();
    }
}
