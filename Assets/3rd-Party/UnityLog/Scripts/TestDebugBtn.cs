using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Beem.Utility.UnityConsole {
    /// <summary>
    /// Test Logs
    /// </summary>
    public class TestDebugBtn : MonoBehaviour, IPointerClickHandler {
        public void OnPointerClick(PointerEventData eventData) {
            Debug.Log("Log");
            Debug.LogError("LogError");
            Debug.LogWarning("LogWarning");
        }
    }
}
