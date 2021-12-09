using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Beem.ARMsg;


public class UIUploaderARMsg : MonoBehaviour {
    [SerializeField]
    private UnityEvent OnARMsgUpdloadedEvent;

    private void Awake() {
        CallBacks.OnARMsgUpdloaded += OnARMsgUpdloaded;
    }

    private void OnEnable() {
        CallBacks.OnUpdloadingUIOpened?.Invoke();
    }

    private void OnDestroy() {
        CallBacks.OnARMsgUpdloaded -= OnARMsgUpdloaded;
    }

    private void OnARMsgUpdloaded() {
        OnARMsgUpdloadedEvent?.Invoke();
    }
}
