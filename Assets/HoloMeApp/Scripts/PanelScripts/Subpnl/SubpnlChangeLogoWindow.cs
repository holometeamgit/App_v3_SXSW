using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beem.SSO;

public class SubpnlChangeLogoWindow : MonoBehaviour, IBlindView {
    [SerializeField]
    private Image _imgLogo;

    [SerializeField]
    private GameObject _imgGO;

    [SerializeField]
    private Button _btnRemoveLogo;

    [SerializeField]
    private string _nextWindowAssetId;

    public void Show(params object[] objects) {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    private void CheckHasLogo() {
        bool hasLogo = HasLogo();
        _imgGO.SetActive(hasLogo);
        _btnRemoveLogo.interactable = hasLogo;

        if (hasLogo)
            _imgLogo.sprite = GetLogo();
    }

    private bool HasLogo() {
        return CallBacks.hasLogoOnDevice(); 
    }

    private Sprite GetLogo() {
        return CallBacks.getLogoOnDevice();
    }

    private void MoveNextMenu() {
        BlindOptionsConstructor.Show(_nextWindowAssetId);
    }

    private void OnEnable() {
        CheckHasLogo();
        CallBacks.onLogoSelected += MoveNextMenu;
    }

    private void OnDisable() {
        _btnRemoveLogo.interactable = false;
        CallBacks.onLogoSelected -= MoveNextMenu;
    }
}
