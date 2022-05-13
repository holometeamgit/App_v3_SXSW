using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;
using UnityEngine.UI;

public class SubpnlCTALinkOptionsUpdatedWindow : MonoBehaviour, IBlindView {
    [SerializeField]
    private GameObject ProcessingGO;

    [SerializeField]
    private GameObject SuccessBlindOptions;

    [SerializeField]
    private Button _closeBtn;

    private float _closeTime = 3;
    private Coroutine _coroutine;

    public void Show(params object[] objects) {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    private void OnUpdatedCTA() {
        SuccessBlindOptions.SetActive(true);
        ProcessingGO.SetActive(false);
        _coroutine = StartCoroutine(HideInTime());
    }

    private void OnEnable() {
        CallBacks.onUpdatedCTA += OnUpdatedCTA;
    }

    private void OnDisable() {
        SuccessBlindOptions.SetActive(false);
        ProcessingGO.SetActive(true);
        CallBacks.onUpdatedCTA += OnUpdatedCTA;

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = null;
    }

    private IEnumerator HideInTime() {
        yield return new WaitForSeconds(_closeTime);
        _closeBtn.onClick?.Invoke();
    }
}
