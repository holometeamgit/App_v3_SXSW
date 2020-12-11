using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BtnController : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] float delayAfterClick = 1;

    public UnityEvent OnPress;

    public void BtnPress() {
        StartCoroutine(InteractableDelay());
        OnPress.Invoke();
    }

    private void OnEnable() {
        btn.interactable = true;
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
    private IEnumerator InteractableDelay() {
        btn.interactable = false;
        yield return new WaitForSeconds(delayAfterClick);
        btn.interactable = true;
    }
}
