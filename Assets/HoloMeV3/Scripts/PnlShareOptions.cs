using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.InstagramKit;

public class PnlShareOptions : MonoBehaviour
{
    [SerializeField]
    Button btnInstagram;

    private void Start()
    {
        string dir = PnlPostRecord.LastRecordingPath;
        btnInstagram.onClick.AddListener(() => ShareAsStory(dir, true));
    }

    private void OnEnable()
    {
        btnInstagram.interactable = InstagramKitManager.IsAvailable();
    }

    private void ShareAsStory(string path, bool isVideo)
    {
        StoryContent content = new StoryContent(path, isVideo);

        // Add any extra data like sticker or caption text or target attachment url
        //Sticker sticker = GetSticker();
        //string attachmentURL = GetAttachmentURL();

        //content.SetSticker(sticker);
        //content.SetAttachmentUrl(attachmentURL);

        InstagramKitManager.Share(content, OnShareComplete);

        /* // Another way to pass the callback
        InstagramKitManager.Instance.Share(content, (bool success, string error) => 
            {
                string message  =  success ? "Successfully Shared" : "Failed to share " + error;
                Log(message);       
            });
         */
    }

    private void OnShareComplete(bool success, string error)
    {
        //string message = success ? "Successfully Shared" : "Failed to share " + error;
        //Debug.Log(message);
    }

}
