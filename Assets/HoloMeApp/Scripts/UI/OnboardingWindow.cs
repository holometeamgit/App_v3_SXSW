using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OnboardingWindow : MonoBehaviour {
    public Action onOnboardingClose;
    public Action onOnboardingOpen;

    [SerializeField]
    private Button _btnStartOnboarding;
    [SerializeField]
    private GameObject _btnNext;
    [SerializeField]
    private GameObject _btnBack;
    [SerializeField]
    private GameObject _btnStartHome;
    [SerializeField]
    private GameObject _skipBtn;

    [SerializeField]
    private GameObject _scrollController;

    [SerializeField]
    private ScrollSnap _scrollSnap;

    [SerializeField]
    private CanvasGroup _interactableCanvasGroup;

    private bool _initialized;

    public void Skip() {
        DisableInteration();
        _scrollSnap.ResetToLast();
    }

    public void Next() {
        DisableInteration();
        if (_scrollSnap.CurrentIndex == 0) {
            _btnStartOnboarding.interactable = false;
        }
        if (_scrollSnap.CurrentIndex == _scrollSnap.MaxIndex - 1) {
            _skipBtn.SetActive(false);
        }

        _scrollSnap.SnapToNext();
    }

    public void Prev() {
        DisableInteration();
        _scrollSnap.SnapToPrev();
    }

    private void OnRelease(int index) {
        UpdateInteraction(index);
    }

    private void UpdateInteraction(int index) {
        _skipBtn.SetActive(true);
        if (index == 0) {
            _btnStartOnboarding.interactable = true;
            _btnNext.SetActive(false);
            _btnBack.SetActive(false);
            _btnStartHome.SetActive(false);
            _scrollController.SetActive(false);
        } else if (index > 0 && index < (_scrollSnap.MaxIndex - 1)) {
            _btnNext.SetActive(true);
            _btnBack.SetActive(true);
            _btnStartHome.SetActive(false);
            _scrollController.SetActive(true);
        } else if (index == _scrollSnap.MaxIndex - 1) {
            _btnNext.SetActive(false);
            _btnBack.SetActive(true);
            _btnStartHome.SetActive(true);
            _scrollController.SetActive(true);
        } else if (index >= _scrollSnap.MaxIndex) {
            _btnNext.SetActive(false);
            _btnBack.SetActive(false);
            _btnStartHome.SetActive(false);
            _skipBtn.SetActive(false);
            _scrollController.SetActive(false);
        }
    }

    private void CloseOnboarding() {
        OnboardingConstructor.OnActivated?.Invoke(false);
        _scrollSnap.ResetToFirst();
    }

    private void DisableInteration() {
        _interactableCanvasGroup.interactable = false;
    }

    private void EnableInteraction() {
        _interactableCanvasGroup.interactable = true;
    }

    private void OnInitialized() {
        _initialized = true;
    }

    private void CheckForClose() {
        if(_scrollSnap.CurrentIndex == _scrollSnap.MaxIndex)
            CloseOnboarding();
    }

    private void OnEnable() {
        _scrollSnap.onStartDrag += DisableInteration;
        _scrollSnap.onLerpComplete.AddListener(EnableInteraction);
        _scrollSnap.onLerpComplete.AddListener(CheckForClose);
        _scrollSnap.onRelease += OnRelease;
        _scrollSnap.onInitialized += OnInitialized;
        if (_initialized) {
            _scrollSnap.ResetToFirst();
        }
        onOnboardingOpen?.Invoke();
    }

    private void OnDisable() {
        _scrollSnap.onStartDrag -= DisableInteration;
        _scrollSnap.onLerpComplete.RemoveListener(EnableInteraction);
        _scrollSnap.onLerpComplete.RemoveListener(CheckForClose);
        _scrollSnap.onRelease -= OnRelease;
        _scrollSnap.onInitialized -= OnInitialized;
        onOnboardingClose?.Invoke();
    }
}
