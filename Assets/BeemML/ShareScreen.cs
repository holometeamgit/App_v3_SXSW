using System.Collections;
using UnityEngine;
using agora_gaming_rtc;
using System.Runtime.InteropServices;

public class ShareScreen : MonoBehaviour
{
    private Texture2D _texture;
    private Rect _rect;
    
    [SerializeField] private string appId = "Your_AppID";
    [SerializeField] private string channelName = "agora";
    private IRtcEngine _rtcEngine;

    private void Start()
    {
        Debug.Log("ScreenShare Activated");
        _rtcEngine = IRtcEngine.GetEngine(appId);
        
        // Debug.Log("_rtcEngine.SetLogFilter");
        // _rtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | 
        //                         LOG_FILTER.WARNING | LOG_FILTER.ERROR |
        //                         LOG_FILTER.CRITICAL);
        
        _rtcEngine.EnableVideo();
        _rtcEngine.EnableVideoObserver();
        _rtcEngine.SetExternalVideoSource(true, false);
        _rtcEngine.JoinChannel(channelName, null, 0);
        _rect = new Rect(0, 0, Screen.width, Screen.height);
        _texture = new Texture2D((int)_rect.width, (int)_rect.height,
                                 TextureFormat.BGRA32, false);
    }

    private void Update()
    {
        StartCoroutine(ShareScreenRoutine());
    }

    private IEnumerator ShareScreenRoutine()
    {
        yield return new WaitForEndOfFrame();
        
        _texture.ReadPixels(_rect, 0, 0);
        _texture.Apply();
        
        var bytes = _texture.GetRawTextureData();
        var size = Marshal.SizeOf(bytes[0]) * bytes.Length;
        var rtc = IRtcEngine.QueryEngine();

        if (rtc == null)
        {
            yield break;
        }
        
        var externalVideoFrame = new ExternalVideoFrame
        {
            type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA,
            format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_BGRA,
            buffer = bytes,
            stride = (int)_rect.width,
            height = (int)_rect.height,
            cropLeft = 10,
            cropTop = 10,
            cropRight = 10,
            cropBottom = 10,
            rotation = 180,
            timestamp = System.DateTime.Now.Ticks / 10000
        };
        
        var a = rtc.PushVideoFrame(externalVideoFrame);
    }
}
