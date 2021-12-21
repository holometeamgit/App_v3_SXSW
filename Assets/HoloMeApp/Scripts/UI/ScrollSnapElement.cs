using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ScrollSnap Element
/// </summary>
public class ScrollSnapElement : MonoBehaviour {

    [SerializeField]
    private int elementNumber;

    private ScrollSnap scrollSnap;

    private List<IScrollSnapElementView> scrollSnapElements;

    private void OnEnable() {
        scrollSnap = GetComponentInParent<ScrollSnap>();
        scrollSnapElements = GetComponentsInChildren<IScrollSnapElementView>().ToList();
        scrollSnap.onProgress.AddListener(OnProgress);
    }

    private void OnDisable() {
        scrollSnap.onProgress.RemoveListener(OnProgress);
    }

    /// <summary>
    /// On Scroll Position Changed
    /// </summary>
    /// <param name="value"></param>
    public void OnProgress(float value, int maxNumber) {
        foreach (var item in scrollSnapElements) {
            item.OnProgress(elementNumber, value, maxNumber);
        }
    }
}
