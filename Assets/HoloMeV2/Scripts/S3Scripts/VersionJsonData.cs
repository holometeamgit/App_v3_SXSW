using System;
using UnityEngine;

[Serializable]
public class VersionJsonData
{
    public string versionIOS;
    public string versionAndroid;
    public bool allowOldVersions;

    public static VersionJsonData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<VersionJsonData>(jsonString);
    }
}
