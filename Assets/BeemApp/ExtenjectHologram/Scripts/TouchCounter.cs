using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Beem.Extenject.Hologram {

    /// <summary>
    /// Touch Counter
    /// </summary>
    public class TouchCounter : IPointerDownHandler, IPointerUpHandler {

        private int touchCount;

        /// <summary>
        /// Touch Count
        /// </summary>
        public int TouchCount {
            get {
                return touchCount;
            }
            private set {
                if (touchCount != value) {
                    touchCount = value;
                    onTouchCountChange?.Invoke();
                }
            }
        }

        public Action onTouchCountChange = delegate { };

        private List<int> touchID = new List<int>(6);

        /// <summary>
        /// Current Touch ID
        /// </summary>
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

        /// <summary>
        /// TouchPerimeter
        /// </summary>
        public float TouchPerimeter {
            get {
                float tempDistance = 0;

                for (int i = 0; i < TouchCount; i++) {
                    if (i < TouchCount - 1) {
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
            TouchCount++;
        }

        public void OnPointerUp(PointerEventData data) {
            if (touchID.Contains(data.pointerId)) {
                touchID.Remove(data.pointerId);
                touchIDData.Remove(data.pointerId);
                TouchCount--;
                return;
            }
        }
    }
}
