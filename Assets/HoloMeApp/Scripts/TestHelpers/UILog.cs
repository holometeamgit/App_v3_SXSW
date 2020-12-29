using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILog : MonoBehaviour {
    string myLog;
    Queue myLogQueue = new Queue();
    [SerializeField] Text text;
    [SerializeField] Text versionText;
    [SerializeField] GameObject canvasLog;
    [SerializeField] GameObject scrollView;

    public void SwitchEnable() {
        scrollView.SetActive(!scrollView.activeSelf);
    }

    public void ClearLog() {
        myLogQueue.Clear();
        text.text = "";
    }

    void Start() {
        versionText.text = "Version " + Application.version;
    }

    void OnEnable() {
        if (Debug.isDebugBuild) {
            canvasLog.SetActive(true);
            Application.logMessageReceived += HandleLog;
        }
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;

        if (type == LogType.Warning) {
            newString = "<color=yellow>" + newString + "</color> ";
        } else if (type == LogType.Exception) {
            newString = "<color=orange>" + newString + "</color> ";
        }

        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception) {
            newString = "\n<color=orange>" + stackTrace + "</color>";
            myLogQueue.Enqueue(newString);
        }
        myLog = string.Empty;
        foreach (string mylog in myLogQueue) {
            myLog += mylog;
        }
        if(text != null)
            text.text = myLog;
    }

}