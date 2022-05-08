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

    private void OnEnable() {
        CheckHasLogo();
    }

    private void OnDisable() {
        _btnRemoveLogo.interactable = false;
    }
}
