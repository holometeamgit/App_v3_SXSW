using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ThumbnailDownloadManager : MonoBehaviour
{
    [SerializeField]
    S3Handler s3Handler;

    int thumbnailFiles;
    int thumbnailSeverCount;
    int thumbnailImagesDownloaded;

    public Action OnThumbnailsDataDownloaded;

    List<VideoThumbnailJsonData> allThumbnailData = new List<VideoThumbnailJsonData>();
    public List<VideoThumbnailJsonData> videoThumbnailShowcaseJsonDatas = new List<VideoThumbnailJsonData>();
    public List<VideoThumbnailJsonData> videoThumbnailUserJsonDatas = new List<VideoThumbnailJsonData>();

    void IncrementThumbnailJsonDownloaded(bool success)
    {
        thumbnailFiles++;
        if (thumbnailFiles == thumbnailSeverCount)
        {
            DownloadThumbnailImageFiles();
        }
    }

    void IncrementThumbnailDownloaded(bool success)
    {
        thumbnailImagesDownloaded++;
        if (thumbnailImagesDownloaded == thumbnailSeverCount)
        {
            foreach (var data in allThumbnailData)
            {
                if (data.showcaseHologram)
                    videoThumbnailShowcaseJsonDatas.Add(data);
                else
                    videoThumbnailUserJsonDatas.Add(data);
            }

            allThumbnailData.Clear();
            OnThumbnailsDataDownloaded?.Invoke();
        }
    }

    void DownloadThumbnailImageFiles()
    {
        foreach (var data in s3Handler.thumbnailData.OrderBy(x => (int.Parse(Regex.Match(x.Key, @"\d+").Value)))) //Sort by value in json file names
        {
            VideoThumbnailJsonData videoThumbnailJsonData = JsonParser.CreateFromJSON<VideoThumbnailJsonData>(JsonParser.ParseFileName(data.Value.FileName));
            allThumbnailData.Add(videoThumbnailJsonData);
            s3Handler.DownloadGeneric(videoThumbnailJsonData.imageURL, data.Value, IncrementThumbnailDownloaded);
        }
    }

    public void DownloadVideoThumbnails()
    {
        if (!s3Handler.Ready || !s3Handler.PopulateComplete)
        {
            Debug.LogError("S3 Handler wasn't ready so can't get thumbnails");
            return;
        }

        thumbnailSeverCount = s3Handler.thumbnailData.Count;
        thumbnailFiles = 0;
        thumbnailImagesDownloaded = 0;
        foreach (var data in s3Handler.thumbnailData)
        {
            s3Handler.DownloadGeneric(data.Value.FileName, data.Value, OnDownloadCompleteOneOff: IncrementThumbnailJsonDownloaded);
        }
    }
}
