using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchCounter : IPointerDownHandler, IPointerUpHandler {

    private int touchCount;

    public int TouchCount {
        get {
            return touchCount;
        }
    }

    private List<int> touchID = new List<int>(6);

    public List<int> TouchID {
        get {
            return touchID;
        }
    }

    private Dictionary<int, PointerEventData> touchIDData = new Dictionary<int, PointerEventData>();

    public Dictionary<int, PointerEventData> TouchIDData {
        get {
            return touchIDData;
        }
    }

    public float TouchPerimeter {
        get {
            float tempDistance = 0;

            for (int i = 0; i < touchCount; i++) {
                if (i < touchCount - 1) {
                    tempDistance += Vector2.Distance(touchIDData[touchID[i]].position, touchIDData[touchID[i + 1]].position);
                } else {
                    tempDistance += Vector2.Distance(touchIDData[touchID[i]].position, touchIDData[touchID[0]].position);
                }
            }

            return tempDistance;
        }
    }

    public void OnPointerDown(PointerEventData data) {
        if (touchID.Contains(data.pointerId)) {
            return;
        }

        touchID.Add(data.pointerId);
        touchIDData.Add(data.pointerId, data);
        touchCount++;
    }

    public void OnPointerUp(PointerEventData data) {
        if (touchID.Contains(data.pointerId)) {
            touchID.Remove(data.pointerId);
            touchIDData.Remove(data.pointerId);
            touchCount--;
            return;
        }
    }
}
