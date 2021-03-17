using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Switcher : MonoBehaviour
{
    [SerializeField]
    bool doOnlyWithActiveObject;
    [Tooltip("Activates all GameObject in switchOnList before deactivating switchOffList.")]
    [SerializeField] List<GameObject> switchOnList;
    [Tooltip("Deactivates all GameObject in switchOffList after activating switchOnList.")]
    [SerializeField] List<GameObject> switchOffList;
    [SerializeField] UnityEvent OnSwitch;

    public void Switch() {
        foreach(var element in switchOnList) {
            element?.SetActive(true);
        }

        foreach (var element in switchOffList) {
            element?.SetActive(false);
        }

        OnSwitch.Invoke();
    }
}
