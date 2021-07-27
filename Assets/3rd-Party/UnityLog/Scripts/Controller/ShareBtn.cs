using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Share Btn
    /// </summary>
    public class ShareBtn : MonoBehaviour, IPointerClickHandler {

        private const string LOGS = "Logs";

        public void OnPointerClick(PointerEventData eventData) {
            string filePath = Application.temporaryCachePath + "/" + LOGS + ".txt";

            StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine(LogData.GetLog());
            writer.Close();
#if !UNITY_EDITOR
              new NativeShare().AddFile(filePath).SetText(LOGS).Share();
#else
            Debug.Log("Share Log:" + LogData.GetLog());
#endif
        }
    }
}
