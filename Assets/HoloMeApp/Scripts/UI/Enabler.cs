using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enabler : MonoBehaviour
{
    public List<GameObject> ObjectListActiveOnEnable;
    public List<GameObject> ObjectListDectiveOnEnable;

    public UnityEvent OnEnableEvent;
    [Space]
    public List<GameObject> ObjectListActiveOnDisable;
    public List<GameObject> ObjectListDectiveOnDisable;

    public UnityEvent OnDisableEvent;

    [Space]
    public float timeDelay;
    public UnityEvent OnDelayEvent;

    [Space]
    public UnityEvent onNotFocusEvent;
    public UnityEvent onFocusEvent;

    private void OnEnable() {
        foreach (var element in ObjectListActiveOnEnable)
            element.SetActive(true);

        foreach (var element in ObjectListDectiveOnEnable)
            element.SetActive(false);

        OnEnableEvent.Invoke();

        if (isActiveAndEnabled) {
            StartCoroutine(InvokeEvent());
        } else if(timeDelay == 0) {
            OnDelayEvent.Invoke();
        }

    }

    private void OnDisable() {
        foreach (var element in ObjectListActiveOnDisable)
            element?.SetActive(true);

        foreach (var element in ObjectListDectiveOnDisable)
            element?.SetActive(false);

        OnDisableEvent.Invoke();

        StopAllCoroutines();
    }

    private void OnDestroy() {
        ObjectListActiveOnDisable = null;
        ObjectListDectiveOnDisable = null;
    }

    IEnumerator InvokeEvent() {
        yield return new WaitForSeconds(timeDelay);

        OnDelayEvent.Invoke();
    }

    private void OnApplicationFocus(bool focus) {
        if (!gameObject.activeInHierarchy)
            return;
        if(focus) { 
            onFocusEvent.Invoke();
        }
    }
}
