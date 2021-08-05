using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDelayBtnChecker : BtnInteractionRequirementChecker
{
    [SerializeField]
    float _delayAfterClick = 1;

    protected override void Awake() {
        base.Awake();
    }

    private void StartDelayInteration() {
        StartCoroutine(InteractableDelay());
    }

    private IEnumerator InteractableDelay() {
        _canInteract = false;
        yield return new WaitForSeconds(_delayAfterClick);
        _canInteract = true;
    }

    private void OnEnable() {
        _btnController.OnPress.AddListener(StartDelayInteration);
    }

    private void OnDisable() {
        StopAllCoroutines();
        _btnController.OnPress.RemoveListener(StartDelayInteration);
    }
}
