using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;

/// <summary>
/// UIUploaderARMsg uploading ARMsg
/// </summary>
public class UIUploaderARMsg : MonoBehaviour {
    [SerializeField]
    private UnityEvent OnARMsgUpdloadedEvent;

    private GalleryBtn _galleryBtn;

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
        MenuConstructor.OnActivated?.Invoke(true);
        OnARMsgUpdloadedEvent?.Invoke();

        //
        ShowPopupGoToGallery();
    }

    private void ShowPopupGoToGallery() {
        WarningConstructor.ActivateDoubleButton("Capture complete!",
            "Your hologram is now processing and we\n" +
            " will notify you when it's ready to preview",
            "go to your beem gallery", "close",
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
        if (_galleryBtn == null)
            _galleryBtn = FindObjectOfType<GalleryBtn>();
        _galleryBtn.OnClick();
    }
}
