using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Beem.SSO;
using System;

/// <summary>
/// Subpnl Preview QRCode Window
/// </summary>
public class SubpnlPreviewQRCodeWindow : MonoBehaviour, IBlindView {

    [SerializeField]
    private RawImage _imgQRCode;

    [SerializeField]
    private Texture2D _defaultTexture;

    [SerializeField]
    private string _nextPnlName = "SubpnlQRCodeSavedWindow";
    private ShareLinkController _shareController = new ShareLinkController();

    private const string SUBPNL_PREVIEW_QRCODE_OPTION = "SubpnlPreviewQRCodeWindow";

    public void Show(params object[] objects) {
        CallBacks.onQRCodeCreated += UpdateQRCode;
        gameObject.SetActive(true);
        if (objects.Length > 0) {
            ARMsgJSON.Data data = objects[0] as ARMsgJSON.Data;
            HelperFunctions.DevLog(data.share_link);
            CallBacks.onGetQRCode?.Invoke(data.share_link);
        }
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Save Image To Gallary
    /// </summary>
    public void SaveImageToGallary() {
        SaveImage((Texture2D)_imgQRCode.texture);
    }

    /// <summary>
    /// Share Image
    /// </summary>
    public void ShareImage() {
        _shareController.ShareTexture((Texture2D)_imgQRCode.texture);
    }

    private void SaveImage(Texture2D texture) {
        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write) != NativeGallery.Permission.Granted) {
            RequestPermission();
        }

        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write) != NativeGallery.Permission.Granted)
            return;

        BlindOptionsConstructor.Show(_nextPnlName);
        NativeGallery.SaveImageToGallery(texture, "Beem", "QRCodeBeem.png", OnSaveImageToGalleryCallBack);
    }

    private void RequestPermission() {
        NativeGallery.RequestPermission(NativeGallery.PermissionType.Write);
    }

    private void UpdateQRCode(Texture2D QRCOdeTexture) {
        _imgQRCode.texture = QRCOdeTexture;
    }

    private void OnSaveImageToGalleryCallBack(bool success, string path) {
        if (success) {
            CallBacks.onQRCodeSaved?.Invoke();
        } else {
            if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write) != NativeGallery.Permission.Granted) {
                WarningConstructor.ActivateDoubleButton(header: "Please allow access", message: "Beem needs to access your photo\nlibrary to perform this action",
                    buttonOneText: "go to settings", buttonTwoText: "Cancel",
                    onButtonOnePress: () => NativeGallery.OpenSettings(),
                    onButtonTwoPress: () => BlindOptionsConstructor.Show(SUBPNL_PREVIEW_QRCODE_OPTION), isWarning: true);
            } else {
                WarningConstructor.ActivateSingleButton(header: "An error occurred while saving", message: "Please try share",
                    buttonText: "Confirm",
                    onBackPress: () => BlindOptionsConstructor.Show(SUBPNL_PREVIEW_QRCODE_OPTION), isWarning: true);
            }
        }
    }

    private void OnDisable() {
        CallBacks.onQRCodeCreated -= UpdateQRCode;
        _imgQRCode.texture = _defaultTexture;
    }
}
