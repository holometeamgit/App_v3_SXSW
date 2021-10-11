using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class RoomPopupShowChecker : MonoBehaviour {
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

    // подписаться на все события при которых будет окно закрываться (так как некоторые окна могу открываться с задержкой и нужно прервать отображения popup)
    // добавить событие открытия стрима и добавить событие закрытие стрима
    // добавить перепроверку в цикле в pnlroompopupcontroller на случай если пытется открыть но не может. отличать ожидание от прерывания
    // добавить все нужные объекты с главной сцены
}
