using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// checks requirements for opening PnlRoomPopup
/// </summary>
public class PopupShowChecker : MonoBehaviour {
    [SerializeField]
    List<GameObject> _needBeActivatedObjects;

    [SerializeField]
    List<GameObject> _needBeDeactivatedObjects;

    public bool CanShow() {
        foreach (var obj in _needBeActivatedObjects) {
            if (!obj.activeInHierarchy) {
                return false;
            }
        }

        foreach (var obj in _needBeDeactivatedObjects) {
            if (obj.activeInHierarchy) {
                return false;
            }
        }

        return true;
    }
}
