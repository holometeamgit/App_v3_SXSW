using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using UnityEngine.UI;

/// <summary>
/// Subpnl QRCode notification foe user that QRCode has Saved 
/// </summary>
public class SubpnlQRCodeSavedWindow : MonoBehaviour, IBlindView {
    [SerializeField]
    private GameObject ProcessingGO;

    [SerializeField]
    private GameObject SuccessBlindOptions;

    [SerializeField]
    private Button _closeBtn;

    private float _closeTime = 3;
    private Coroutine _coroutine;

    public void Show(params object[] objects) {
        CallBacks.onQRCodeSaved += OnQRCodeSaved;
        gameObject.SetActive(true);
#if UNITY_EDITOR
        OnQRCodeSaved();
#endif
    }

    public void Hide() {
        CallBacks.onQRCodeSaved -= OnQRCodeSaved;
        gameObject.SetActive(false);
    }

    private void OnQRCodeSaved() {
        SuccessBlindOptions.SetActive(true);
        ProcessingGO.SetActive(false);
        _coroutine = StartCoroutine(HideInTime());
    }

    private void OnEnable() {

    }

    private void OnDisable() {
        SuccessBlindOptions.SetActive(false);
        ProcessingGO.SetActive(true);

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = null;
    }

    private IEnumerator HideInTime() {
        yield return new WaitForSeconds(_closeTime);
        _closeBtn.onClick?.Invoke();
    }
}
