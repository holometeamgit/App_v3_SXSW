using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.SSO;

public class SubpnlPreviewLogoWindow : MonoBehaviour, IBlindView {
    [SerializeField]
    private Image _imgLogo;

    [SerializeField]
    private string _nextWindowAssetId;

    [SerializeField]
    private Button _updateLogo;

    public void Show(params object[] objects) {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    private void ShowSelectedLogo() {
        _imgLogo.sprite = GetSelectedLogo();
    }

    private Sprite GetSelectedLogo() {
        return CallBacks.getSelectedLogoOnDevice();
    }

    private void MoveNextMenu() {
        BlindOptionsConstructor.Show(_nextWindowAssetId);
    }

    private void OnEnable() {
        ShowSelectedLogo();
        CallBacks.onUploadSelectedLogo += MoveNextMenu;
    }

    private void OnDisable() {
        _updateLogo.interactable = true;
        CallBacks.onUploadSelectedLogo -= MoveNextMenu;
    }
}
