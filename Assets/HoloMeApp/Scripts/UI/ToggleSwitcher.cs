using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleSwitcher : MonoBehaviour
{
    public UnityEvent OnIsOn;
    public UnityEvent OnIsOff;

    public void OnChangeIsOn(bool isOn) {
        if(isOn) {
            OnIsOn.Invoke();
        } else {
            OnIsOff.Invoke();
        }
    }
}
