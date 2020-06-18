using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enabler : MonoBehaviour
{
    public List<GameObject> ObjectListActiveOnEnable;
    public List<GameObject> ObjectListDectiveOnEnable;

    [Space]
    public List<GameObject> ObjectListActiveOnDisable;
    public List<GameObject> ObjectListDectiveOnDisable;

    private void OnEnable() {
        foreach (var element in ObjectListActiveOnEnable)
            element.SetActive(true);

        foreach (var element in ObjectListDectiveOnEnable)
            element.SetActive(false);
    }

    private void OnDisable() {
        foreach (var element in ObjectListActiveOnDisable)
            element.SetActive(true);

        foreach (var element in ObjectListDectiveOnDisable)
            element.SetActive(false);
    }
}
