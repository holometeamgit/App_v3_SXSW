using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.InstagramKit;
using VoxelBusters.InstagramKit.Common.Utility;

public class PnlShareOptions : MonoBehaviour
{
    [SerializeField]
    Button btnInstagram;

    [SerializeField]
    S3Handler s3Handler;

    bool isVideo;

    Texture2D image;

    string hologramName;

    private void Awake()
    {
        btnInstagram.onClick.AddListener(ShareAsStory);
    }

    public void Activate(bool isVideo, Texture2D image, string hologramName)//Flag is required as image may not always be null
    {
        this.isVideo = isVideo;
        this.image = image;
        this.hologramName = hologramName;
        gameObject.SetActive(true);
        btnInstagram.interactable = InstagramKitManager.IsAvailable();
        //UploadToS3();

        //print("isVideo = " + isVideo);
    }

    private void UploadToS3()
    {
        if (!string.IsNullOrEmpty(PnlPostRecord.LastRecordingPath))
        {
            s3Handler.UploadFile(PnlPostRecord.LastRecordingPath, "Share_Upload_", "UserContent");
        }
        else
        {
            Debug.LogError("Last recording path was null or empty");
        }
    }

    private void ShareAsStory()
    {
        AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyShareHologramMediaPressed, AnalyticParameters.ParamVideoName, hologramName);

        if (isVideo)
        {
            if (!string.IsNullOrEmpty(PnlPostRecord.LastRecordingPath))
            {
                AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeyVideoShared, AnalyticParameters.ParamVideoName, hologramName);
                //print("ShareAsStory Called " + "Path is = " + path);
                StoryContent content = new StoryContent(PnlPostRecord.LastRecordingPath, isVideo);


                //Add any extra data like sticker or caption text or target attachment url
                //Sticker sticker = GetSticker();
                //string attachmentURL = GetAttachmentURL();

                //content.SetSticker(sticker);
                //content.SetAttachmentUrl(attachmentURL);

                InstagramKitManager.Share(content, OnShareComplete);
            }

            /* // Another way to pass the callback
            InstagramKitManager.Instance.Share(content, (bool success, string error) => 
                {
                    string message  =  success ? "Successfully Shared" : "Failed to share " + error;
                    Log(message);       
                });
             */
            //print("ShareAsStory Exit");
        }
        else
        {
            AnalyticsController.Instance.SendCustomEvent(AnalyticKeys.KeySnapshotShared, AnalyticParameters.ParamVideoName, hologramName);
            byte[] data = null;
            data = ImageConversion.EncodeToJPG(image);
            string saveDir = Application.persistentDataPath + "/" + "InstagramSharePlaceholder.jpg";
            FileOperations.WriteAllBytes(saveDir, data);

            StoryContent content = new StoryContent(saveDir, isVideo);
            InstagramKitManager.Share(content, OnShareComplete);
        }
    }

    private void OnShareComplete(bool success, string error)
    {
        //string message = success ? "Successfully Shared" : "Failed to share " + error;
        //Debug.Log(message);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError(error);
        }
    }

}
