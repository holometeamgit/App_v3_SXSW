using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class starts the delay timer and
/// returns at this moment that the button should not be interactive
/// </summary>
public class InteractionDelayBtnChecker : BtnInteractionRequirementChecker {
    [SerializeField]
    float _delayAfterClick = 1;

    protected override bool CheckRequirements() {
        return _canInteract;
    }

    protected override void OnEnable() {
        base.OnEnable();
        _btnController.OnPress.AddListener(StartDelayInteration);
        _onAvailableUpdated?.Invoke(_canInteract);
    }

    protected override void OnDisable() {
        StopAllCoroutines();
        base.OnDisable();
        _btnController.OnPress.RemoveListener(StartDelayInteration);
        _canInteract = true;
        _onAvailableUpdated?.Invoke(_canInteract);
    }

    private void StartDelayInteration() {
        if (isActiveAndEnabled)
            StartCoroutine(InteractableDelay());
    }

    private IEnumerator InteractableDelay() {
        _canInteract = false;
        _onAvailableUpdated?.Invoke(_canInteract);
        yield return new WaitForSeconds(_delayAfterClick);
        _canInteract = true;
        _onAvailableUpdated?.Invoke(_canInteract);
    }
}
