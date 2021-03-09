using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Clear Log
    /// </summary>
    public class ClearBtn : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            LogCallBacks.OnClear?.Invoke();
        }
    }
}
