using System.Collections.Generic;
using System;
using UnityEngine;

public class ServerDataHandler : MonoBehaviour
{
    [SerializeField]
    S3Handler s3Handler;

    Dictionary<string, ServerFileData> videoData = new Dictionary<string, ServerFileData>();

    public Action OnDictionaryPopulated;

    public bool DataReceived { get; private set; }

    void Awake()
    {
        //s3Handler.OnReadyForServerCalls += PopulateVideoDictionary;
    }

    public void PopulateVideoDictionary()
    {
        if (s3Handler.Ready)
        {
            s3Handler.PopulateData(OnVideoDataReturned);
        }
        else
        {
            Debug.LogWarning($"{nameof(S3Handler)} wasn't ready to populate dictionary");
        }
    }

    public void AssignDownloadProgressAction(Action<float> action)
    {
        s3Handler.OnDownloadProgressUpdate += action;
    }

    public void AssignDownloadCompleteAction(Action action)
    {
        s3Handler.OnDownloadVideoComplete += action;
    }

    public void AssignDownloadFailedAction(Action action)
    {
        s3Handler.OnDownloadFailed += action;
    }

    public void InvokeDownloadFailedEvent()
    {
        s3Handler.OnDownloadFailed?.Invoke();
    }

    void OnVideoDataReturned(Dictionary<string, ServerFileData> newVideoData)
    {
        videoData.Clear();
        videoData = newVideoData;

        DataReceived = true;
        OnDictionaryPopulated?.Invoke();
    }

    public ServerFileData GetVideoData(string code)
    {
        if (videoData.ContainsKey(code))
        {
            return videoData[code];
        }
        //Debug.LogError("Video Not Found");
        return null;
    }

    public void DownloadOrPlayLocalVideo(string code)
    {
        if (HelperFunctions.DoesFileExist(code))
        {
            s3Handler.OnDownloadVideoComplete?.Invoke();
        }
        else
        {
            s3Handler.DownloadVideo(videoData[code].FileName, code + HelperFunctions.GetExtension(videoData[code].FileName));
        }
    }

    /// <summary>
    /// For downloading non code video files images etc
    /// </summary>
    /// <param name="fileName">INCLUDE FILE EXTENSION</param>
    public void DownloadSupplementaryFile(string fileName)
    {
        if (HelperFunctions.DoesFileExist(fileName))
        {
            s3Handler.OnDownloadVideoComplete?.Invoke();
        }
        else
        {
            s3Handler.DownloadVideo(fileName, fileName);
        }
    }
}
