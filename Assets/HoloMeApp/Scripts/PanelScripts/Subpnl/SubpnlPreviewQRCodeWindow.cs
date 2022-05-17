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
    private ShareLinkController _shareController = new ShareLinkController();

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

    public void SaveImageToGallary() {
        SaveImage((Texture2D)_imgQRCode.texture);
    }

    public void ShareImage() {
        _shareController.ShareTexture((Texture2D)_imgQRCode.texture);
    }

    private void SaveImage(Texture2D texture) {
        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write) != NativeGallery.Permission.Granted) {
            RequestPermission();
        }

        if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Write) != NativeGallery.Permission.Granted)
            return;

        NativeGallery.SaveImageToGallery(texture, "Beem", "QRCodeBeem"+ DateTime.Now + ".png");
    }

    private void RequestPermission() {
        NativeGallery.RequestPermission(NativeGallery.PermissionType.Write);
    }

    private void UpdateQRCode(Texture2D QRCOdeTexture) {
        _imgQRCode.texture = QRCOdeTexture;
    }

    private void OnDisable() {
        CallBacks.onQRCodeCreated -= UpdateQRCode;
        _imgQRCode.texture = _defaultTexture;
    }
}
