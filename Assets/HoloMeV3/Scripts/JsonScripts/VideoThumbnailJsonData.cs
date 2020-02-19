using UnityEngine;

public class VideoThumbnailJsonData
{
    public bool showcaseHologram;
    public string code;
    public string imageURL;

    public static VideoThumbnailJsonData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<VideoThumbnailJsonData>(jsonString);
    }
}
