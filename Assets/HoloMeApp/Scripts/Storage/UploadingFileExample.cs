using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Beem.ARMsg.ARMsgController;
using static UnityEngine.Networking.UnityWebRequest;

public class UploadingFileExample : MonoBehaviour
{
    private FirebaseStorageController _firebaseStorageController;

    private void Awake()
    {
        _firebaseStorageController = new FirebaseStorageController();
    }

    private void Start()
    {
        Uploading();
    }

    private void Uploading()
    {
        string path = $"{Application.dataPath}/Resources/screen.mov";
        Debug.Log(path);


        ProcessingFirebaseStorageUploading processingFirebaseStorageUploading = new ProcessingFirebaseStorageUploading();

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        _firebaseStorageController.Upload(path, processingFirebaseStorageUploading).ContinueWith(x => {
            Debug.Log(x.Result);
        }, taskScheduler) ;
    }
}
