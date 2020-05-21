using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.InstagramKit;

public class PnlShareOptions : MonoBehaviour
{
    [SerializeField]
    Button btnInstagram;

    [SerializeField]
    S3Handler s3Handler;

    private void Awake()
    {
        btnInstagram.onClick.AddListener(() => ShareAsStory(true));
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        btnInstagram.interactable = InstagramKitManager.IsAvailable();

        if (!string.IsNullOrEmpty(PnlPostRecord.LastRecordingPath))
        {
            s3Handler.UploadFile(PnlPostRecord.LastRecordingPath, "Share_Upload_", "UserContent");
        }
        else
        {
            Debug.LogError("Last recording path was null or empty");
        }
    }

    private void ShareAsStory(bool isVideo)
    {
        if (!string.IsNullOrEmpty(PnlPostRecord.LastRecordingPath))
        {
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

    private void OnShareComplete(bool success, string error)
    {
        //string message = success ? "Successfully Shared" : "Failed to share " + error;
        //Debug.Log(message);
    }

}
