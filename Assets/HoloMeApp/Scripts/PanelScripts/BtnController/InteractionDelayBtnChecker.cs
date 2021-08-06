using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDelayBtnChecker : BtnInteractionRequirementChecker
{
    [SerializeField]
    float _delayAfterClick = 1;

    protected override bool CheckRequirements() {
        return _canInteract;
    }

    protected override void OnEnable() {
        base.OnEnable();
        _btnController.OnPress.AddListener(StartDelayInteration);
        _onRequirementsUpdated.Invoke();
    }

    protected override void OnDisable() {
        base.OnDisable();
        StopAllCoroutines();
        _btnController.OnPress.RemoveListener(StartDelayInteration);
        _canInteract = true;
    }

    private void StartDelayInteration() {
        if (isActiveAndEnabled)
            StartCoroutine(InteractableDelay());
    }

    private IEnumerator InteractableDelay() {
        _canInteract = false;
        _onRequirementsUpdated?.Invoke();
        yield return new WaitForSeconds(_delayAfterClick);
        _canInteract = true;
        _onRequirementsUpdated?.Invoke();
    }
}
