using System;

/// <summary>
/// Unity Cloud Manifest Data
/// </summary>
[Serializable]
public class UnityCloudBuildManifestData {
    public string scmCommitId;
    public string scmBranch;
    public string buildNumber;
    public string buildStartTime;
    public string projectId;
    public string bundleId;
    public string unityVersion;
    public string xcodeVersion;
    public string cloudBuildTargetName;
}

