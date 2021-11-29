using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoingRecordController : MonoBehaviour
{
    [SerializeField] List<Counter> counters;

    [SerializeField] List<int> timersScaleList = new List<int> { 5, 10, 15 };

    private const int START_VALUE = 0;

    private void Awake() {
        ChangeVaiting(START_VALUE);
#if UNITY_EDITOR
        ChangeVaiting(0);
#endif
    }

    public void ChangeVaiting(int value) {
        if (value > timersScaleList.Count - 1)
            return;
        SetNewValue(timersScaleList[value]);
    }

    private void SetNewValue(int timerScale) {
        foreach(var conter in counters) {
            conter.SetCounterTime(timerScale);
        }
    }
}
