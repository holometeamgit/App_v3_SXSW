using UnityEngine;
using UnityEngine.UI;
using System;
using Zenject;

/// <summary>
/// OnboardingWindow for new users
/// </summary>
public class OnboardingWindow : MonoBehaviour {

    private BusinessProfileManager _businessProfileManager;
    [SerializeField]
    private GameObject[] _businessTutorialsElements;
    [SerializeField]
    private GameObject _scrollMarker;

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

    [Inject]
    public void Constructor(BusinessProfileManager businessProfileManager) {
        _businessProfileManager = businessProfileManager;
    }

    private void Awake() {
        foreach(GameObject businessTutorialElement in _businessTutorialsElements) {
            businessTutorialElement.gameObject.SetActive(_businessProfileManager.IsBusinessProfile());
        }
        _scrollMarker.gameObject.SetActive(_businessProfileManager.IsBusinessProfile());
    }

    /// <summary>
    /// Skip onboarding
    /// </summary>
    public void Skip() {
        Close();
    }

    /// <summary>
    /// next onboarding screen
    /// </summary>
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

    /// <summary>
    /// Prev onboarding screen
    /// </summary>
    public void Prev() {
        DisableInteration();
        _scrollSnap.SnapToPrev();
    }

    /// <summary>
    /// Close onboarding
    /// </summary>
    public void Close() {
        _btnNext.SetActive(false);
        _btnBack.SetActive(false);
        _btnStartHome.SetActive(false);
        _skipBtn.SetActive(false);
        _scrollController.SetActive(false);
        CloseOnboarding();
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
        } else if (index > 0 && index <= (_scrollSnap.MaxIndex - 1)) {
            _btnNext.SetActive(true);
            _btnBack.SetActive(true);
            _btnStartHome.SetActive(false);
            _scrollController.SetActive(true);
        } else if (index == _scrollSnap.MaxIndex) {
            _btnNext.SetActive(false);
            _btnBack.SetActive(true);
            _btnStartHome.SetActive(true);
            _scrollController.SetActive(true);
        } 
    }

    private void CloseOnboarding() {
        MenuConstructor.OnActivated?.Invoke(true);
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

    private void OnEnable() {
        _scrollSnap.onStartDrag += DisableInteration;
        _scrollSnap.onLerpComplete.AddListener(EnableInteraction);
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
        _scrollSnap.onRelease -= OnRelease;
        _scrollSnap.onInitialized -= OnInitialized;
        onOnboardingClose?.Invoke();
    }
}
