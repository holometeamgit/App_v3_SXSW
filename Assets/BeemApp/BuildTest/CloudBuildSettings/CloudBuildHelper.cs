using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudBuildHelper : MonoBehaviour {
    [System.Serializable]
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

    [SerializeField]
    private Text manifestInfo;

    private void Start() {
        var manifest = (TextAsset)Resources.Load("UnityCloudBuildManifest.json");
        if (manifest != null) {
            var data = JsonUtility.FromJson<UnityCloudBuildManifestData>(manifest.text);
            manifestInfo.text = data.scmCommitId + "\n" +
                 data.scmBranch + "\n" +
                   data.buildNumber + "\n" +
                    data.buildStartTime + "\n" +
                       data.projectId + "\n" +
                          data.bundleId + "\n" +
                            data.unityVersion + "\n" +
                               data.xcodeVersion + "\n" +
                                 data.cloudBuildTargetName;

        }

    }
}
