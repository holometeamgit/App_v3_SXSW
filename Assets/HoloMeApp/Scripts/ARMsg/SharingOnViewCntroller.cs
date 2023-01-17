using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SharingOnViewCntroller : MonoBehaviour, IInitializable, IDisposable {
    [SerializeField]
    private PnlViewingExperience _pnlViewingExperience;
    [SerializeField]
    private GameObject _sharePopup;

    private float _shareTimeDelay = 5;
    private Coroutine _preparePopupCorotine;

    private SignalBus _signalBus;

    private bool _needShow;
    private string _shareLink = "";

    private ShareLinkController _shareController = new ShareLinkController();

    [Inject]
    public void Construct(SignalBus signalBus) {
        _signalBus = signalBus;
    }

    public void Initialize() {
        _signalBus.Subscribe<PnlViewingExperience.BeemVideoPlayStartedSignal>(onBeemVideoPlayStartedSignal);
        _signalBus.Subscribe<PnlViewingExperience.BeemVideoPlayStoped>(onBeemVideoPlayStoped);
        _signalBus.Subscribe<PnlViewingExperience.ARBeemShareLinkReceived>(onARBeemShareLinkReceived);
    }

    public void Dispose() {
        _signalBus.Unsubscribe<PnlViewingExperience.BeemVideoPlayStartedSignal>(onBeemVideoPlayStartedSignal);
        _signalBus.Unsubscribe<PnlViewingExperience.BeemVideoPlayStoped>(onBeemVideoPlayStoped);
        _signalBus.Unsubscribe<PnlViewingExperience.ARBeemShareLinkReceived>(onARBeemShareLinkReceived);
    }

    public void ShareLink() {
        _shareController.ShareLink(_shareLink);
    }

    private void OnEnable() {
        if (!_needShow)
            return;

        StopPreparing();
        _preparePopupCorotine = StartCoroutine(PreparePopup());
    }

    private void onBeemVideoPlayStartedSignal(PnlViewingExperience.BeemVideoPlayStartedSignal beemVideoPlayStartedSignal) {
        _needShow = true;
        StopPreparing();
        _preparePopupCorotine = StartCoroutine(PreparePopup());
    }

    private void onBeemVideoPlayStoped(PnlViewingExperience.BeemVideoPlayStoped beemVideoPlayStoped) {
        _needShow = false;
    }

    private void onARBeemShareLinkReceived(PnlViewingExperience.ARBeemShareLinkReceived arBeemShareLinkReceived) {
        _shareLink = arBeemShareLinkReceived.ShareLink;
    }

    private void StopPreparing() {
        if (_preparePopupCorotine != null)
            StopCoroutine(_preparePopupCorotine);
    }

    private void OnDisable() {
        StopPreparing();
        _shareLink = "";
        _sharePopup.SetActive(false);
    }

    private IEnumerator PreparePopup() {
        yield return new WaitForSeconds(_shareTimeDelay);
        _sharePopup.SetActive(true);
        _needShow = false;
    }
}
