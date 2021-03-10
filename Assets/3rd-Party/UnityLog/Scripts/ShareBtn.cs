using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Share Btn
    /// </summary>
    public class ShareBtn : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            LogCallBacks.OnShare?.Invoke();
        }
    }
}
