using agora_gaming_rtc;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class AgoraCustomTextureSender : MonoBehaviour {

    [SerializeField]
    private RenderTexture renderTex;

    private Texture2D imgTexture;
    private Coroutine sendTextureRoutine;

    public void StartSendingTextureUpdates() {
        sendTextureRoutine = StartCoroutine(SendTexture());
    }

    public void StopSendingTextureRoutine() {
        if (sendTextureRoutine != null) {
            StopCoroutine(sendTextureRoutine);
        }
    }

    private void OnDisable() {
        StopSendingTextureRoutine();
    }

    private IEnumerator SendTexture() {
        while (true) {
            MapRenderTextureToTex2D();
            // Gets the Raw Texture data from the texture and apply it to an array of bytes.
            byte[] bytes = imgTexture.GetRawTextureData();
            // Gives enough space for the bytes array.
            int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
            // Checks whether the IRtcEngine instance is existed.
            IRtcEngine rtc = IRtcEngine.QueryEngine();
            if (rtc != null) {
                // Creates a new external video frame.
                ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();
                // Sets the buffer type of the video frame.
                externalVideoFrame.type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
                // Sets the format of the video pixel.
                externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA;
                // Applies raw data.
                externalVideoFrame.buffer = bytes;
                // Sets the width (pixel) of the video frame.
                externalVideoFrame.stride = (int)imgTexture.width;
                // Sets the height (pixel) of the video frame.
                externalVideoFrame.height = (int)imgTexture.height;
                // Removes pixels from the sides of the frame
                //externalVideoFrame.cropLeft = 10;
                //externalVideoFrame.cropTop = 10;
                //externalVideoFrame.cropRight = 10;
                //externalVideoFrame.cropBottom = 10;
                // Rotates the video frame (0, 90, 180, or 270)
                externalVideoFrame.rotation = 180;
                // Calculates the video timestamp in milliseconds according to the system time.
                externalVideoFrame.timestamp = System.DateTime.Now.Ticks / 10000;
                // Pushes the external video frame with the frame you create.
                int a = rtc.PushVideoFrame(externalVideoFrame);

                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void MapRenderTextureToTex2D() {
        if (imgTexture == null) {
            imgTexture = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);
        }
        var currentRT = RenderTexture.active;
        RenderTexture.active = renderTex;
        imgTexture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        //Graphics.Blit(rawImageRef.texture, renderTex, matToBlit);//Enable for use without camera
        imgTexture.Apply();
        RenderTexture.active = currentRT;
    }
}
