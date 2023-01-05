using UnityEngine;
using Firebase.Storage;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System;
using UnityEditor.VersionControl;
using UnityEngine.UI;
using System.Runtime.Remoting.Contexts;

public class FirebaseStorageController {

    private FirebaseStorage _storage;

    private const string RAW_VIDEO_FOLDER = "gcp_storage_ref_source";

    public FirebaseStorageController() {
        _storage = FirebaseStorage.DefaultInstance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path to file</param>
    /// <returns></returns>
    public async Task<string> Upload(string path, IProgress<UploadState> uploadingFirebaseProgress) {
        HelperFunctions.DevLog("start uploading: " + path);

        //TaskCompletionSource<string> result = new TaskCompletionSource<string>();

        string resultURL = "";
        string filename = Path.GetFileName(path);

        if (!File.Exists(path)) {
            Debug.LogError($"File {path} not exist");
            return null;
        }

        Debug.Log("filename: " + filename);

        long userId = Beem.SSO.CallBacks.onUserId?.Invoke() ?? -1;

        var reference = _storage.GetReference($"/{RAW_VIDEO_FOLDER}/{userId}/{filename}");

        var storageMetadata = await reference.PutFileAsync(path, progressHandler: uploadingFirebaseProgress);

        //var uploadingTask = reference.PutFileAsync(path, progressHandler: uploadingFirebaseProgress);

        if (storageMetadata == null) {
            throw new Exception();
        }

        Uri url = await reference.GetDownloadUrlAsync();

        if (url == null) {
            throw new Exception();
        }

        resultURL = url.OriginalString;

        return resultURL;
    }

    public class UploadingFirebaseProgress : IProgress<UploadState> {

        public void Report(UploadState value) {
            Debug.Log($"Progress: {value.BytesTransferred / value.TotalByteCount * 100}%");
        }

    }

}
